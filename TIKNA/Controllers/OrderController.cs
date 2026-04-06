
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context) => _context = context;



   
        [Authorize]
        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderCreateDTO dto)
        {
            // 1. إنشاء الأوردر الأساسي
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = 0 // سنحسبه في الخطوة القادمة
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // حفظنا الأوردر عشان ناخد الـ ID بتاعه

            decimal total = 0;

            // 2. إضافة المنتجات في جدول OrderProd
            foreach (var pId in dto.ProductIds)
            {
                var product = await _context.Products.FindAsync(pId);
                if (product != null)
                {
                    var orderProd = new OrderProd
                    {
                        OrderId = order.OrderId,
                        ProductId = pId,
                        // Price = product.Price // يفضل تسجيل السعر وقت الشراء
                    };
                    _context.OrderProducts.Add(orderProd);
                    total += product.Price;
                }
            }

            // 3. تحديث إجمالي السعر في الأوردر
            order.TotalPrice = total;
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تسجيل الطلب بنجاح", orderId = order.OrderId });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts) // ادخل لجدول الربط
                    .ThenInclude(op => op.Product) // ومنه هات بيانات المنتج
                .FirstOrDefaultAsync(o => o.OrderId== id);

            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Select(o => new OrderReadDTO
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.Name,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    Products = o.OrderProducts.Select(op => new ProductSummaryDTO
                    {
                        Name = op.Product.Name,
                        Price = op.Product.Price,
                        ImageUrl = op.Product.ImageUrl
                    }).ToList()
                }).ToListAsync();

            return Ok(orders);
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderUpdateStatusDTO statusDto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("الأوردر غير موجود");

            // نحدث فقط الحقل المطلوب
            order.Status = statusDto.NewStatus;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم تحديث الحالة بنجاح" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            // مسح روابط المنتجات أولاً
            _context.OrderProducts.RemoveRange(order.OrderProducts);

            // مسح الأوردر
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return Ok("تم حذف الأوردر وبياناته");
        }

    }
}
