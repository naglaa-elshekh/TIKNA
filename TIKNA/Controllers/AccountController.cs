using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TIKNA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        // 1. تسجيل مستخدم جديد (طالب أو شركة)
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null) return BadRequest("الإيميل مسجل بالفعل.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                UserType = dto.UserType,
                // لو شركة تبدأ بـ Pending، لو فرد Approved علطول
                ApprovalStatus = (dto.UserType == "Company") ? "Pending" : "Approved"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.UserType);

            // حفظ الاسم الكامل والبيانات في جدول الـ Customer
            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                Name = dto.Name,
                Address = "Not Specified",
                Phone = "0000000000"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var token = await GenerateJwtToken(user);
            return Ok(new { token, message = "تم التسجيل بنجاح" });
        }

        // 2. تسجيل الدخول
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var token = await GenerateJwtToken(user);
                return Ok(new
                {
                    token,
                    userType = user.UserType,
                    status = user.ApprovalStatus
                });
            }
            return Unauthorized("بيانات الدخول غير صحيحة.");
        }

        // 3. عرض الشركات التي تنتظر الموافقة (للأدمن فقط)
        [Authorize(Roles = "Admin")]
        [HttpGet("GetPendingCompanies")]
        public async Task<IActionResult> GetPendingCompanies()
        {
            var pendingUsers = await _userManager.Users
                .Where(u => u.UserType == "Company" && u.ApprovalStatus == "Pending")
                .ToListAsync();

            return Ok(pendingUsers);
        }

        // 4. الموافقة أو الرفض (للأدمن فقط)
        [Authorize(Roles = "Admin")]
        [HttpPut("ApproveOrRejectCompany")]
        public async Task<IActionResult> ApproveOrRejectCompany(string userId, string status)
        {
            // status لازم تكون "Approved" أو "Rejected"
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("المستخدم غير موجود");

            user.ApprovalStatus = status;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { message = $"تم تحديث حالة الشركة إلى {status}" });

            return BadRequest(result.Errors);
        }

        // 5. ميثود مساعدة لتوليد التوكن
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("UserStatus", user.ApprovalStatus?? "Pending"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}