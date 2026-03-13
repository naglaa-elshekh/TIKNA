using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.DTOs;
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
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            // 1. نجلب المنتج مع المالك
            var product = await _context.Products
                .Include(p => p.Owner) // أو p.Owner حسب التسمية عندك
                .FirstOrDefaultAsync(p => p.ProductId == id);

            // 2. لو مش موجود نرجع NotFound حقيقية
            if (product == null) return NotFound();

            // 3. الحل السحري: نرجع "بيانات محددة" فقط (Anonymous Object) 
            // ده بيمنع الـ JSON من إنه يدخل في دوامة الـ Products اللي جوه المالك
            return Ok(new
            {
                productId = product.ProductId,
                name = product.Name,
                price = product.Price,
                description = product.Description,
                //imageUrl = product.ImageUrl,
                // بنأخد اسم المالك بس، وبكده كسرنا الـ Cycle
                ownerName = product.Owner?.Name ?? "Unknown"
            });
        }

        [Authorize]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateDto dto)
        {
            // 1. التأكد من اليوزر (نفس الـ Logic اللي ظبطناه)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null) return Forbid();

            // 2. معالجة رفع الصورة
            string fileName = null;
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // توليد اسم فريد
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);

                // المسار في wwwroot/Images
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                // حفظ الملف
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
            }

            // 3. إنشاء المنتج وربطه بالصورة والعميل
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = fileName, // اسم الصورة اللي اتسيف
                CustomerId = customer.Id
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم إضافة المنتج بنجاح", productId = product.ProductId });
        }
        // 3. تعديل المنتج (لصاحب المنتج فقط)
        [Authorize]
        [HttpPut("{id}")] // الـ id هنا هو رقم المنتج
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            // 1. الوصول للمستخدم الحالي (الكوبري)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null) return Forbid();

            // 2. البحث عن المنتج الأصلي من الداتابيز
            var productInDb = await _context.Products.FindAsync(id);

            if (productInDb == null) return NotFound("المنتج غير موجود");

            // 3. اختبار الملكية (مقارنة ID العميل بـ CustomerId المتسيف في المنتج)
            if (productInDb.CustomerId != customer.Id)
            {
                return Forbid("لا تملك صلاحية تعديل هذا المنتج");
            }

            // 4. تحديث البيانات من الـ DTO للمنتج الأصلي (Mapping)
            productInDb.Name = dto.Name;
            productInDb.Description = dto.Description;
            productInDb.Price = dto.Price;
            //productInDb.ImageUrl = dto.ImageUrl;
            //productInDb.CategoryId = dto.CategoryId;

            // 5. حفظ التعديلات
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم التحديث بنجاح" });
        }

        // 4. حذف المنتج (لصاحب المنتج فقط)
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // 1. استخراج الـ GUID من التوكن
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. البحث عن العميل المرتبط بالـ GUID ده
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userIdFromToken);

            if (customer == null)
                return BadRequest($"مشكلة: مفيش سطر في جدول العميل مرتبط بالـ GUID ده: {userIdFromToken}");

            // 3. البحث عن المنتج
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("المنتج مش موجود أصلاً");

            // 4. الفحص النهائي (هنا هنعرف العيب فين)
            if (product.CustomerId != customer.Id)
            {
                return Forbid($"ممنوع! المنتج ده ملك العميل رقم ({product.CustomerId})، وأنت داخل بحساب العميل رقم ({customer.Id})");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("تم الحذف بنجاح");
        }


    }
}