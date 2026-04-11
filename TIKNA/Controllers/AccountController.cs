using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TIKNA.Models;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
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
                Name = dto.Name,
                UserType = dto.UserType,
                Address = dto.Address ?? "Not Specified",
                University = dto.University,
                Faculty = dto.Faculty,
                Bio = dto.CompanyDescription,
                CompanyServiceType = dto.ServiceType,
                CommercialRegister = dto.CompanyRegisterNumber,
                PhoneNumber = dto.Phone,
                ApprovalStatus = (dto.UserType.ToLower() == "company") ? "Pending" : "Approved",
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // --- التأكد من وجود الرول وإضافته (حل مشكلة الـ Null) ---
            if (!await _roleManager.RoleExistsAsync(dto.UserType))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.UserType));
            }
            await _userManager.AddToRoleAsync(user, dto.UserType);

            var token = await GenerateJwtToken(user);

            return Ok(new
            {
                message = user.UserType.ToLower() == "company"
                    ? "أهلاً بك في TIKNA! تم إنشاء حسابك ويمكنك تصفح لوحة التحكم، مع العلم أن تفعيل ميزات البيع والصيانة يتطلب موافقة الإدارة."
                    : "تم التسجيل والدخول بنجاح.",
                token = token,
                userId = user.Id,       // تم الإضافة لربط البروفايل
                userType = user.UserType,
                fullName = user.Name,
                status = user.ApprovalStatus
            });
        }

        // 2. تسجيل الدخول
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                if (user.ApprovalStatus == "Rejected")
                    return BadRequest("تم رفض هذا الحساب.");

                var roles = await _userManager.GetRolesAsync(user);
                var token = await GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    userId = user.Id,           // تم الإضافة
                    role = roles.FirstOrDefault(), // حل مشكلة الـ Role Null
                    userType = user.UserType,
                    status = user.ApprovalStatus,
                    fullName = user.Name
                });
            }
            return Unauthorized("بيانات الدخول غير صحيحة.");
        }

        // 3. عرض الشركات المنتظرة (للأدمن)
        [Authorize(Roles = "Admin")]
        [HttpGet("GetPendingCompanies")]
        public async Task<IActionResult> GetPendingCompanies()
        {
            var pending = await _userManager.Users
                .Where(u => u.UserType == "Company" && u.ApprovalStatus == "Pending")
                .ToListAsync();
            return Ok(pending);
        }

        // 4. موافقة أو رفض (للأدمن)
        [Authorize(Roles = "Admin")]
        [HttpPut("ApproveOrReject")]
        public async Task<IActionResult> ApproveOrReject(string userId, string status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("المستخدم غير موجود");

            user.ApprovalStatus = status;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded && status == "Approved")
            {
                await SendApprovalEmail(user.Email, user.Name);
            }

            return Ok(new { message = $"تم تحديث الحالة إلى {status} وإرسال إشعار للمستخدم." });
        }

        private async Task SendApprovalEmail(string userEmail, string userName)
        {
            var subject = "تم قبول حسابك في منصة TIKNA";
            var body = $"أهلاً {userName}، نود إعلامك بأن الإدارة قد وافقت على طلب انضمامك. يمكنك الآن البدء في رفع منتجاتك واستلام طلبات الصيانة.";
            // هنا يتم استدعاء EmailService الفعلي
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("UserType", user.UserType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(12),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}