using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1️⃣ إنشاء ApplicationUser للـ Login فقط
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Password", error.Description);

                return BadRequest(ModelState);
            }

            // 2️⃣ إنشاء Customer مرتبط بالـ User
            var customer = new Customer
            {
                Name = dto.Name,   // الاسم الكامل هنا
                Phone = dto.Phone,
                Address = dto.Address,
                ApplicationUserId = user.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok("User and Customer created successfully");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                ModelState.AddModelError("UserName", "UserName or Password not valid");
                return BadRequest(ModelState);
            }

            var validPassword = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword)
            {
                ModelState.AddModelError("Password", "UserName or Password not valid");
                return BadRequest(ModelState);
            }

            // إعداد Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // إعداد الـ Token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("tyugkkuytdtyyv4568yyugyvuy67585u67ufuyfyug6757rfgyigiiu"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: "http://localhost:5181/",
                issuer: "http://localhost:4200/",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}