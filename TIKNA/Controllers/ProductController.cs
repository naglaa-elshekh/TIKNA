using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models;
using Microsoft.AspNetCore.Identity;

namespace Tikna.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. عرض كل المنتجات (متاح للجميع)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            return await _context.Products
                .Select(p => new {
                    p.ProductId,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.Category,
                    p.Brand,
                    p.IsActive,
                    ownerName = p.Owner.Name // بنجيب اسم المالك علطول
                }).ToListAsync();
        }

        // 2. عرض تفاصيل منتج واحد
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound(new { message = "المنتج غير موجود" });

            return Ok(new
            {
                productId = product.ProductId,
                name = product.Name,
                price = product.Price,
                description = product.Description,
                category = product.Category,
                brand = product.Brand,
                imageUrl = product.ImageUrl,
                ownerName = product.Owner?.Name,
                ownerEmail = product.Owner?.Email
            });
        }

        // 3. إضافة منتج (مع شرط الـ Approval للشركات)
        [Authorize]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto dto)
        {
            // أ- الحصول على الـ User الحالي
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return Unauthorized();

            // ب- فحص حالة الشركة (اللي اتكلمنا عليه يا لييدر)
            if (user.UserType == "Company" && user.ApprovalStatus != "Approved")
            {
                return BadRequest(new { message = "عفواً، لا يمكنك إضافة منتجات قبل موافقة الإدارة على حسابك." });
            }

            // جـ- معالجة رفع الصورة
            string fileName = "default.png";
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                // تأكدي إن الفولدر موجود
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images")))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images"));

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
            }

            // د- تجميع المواصفات (Specs)
            string formattedSpecs = $"RAM: {dto.Ram} | Storage: {dto.Storage} | Processor: {dto.Processor} | OS: {dto.OS}";

            // هـ- إنشاء المنتج
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Category = dto.Category,
                Brand = dto.Brand,
                ImageUrl = fileName,
                ApplicationUserId = userId, // الربط المباشر باليوزر
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إضافة المنتج بنجاح", productId = product.ProductId });
        }

        // 4. حذف المنتج (فحص الملكية بالـ UserId)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound("المنتج غير موجود");

            // فحص الملكية: هل اليوزر اللي داخل هو صاحب المنتج؟
            if (product.ApplicationUserId != userId)
                return Forbid();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المنتج بنجاح" });
        }

        // 5. تعديل منتج
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productInDb = await _context.Products.FindAsync(id);

            if (productInDb == null) return NotFound("المنتج غير موجود");
            if (productInDb.ApplicationUserId != userId) return Forbid();

            productInDb.Name = dto.Name;
            productInDb.Price = dto.Price;
            productInDb.Brand = dto.Brand;
            productInDb.Category = dto.Category;

            if (dto.ImageFile != null)
            {
                // كود تغيير الصورة هنا
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم التعديل بنجاح" });
        }
    }
}