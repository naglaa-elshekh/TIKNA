using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models; // تأكدي إن المسار ده صح حسب مشروعك
using TIKNA.Models;

namespace Tikna.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. عرض كل المنتجات (متاح للجميع)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // 2. إضافة منتج جديد (للـ Customer فقط)
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // سحب ID المستخدم اللي عامل Login حالياً
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("You must Login first");

            product.CustomerId = int.Parse(userIdClaim);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducts), new { id = product.ProductId }, product);
        }

        // 3. تعديل المنتج (لصاحب المنتج فقط)
        [Authorize(Roles = "Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);

            if (existingProduct == null) return NotFound();

            // التأكد إن اللي بيعدل هو صاحب المنتج
            if (existingProduct.CustomerId != userId)
                return Forbid("You don't have permission to modify this product");

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Modified successfully");
        }

        // 4. حذف المنتج (لصاحب المنتج فقط)
        [Authorize(Roles = "Customer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            // التأكد إن اللي بيحذف هو صاحب المنتج
            if (product.CustomerId != userId)
                return Forbid("You don't have permission to delete this product");
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}