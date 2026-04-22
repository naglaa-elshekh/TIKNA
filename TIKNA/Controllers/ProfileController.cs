using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.DTOs;
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
    [HttpPost("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null) return NotFound("User not found");

        // 1. التأكد من الباسوورد قبل أي تعديل
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
        if (!isPasswordValid)
        {
            return BadRequest("كلمة المرور الحالية غير صحيحة");
        }

        // 2. تحديث البيانات المسموح بها فقط
        user.Address = model.Address;
        user.PhoneNumber = model.PhoneNumber;
        user.Bio = model.Bio; // لو شركة
        user.Faculty = model.Faculty; // لو طالب

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok(new { Message = "تم تحديث البيانات بنجاح" });
        }

        return BadRequest(result.Errors);
    }
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        // 1. التأكد أن الباسوورد الجديد وتأكيده متطابقين
        if (model.NewPassword != model.ConfirmPassword)
        {
            return BadRequest("كلمة المرور الجديدة وتأكيدها غير متطابقين");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null) return NotFound("User not found");

        // 2. استخدام ميثود جاهزة في Identity بتعمل (تأكد من القديم + تغيير للجديد) في خطوة واحدة
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded)
        {
            return Ok(new { Message = "تم تغيير كلمة المرور بنجاح" });
        }

        // بنشوف أول خطأ رجع من السيستم عشان نترجمه
        var error = result.Errors.FirstOrDefault();

        if (error?.Code == "PasswordMismatch")
        {
            // دي الرسالة اللي هتظهر لو الباسوورد القديم اللي دخله غلط
            return BadRequest("كلمة المرور الحالية التي أدخلتها غير صحيحة.");
        }

        // لو فيه أي خطأ تاني (مثلاً السيستم رافض الباسوورد الجديد لأنه ضعيف)
        // هيرجع الأخطاء التقنية عشان المبرمج أو اليوزر يعرف إيه شروط الباسوورد
        return BadRequest(result.Errors);
    }
}