using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TIKNA.Data;
using TIKNA.Models;

namespace TIKNA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // تأكدي من تعيين دور Admin للوسيط
    public class BrokerAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BrokerAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetDashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            // 1. عدد الشركات
            var totalCompanies = await _context.Users.CountAsync(u => u.UserType == "Company");

            // 2. المنتجات المعلقة
            var pendingProducts = await _context.Products.CountAsync(p => p.IsActive == false);

    // 3. عدد طلبات الصيانة المعلقة
    var pendingMaintenance = await _context.MaintenanceRequests.CountAsync(m => m.Status == "Pending");

    // 4. إجمالي المبيعات (Revenue)
    // بنجمع (الكمية * السعر) من جدول OrderProd
    var totalRevenue = await _context.OrderProducts.SumAsync(op => op.Quantity * op.UnitPrice);
    return Ok(new
    {
        TotalCompanies = totalCompanies,
        PendingProducts = pendingProducts,
        PendingMaintenance = pendingMaintenance,
        TotalRevenue = totalRevenue
    });
        }

        // 2. إدارة المنتجات المعلقة (الموافقة أو الرفض)
        [HttpGet("GetPendingProducts")]
        public async Task<IActionResult> GetPendingProducts()
        {
            var pending = await _context.Products
                .Include(p => p.Owner)
                .Where(p => !p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return Ok(pending);
        }

        [HttpPatch("ApproveProduct/{id}")]
        public async Task<IActionResult> ApproveProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = true; // تفعيل المنتج ليظهر في المتجر
            await _context.SaveChangesAsync();
            return Ok(new { message = "تمت الموافقة على المنتج بنجاح" });
        }

        // 3. رقابة على طلبات الصيانة
        [HttpGet("GetAllMaintenanceRequests")]
        public async Task<IActionResult> GetAllMaintenance()
        {
            var requests = await _context.MaintenanceRequests
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new {
                    r.OrderNumber,
                    r.Brand,
                    r.ProblemType,
                    r.Status,
                    r.FinalPrice,
                    r.IsPaid
                })
                .ToListAsync();
            return Ok(requests);
        }

        // 4. عرض كافة الطلبات (Orders) في النظام
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Buyer)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new {
                    o.OrderId,
                    BuyerName = o.Buyer.Name,
                    o.TotalPrice,
                    o.Status,
                    o.OrderDate
                })
                .ToListAsync();
            return Ok(orders);
        }
    }
}