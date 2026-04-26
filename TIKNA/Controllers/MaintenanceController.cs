using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIKNA.Models;
using System.Security.Claims;
using TIKNA.DTOs;
using TIKNA.Data;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. إنشاء طلب صيانة جديد (المتوافق مع الفرونت إند)
        [HttpPost("Create")]
        public async Task<IActionResult> CreateRequest([FromForm] MaintenanceRequestDto dto)
        {
            // الحصول على معرف المستخدم الحالي
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });
            }

            // بيزنس لوجيك: منع تكرار الطلبات لنفس الجهاز وهي لسه تحت المراجعة
            var hasPendingRequest = await _context.MaintenanceRequests
                .AnyAsync(r => r.UserId == userId
                   && r.Brand == dto.Brand
                   && r.ModelName == dto.ModelName
                   && r.Status == MaintenanceStatus.Pending.ToString());

            if (hasPendingRequest)
            {
                return BadRequest(new { message = "لديك طلب صيانة معلق بالفعل لهذا النوع من الأجهزة" });
            }

            // توليد رقم طلب فريد
            var orderNumber = "TIKNA-M-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // حساب التكلفة التقديرية (بناءً على نوع المشكلة المختارة في الفرونت)
            decimal minPrice = 150;
            decimal maxPrice = 500;

            if (dto.ProblemType.Contains("Hardware")) { minPrice = 300; maxPrice = 1500; }

            var request = new MaintenanceRequest
            {
                UserId = userId,
                OrderNumber = orderNumber,
                Brand = dto.Brand,
                ModelName = dto.ModelName,
                DeviceAge = dto.DeviceAge,
                ProblemType = dto.ProblemType,
                Description = dto.Description,
                ServiceType = dto.ServiceType,
                PreferredDate = dto.PreferredDate,
                PreferredTimeSlot = dto.PreferredTimeSlot,
                EstimatedPriceMin = minPrice,
                EstimatedPriceMax = maxPrice,
                Status = MaintenanceStatus.Pending.ToString(),
                CreatedAt = DateTime.Now
            };

            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم استلام طلبك بنجاح",
                orderNumber = orderNumber,
                estimatedPrice = $"{minPrice} - {maxPrice} EGP"
            });
        }

        // 2. عرض طلبات الصيانة الخاصة بالطالب الحالي
        [HttpGet("MyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = await _context.MaintenanceRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(requests);
        }

        // 3. عرض تفاصيل طلب معين برقم الطلب
        [HttpGet("Details/{orderNumber}")]
        public async Task<IActionResult> GetRequestDetails(string orderNumber)
        {
            var request = await _context.MaintenanceRequests
                .FirstOrDefaultAsync(r => r.OrderNumber == orderNumber);

            if (request == null) return NotFound(new { message = "الطلب غير موجود" });

            return Ok(request);
        }
    }
}