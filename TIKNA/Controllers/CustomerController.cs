using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // --- قسم الـ Admin: عرض كل العملاء (الطلبة فقط) ---
    [HttpGet("Admin/All")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(string? search)
    {
        // بنعمل Include للـ User عشان نجيب الإيميل والبيانات الأساسية
        var query = _context.Customers.Include(c => c.ApplicationUser).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Name.Contains(search) || c.Phone.Contains(search));

        return Ok(await query.ToListAsync());
    }

    // --- قسم الـ Profile: متاح للأدمن وللطالب ---
    [HttpGet("MyProfile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return NotFound();

        // لو طالب، هنجيب بياناته من جدول الـ Customer
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        // بنرجع كائن فيه بيانات الـ Identity + بيانات الـ Customer لو وجدت
        return Ok(new
        {
            Email = user.Email,
            Name = customer?.Name ?? "Admin User", // لو أدمن ملوش سطر في Customer هنعرض اسم افتراضي
            Phone = customer?.Phone ?? user.PhoneNumber,
            Address = customer?.Address ?? "N/A",
            IsAdmin = User.IsInRole("Admin")
        });
    }

    [HttpPut("UpdateMyProfile")]
    public async Task<IActionResult> Update(CustomerDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. تحديث بيانات الـ Identity (مهمة للأدمن والطالب)
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // 2. تحديث بيانات الـ Customer (للطالب فقط)
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (customer != null)
        {
            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;
            _context.Customers.Update(customer);
        }
        else if (User.IsInRole("Admin"))
        {
            // هنا لو أدمن وعايزة تسمحي له يعدل بياناته هو شخصياً (ممكن تضيفي خانات للـ ApplicationUser)
            user.PhoneNumber = dto.Phone;
            await _userManager.UpdateAsync(user);
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "تم تحديث البيانات بنجاح" });
    }
}