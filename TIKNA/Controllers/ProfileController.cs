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

    [Authorize(Roles = "Student")]
    [HttpGet("GetDashboardData")]
    public async Task<IActionResult> GetDashboardData()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(currentUserId);

        if (user == null) return NotFound("User not found");

        var profileInfo = new
        {
            user.Name,
            user.Email,
            user.Address,
            user.University,
            user.PhoneNumber,
            user.Faculty,
            user.ProfilePictureUrl,
            Role = "Student"
        };

        // ✅ الحل: اختيار البيانات المطلوبة فقط (Select) لمنع الانهيار 500
        var myOrders = await _context.Orders
            .Where(o => o.BuyerId == currentUserId)
            .Select(o => new {
                o.OrderId,
                o.TotalPrice,
                o.Status,
                OrderDate = o.OrderDate.ToString("yyyy-MM-dd")
            })
            .ToListAsync();

        var myMaintenance = await _context.MaintenanceRequests
            .Where(m => m.UserId == currentUserId)
            .Select(m => new {
                m.Id,
                m.ModelName, // تأكدي من مسمى الحقل في الـ Model
                m.Center, // تأكدي من مسمى الحقل في الـ Model
                m.Status,
                m.FinalPrice,
                CreatedAt = m.CreatedAt.ToString("yyyy-MM-dd")
            })
            .ToListAsync();

        return Ok(new
        {
            Info = profileInfo,
            Orders = myOrders,
            Maintenance = myMaintenance
        });
    }
    // 2. تحديث البيانات (الاسم، الهاتف، العنوان، الجامعة)
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

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) return Ok(new { Message = "تم التحديث بنجاح" });

        return BadRequest(result.Errors);
    }

    // 3. تغيير كلمة المرور
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        if (model.NewPassword != model.ConfirmPassword) return BadRequest("كلمات المرور غير متطابقة");

        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (result.Succeeded) return Ok(new { Message = "تم تغيير الباسورد بنجاح" });
        return BadRequest(result.Errors);
    }

    // 4. رفع الصورة الشخصية (تم تغيير البارامتر إلى file)
    [HttpPost("UploadProfilePicture")]
    public async Task<IActionResult> UploadImage(IFormFile file) // التغيير هنا ليتوافق مع JS
    {
        if (file == null || file.Length == 0) return BadRequest("لم يتم استلام ملف.");

        // تحديد مسار المجلد داخل wwwroot
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profiles");

        // إنشاء المجلد إذا لم يكن موجوداً
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // إنشاء اسم فريد للملف لمنع التكرار
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(folderPath, fileName);

        // حفظ الملف في السيرفر
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // تحديث رابط الصورة في قاعدة البيانات للمستخدم الحالي
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        user.ProfilePictureUrl = $"/profiles/{fileName}";

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { url = user.ProfilePictureUrl });
        }

        return BadRequest("حدث خطأ أثناء تحديث بيانات الصورة.");
    }
}