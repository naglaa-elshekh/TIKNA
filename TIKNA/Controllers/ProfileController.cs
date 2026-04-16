using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [Authorize(Roles = "Student")] // التعديل هنا: هيسمح فقط للطلاب بالوصول للـ API ده
    [HttpGet("GetDashboardData")]
    public async Task<IActionResult> GetDashboardData()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null) return NotFound("User not found");

        // دي الخطوة اللي بتمنع الـ null: بنجيب لستة الرولز كاملة
        var roles = await _userManager.GetRolesAsync(user);

        var profileInfo = new
        {
            user.Name,
            user.Email,
            user.Address,
            user.University,
            user.PhoneNumber ,

            // بنبعت أول رول موجود في القائمة للفرونت إند
            Role = roles.FirstOrDefault()
        };

        // التمييز الدقيق باستخدام Contains
        if (roles.Contains("Admin"))
        {
            return Ok(new { Info = profileInfo, Message = "مرحباً بك في لوحة تحكم الإدارة" });
        }

        if (roles.Contains("Company"))
        {
            var myProducts = await _context.Products
                .Where(p => p.ApplicationUserId == currentUserId)
                .ToListAsync();

            return Ok(new { Info = profileInfo, Products = myProducts });
        }

        if (roles.Contains("Student"))
        {
            var myOrders = await _context.Orders.Where(o => o.BuyerId == currentUserId).ToListAsync();
            var myMaintenance = await _context.MaintenanceRequests.Where(m => m.UserId == currentUserId).ToListAsync();

            return Ok(new { Info = profileInfo, Orders = myOrders, Maintenance = myMaintenance });
        }

        return BadRequest("User has no assigned role");
    }
}