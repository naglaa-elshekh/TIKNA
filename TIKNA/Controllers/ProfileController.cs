using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.DTOs;
using TIKNA.Models;
using TIKNA.Data;

[Route("api/[controller]")]
[ApiController]
[Authorize] // تأمين الكنترولر بشكل عام
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // هذا الأكشن متاح فقط للمستخدم الذي يملك رول Student
    [Authorize(Roles = "Student")]
    [HttpGet("GetDashboardData")]
    public async Task<IActionResult> GetDashboardData()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null) return NotFound("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        // تجميع بيانات البروفايل الأساسية
        var profileInfo = new
        {
            user.Name,
            user.Email,
            user.Address,
            user.University,
            user.PhoneNumber,
            user.Faculty, // مضاف للطالب
            Role = "Student"
        };

        // جلب الطلبات وطلبات الصيانة الخاصة بهذا الطالب فقط
        var myOrders = await _context.Orders
            .Where(o => o.BuyerId == currentUserId)
            .ToListAsync();

        var myMaintenance = await _context.MaintenanceRequests
            .Where(m => m.UserId == currentUserId)
            .ToListAsync();

        return Ok(new
        {
            Info = profileInfo,
            Orders = myOrders,
            Maintenance = myMaintenance
        });
    }

    [HttpPost("UpdateProfile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null) return NotFound();

        user.Name = model.Name;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        user.University = model.University;
        user.Faculty = model.Faculty;

        await _userManager.UpdateAsync(user);
        return Ok(new { Message = "تم التحديث بنجاح" });
    }

    // [ب] تغيير كلمة المرور
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        if (model.NewPassword != model.ConfirmPassword) return BadRequest("كلمات المرور غير متطابقة");

        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded) return Ok(new { Message = "تم تغيير الباسورد" });
        return BadRequest(result.Errors);
    }

    // [ج] رفع الصورة الشخصية
    [HttpPost("UploadProfilePicture")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profiles");
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        user.ProfilePictureUrl = $"/profiles/{fileName}";
        await _userManager.UpdateAsync(user);

        return Ok(new { url = user.ProfilePictureUrl });





    }
}