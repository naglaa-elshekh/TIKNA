using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIKNA.Models;
using TIKNA.Data;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // 2. عمل الـ Constructor عشان الـ Program.cs يبعت الداتابيز هنا
        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(int orderId)
        {
            // 1. التأكد من وجود الأوردر
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound("الأوردر غير موجود");

            // 2. محاكاة نجاح الدفع (Success)
            order.Status = "Paid"; // تغيير حالة الأوردر لمدفوع

            // 3. تسجيل العملية في جدول الـ Payment للمصداقية
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = order.TotalPrice,


                PaymentDate = DateTime.Now,
                PaymentType = "CreditCard",
                Status = "Completed"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم الدفع بنجاح! شكراً لثقتك في تيكنا." });
        }
    } }


