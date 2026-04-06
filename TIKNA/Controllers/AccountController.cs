using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TIKNA.Models;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.Name,
            PhoneNumber = dto.Phone,
            UserType = dto.UserType, // "Individual" أو "Company"
            // لو مستخدم عادي يكون مقبول فوراً، لو شركة يبقى false لحد ما الأدمن يقبل
            IsApproved = (dto.UserType == "Individual")
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            var customer = new Customer
            {
                ApplicationUserId = user.Id,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address ?? "N/A"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // إضافة دور طالب بشكل افتراضي (ممكن تضيفي دور Company لو حبيتي)
            var roleName = dto.UserType == "Company" ? "Company" : "Student";
            if (!await _roleManager.RoleExistsAsync(roleName)) await _roleManager.CreateAsync(new IdentityRole(roleName));
            await _userManager.AddToRoleAsync(user, roleName);

            string message = user.IsApproved ? "تم التسجيل بنجاح" : "تم تسجيل طلب الشركة بنجاح وفي انتظار موافقة الإدارة";
            return Ok(new { Message = message });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("الإيميل أو كلمة المرور غير صحيحة");

        // --- القفل الجديد: لو مش مقبول (شركة لسه ما وافقش عليها الأدمن) يرفض الدخول ---
        if (!user.IsApproved)
            return BadRequest("عفواً، حسابك قيد المراجعة من قِبل الإدارة حالياً.");

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("FullName", user.FullName ?? ""),
            new Claim("UserType", user.UserType), // ضيفنا نوع اليوزر في التوكن
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    // --- قسم الأدمن: عرض طلبات الشركات والقبول ---

    [Authorize(Roles = "Admin")]
    [HttpGet("PendingCompanies")]
    public async Task<IActionResult> GetPendingCompanies()
    {
        // عرض كل المستخدمين اللي نوعهم شركة ولسه مش مقبولة
        var pending = await _userManager.Users
            .Where(u => u.UserType == "Company" && !u.IsApproved)
            .Select(u => new { u.Id, u.FullName, u.Email, u.PhoneNumber })
            .ToListAsync();

        return Ok(pending);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("ApproveAccount/{userId}")]
    public async Task<IActionResult> ApproveAccount(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("المستخدم غير موجود");

        user.IsApproved = true;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? Ok("تم تفعيل الحساب بنجاح") : BadRequest(result.Errors);
    }

    [HttpPost("seed-admin")]
    public async Task<IActionResult> SeedAdmin()
    {
        if (!await _roleManager.RoleExistsAsync("Admin")) await _roleManager.CreateAsync(new IdentityRole("Admin"));

        var adminEmail = "admin@tikna.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "TIKNA Admin",
                UserType = "Admin",
                IsApproved = true, // الأدمن مقبول دايماً
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(newAdmin, "Admin@123");
            await _userManager.AddToRoleAsync(newAdmin, "Admin");
            return Ok("تم إنشاء حساب الأدمن بنجاح");
        }
        return BadRequest("الأدمن موجود بالفعل");
    }
}