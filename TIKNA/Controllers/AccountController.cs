using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TIKNA.DTOs;
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
        // ... (نفس الـ Using الموجودة عندك)

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
                UserType = dto.UserType, // التأكد من أن القيمة تأتي (Student أو Company)
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
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // --- التعديل الجوهري هنا لضمان عدم وجود Null ---
            // توحيد حالة الأحرف لتجنب الأخطاء (مثلاً: Student تصبح STUDENT)
            string roleName = dto.UserType;

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // إضافة المستخدم للرول
            await _userManager.AddToRoleAsync(user, roleName);

            return Ok(new
            {
                message = user.UserType.ToLower() == "company"
                    ? "تم إنشاء حسابك بنجاح. يرجى انتظار موافقة الإدارة."
                    : "تم إنشاء حسابك بنجاح! يمكنك الآن تسجيل الدخول.",
                userType = user.UserType,
                fullName = user.Name
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                if (user.ApprovalStatus == "Rejected")
                    return BadRequest("تم رفض هذا الحساب من قبل الإدارة.");

                // جلب الأدوار بشكل صريح
                var roles = await _userManager.GetRolesAsync(user);
                var token = await GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    userId = user.Id,
                    role = roles.FirstOrDefault(), // سيعود الآن بـ Student أو Company وليس Null
                    userType = user.UserType,
                    status = user.ApprovalStatus,
                    fullName = user.Name
                });
            }
            return Unauthorized("بيانات الدخول غير صحيحة.");
        }

        // في ميثود توليد التوكن، تأكدي من إضافة الـ Claim الخاص بالرول هكذا:
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim("UserType", user.UserType),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            // إضافة الأدوار للتوكن ضروري جداً لتشغيل [Authorize(Roles="...")]
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("09686dfghgjkfmvnbbhu4768797784hvftyr8954hvbnncrfuirt"));

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