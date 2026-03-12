using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.DTOs;
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
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(OrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null) return BadRequest("Customer not found.");

            // 1. إنشاء الأوردر الأساسي
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.Now,
                OrderProducts = new List<OrderProd>() // نجهز القائمة
            };

            // 2. إضافة المنتجات للأوردر
            foreach (var item in dto.Items)
            {
                order.OrderProducts.Add(new OrderProd
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    // لو عندك PriceAtPurchase في جدول OrderProd يفضل تخزنيه هنا عشان السعر لو اتغير مستقبلاً
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Order Placed!", OrderId = order.OrderId });
        }
    }
}
