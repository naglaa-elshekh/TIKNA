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

        // --- 1. إنشاء طلب صيانة (كما هو - يعمل بنجاح) ---
        [HttpPost("Create")]
        public async Task<IActionResult> CreateRequest([FromForm] MaintenanceRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "يجب تسجيل الدخول" });

            var hasPendingRequest = await _context.MaintenanceRequests
                .AnyAsync(r => r.UserId == userId && r.Brand == dto.Brand && r.Status == "Pending");

            if (hasPendingRequest) return BadRequest(new { message = "لديك طلب معلق بالفعل" });

            var orderNumber = "TIKNA-M-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            var request = new MaintenanceRequest
            {
                UserId = userId,
                CenterId = dto.CenterId.ToString(),
                OrderNumber = orderNumber,
                Brand = dto.Brand,
                ModelName = dto.ModelName,
                DeviceAge = dto.DeviceAge,
                ProblemType = dto.ProblemType,
                Description = dto.Description,
                ServiceType = dto.ServiceType,
                PreferredDate = dto.PreferredDate,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم الاستلام", orderNumber });
        }

        // --- 2. عرض طلبات العميل ---
        [HttpGet("MyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _context.MaintenanceRequests.Where(r => r.UserId == userId).ToListAsync());
        }

        // --- 3. تفاصيل الطلب ---
        [HttpGet("Details/{orderNumber}")]
        public async Task<IActionResult> GetRequestDetails(string orderNumber)
        {
            var request = await _context.MaintenanceRequests.FirstOrDefaultAsync(r => r.OrderNumber == orderNumber);
            return request == null ? NotFound() : Ok(request);
        }

        // --- 4. جلب طلبات المركز (لوحة تحكم الشركة) ---
        [HttpGet("CenterRequests")]
        public async Task<IActionResult> GetCenterRequests()
        {
            var centerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _context.MaintenanceRequests.Where(r => r.CenterId == centerId).ToListAsync());
        }

        // --- 5. تحديث السعر من المركز (الخطوة التي تسبق الدفع) ---
        [HttpPut("UpdatePrice/{orderNumber}")]
        public async Task<IActionResult> UpdatePrice(string orderNumber, [FromBody] MaintenanceUpdateDto updateDto)
        {
            var request = await _context.MaintenanceRequests.FirstOrDefaultAsync(r => r.OrderNumber == orderNumber);
            if (request == null) return NotFound();

            request.FinalPrice = updateDto.FinalPrice;
            request.Status = "AwaitingPayment"; // تغيير الحالة لتنبيه العميل بالدفع
            request.NoteFromCenter = updateDto.Note;
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم إرسال السعر للعميل" });
        }

        // --- 6. تأكيد عملية الدفع (تستدعى بعد نجاح الدفع في صفحتك الموحدة) ---
        [HttpPost("ConfirmPayment/{orderNumber}")]
        public async Task<IActionResult> ConfirmPayment(string orderNumber)
        {
            var request = await _context.MaintenanceRequests.FirstOrDefaultAsync(r => r.OrderNumber == orderNumber);
            if (request == null) return NotFound();

            request.Status = "Paid"; // تحويل الحالة لمدفوع لبدء الإصلاح
            request.IsPaid = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث حالة الطلب إلى مدفوع" });
        }
    }
}