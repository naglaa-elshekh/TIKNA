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
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            // جلب الـ ID بتاع اليوزر اللي عامل Login حالياً من التوكن
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

     if (string.IsNullOrEmpty(userId)) return Unauthorized("يجب تسجيل الدخول أولاً"); 

     var cart = await _context.Carts
         .Include(c => c.CartItems)
         .ThenInclude(ci => ci.Product)
         .FirstOrDefaultAsync(c => c.UserId == userId); 

     if (cart == null || !cart.CartItems.Any())
                return BadRequest("عفواً، السلة فارغة لا يمكن إتمام الطلب."); 

     using var transaction = await _context.Database.BeginTransactionAsync(); 
     try
            {
                // حساب الإجمالي
                decimal total = cart.CartItems.Sum(item => item.Product.Price * item.Quantity); 

         var order = new Order
         {
             BuyerId = userId, // المسمى الصح في الموديل بتاعك هو BuyerId
             OrderDate = DateTime.Now,
             
             TotalPrice = total, // المسمى الصح في الموديل بتاعك هو TotalPrice
             Status = "Pending"
         };

                _context.Orders.Add(order); 
         await _context.SaveChangesAsync(); // هنا الـ OrderId بيتولد تلقائياً

                // نقل المنتجات لجدول OrderProd وتحديث المخزن
                foreach (var item in cart.CartItems)
                {
                    var orderProd = new OrderProd
                    {
                        OrderId = order.OrderId,
       
                        ProductId = item.ProductId,
                        
       
                        Quantity = item.Quantity,
                       
       
                        UnitPrice = item.Product.Price // تخزين السعر وقت الشراء
                    };
                    _context.OrderProducts.Add(orderProd); 

             // تحديث الكمية في المخزن (مهم جداً)
             item.Product.Quantity -= item.Quantity; 
         }

                // تصفير السلة
                _context.CartItems.RemoveRange(cart.CartItems); 

         await _context.SaveChangesAsync();
         await transaction.CommitAsync(); 

         return Ok(new { OrderId = order.OrderId, Message = "تمت العملية بنجاح وتفريغ السلة" }); 
     }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); 
         return StatusCode(500, $"حدث خطأ أثناء إتمام الطلب: {ex.Message}"); 
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


        // 6. جلب الطلبات الواردة للشركة (التي تحتوي على منتجاتها)
        [HttpGet("IncomingOrders")]
        public async Task<ActionResult> GetIncomingOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // جلب الطلبات التي تحتوي على منتجات تابعة لهذا المستخدم (الشركة)
            var incomingOrders = await _context.OrderProducts
                .Where(op => op.Product.ApplicationUserId == userId)
                .OrderByDescending(op => op.Order.OrderDate)
                .Select(op => new
                {
                    OrderId = op.OrderId,
                    ProductName = op.Product.Name,
                    BuyerName = op.Order.Buyer.Name, // تأكدي من وجود خاصية FullName في الـ User
                    OrderDate = op.Order.OrderDate,
                    Price = op.Product.Price,
                    Status = op.Order.Status
                })
                .ToListAsync();

            return Ok(incomingOrders);
        }
    }
}