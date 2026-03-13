using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TIKNA.Models;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1. إنشاء كائن المستخدم (Identity User)
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            // ضيفي أي بيانات إضافية موجودة في الـ ApplicationUser
        };

        // 2. حفظ المستخدم وتشفير الباسورد
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            // 3. الربط الآلي: إنشاء سطر في جدول الـ Customer
            // هنا بنربط الـ GUID (user.Id) بالـ Customer الجديد
            var customer = new Customer
            {
                ApplicationUserId = user.Id, // ده الـ GUID اللي جاي من الـ Identity
                Name = dto.Name,         // تأكدي إن الاسم موجود في الـ DTO
                Phone = dto.Phone
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // 4. (اختياري) إضافة دور الـ User العادي
            // await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { Message = "تم التسجيل بنجاح وإنشاء بروفايل العميل" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Invalid login");

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // إضافة الأدوار للـ Token عشان الـ [Authorize(Roles="...")] تشتغل
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}