using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models;
using TIKNA.Data;


namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // تأمين كل الميثودز بالتوكن
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. إنشاء أوردر جديد من السلة (Checkout)
        [HttpPost("Checkout")]
        public async Task<ActionResult> CreateOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // جلب السلة بمنتجاتها
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return BadRequest("عفواً، السلة فارغة لا يمكن إتمام الطلب.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // حساب الإجمالي من أسعار المنتجات الحالية
                decimal total = cart.CartItems.Sum(item => item.Product.Price * item.Quantity);

                // إنشاء سجل الأوردر
                var order = new Order
                {
                    BuyerId = userId,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalPrice = total
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // نقل المنتجات لجدول الربط (OrderProd)
                foreach (var item in cart.CartItems)
                {
                    var orderProd = new OrderProd
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        // Quantity = item.Quantity // ضيفيها لو الجدول عندك بيدعم كمية لكل منتج
                    };
                    _context.OrderProducts.Add(orderProd);
                }

                // تصفير السلة بعد نجاح العملية
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "تم تسجيل الطلب بنجاح وتفريغ السلة",
                    orderId = order.OrderId,
                    totalAmount = total
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"حدث خطأ: {ex.Message}");
            }
        }

        // 2. جلب أوردر محدد بالتفاصيل
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.BuyerId == userId);

            if (order == null) return NotFound("الأوردر غير موجود أو لا تملك صلاحية الوصول إليه.");

            return Ok(order);
        }

        // 3. جلب كل أوردرات اليوزر الحالي
        [HttpGet("MyOrders")]
        public async Task<ActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(o => o.BuyerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.TotalPrice,
                    o.Status,
                    ItemCount = o.OrderProducts.Count()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // 4. تحديث حالة الأوردر (للأدمن مثلاً)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("الأوردر غير موجود");

            order.Status = newStatus;
            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث حالة الطلب بنجاح" });
        }

        // 5. حذف أوردر (Cancel Order)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.BuyerId == userId);

            if (order == null) return NotFound();

            _context.OrderProducts.RemoveRange(order.OrderProducts);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok("تم إلغاء الطلب بنجاح.");
        }
    }
}