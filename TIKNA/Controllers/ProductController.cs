//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;
//using TIKNA.Models;
//using Microsoft.AspNetCore.Identity;

//namespace Tikna.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        // 1. عرض كل المنتجات (متاح للجميع)
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
//        {
//            return await _context.Products
//                .Select(p => new {
//                    p.ProductId,
//                    p.Name,
//                    p.Price,
//                    p.ImageUrl,
//                    p.Category,
//                    p.Brand,
//                    p.IsActive
//                }).ToListAsync();
//        }

//        // 2. عرض تفاصيل منتج واحد
//        [AllowAnonymous]
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetProductById(int id)
//        {
//            var product = await _context.Products
//                .Include(p => p.Owner) // عشان لو عايزة تعرضي اسم البائع
//                .FirstOrDefaultAsync(p => p.ProductId == id);

//            if (product == null) return NotFound();

//            return Ok(new
//            {
//                productId = product.ProductId,
//                name = product.Name,
//                price = product.Price,
//                description = product.Description,
//                category = product.Category,
//                brand = product.Brand,
//                imageUrl = product.ImageUrl,
//                ownerName = product.Owner?.Name // اسم الشخص أو الشركة اللي عارضة
//            });
//        }

//        // 3. إضافة منتج (متاح لكل المستخدمين المسجلين)
//        [Authorize]
//        [HttpPost("AddProduct")]
//        public async Task<IActionResult> AddProduct([FromForm] ProductDto dto)
//        {
//            // أ- الحصول على الـ User الحالي
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var user = await _userManager.FindByIdAsync(userId);

//            // ب- البحث عن الـ Customer المرتبط باليوزر ده (عشان نجيب الـ CustomerId)
//            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

//            if (customer == null) return Unauthorized("لم يتم العثور على حساب عميل مرتبط.");

//            // ج- تطبيق شرط الـ Approval لو كان المستخدم "شركة"
//            if (user.UserType == "Company" && user.ApprovalStatus != "Approved") 
//            {
//                return BadRequest("عفواً، لا يمكنك إضافة منتجات قبل إتمام إجراءات التعاقد وموافقة الإدارة.");
//            }

//            // د- تجميع المواصفات الفنية من الـ DTO في نص واحد (لأن الـ Database مجمعة)
//            string formattedSpecs = $"Processor: {dto.Processor} | " +
//                                   $"RAM: {dto.Ram} | " +
//                                   $"Storage: {dto.Storage} | " +
//                                   $"Graphics: {dto.GraphicsCard} | " +
//                                   $"Screen: {dto.Screen} | " +
//                                   $"OS: {dto.OS}";

//            // هـ- معالجة رفع الصورة
//            string fileName = "default.png";
//            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
//            {
//                fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
//                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

//                using (var stream = new FileStream(path, FileMode.Create))
//                {
//                    await dto.ImageFile.CopyToAsync(stream);
//                }
//            }

//            // و- إنشاء المنتج وحفظه برقم الـ CustomerId
//            var product = new Product
//            {
//                Name = dto.Name,
//                Price = dto.Price,
//                Category = dto.Category,
//                Brand = dto.Brand,
//                Description = formattedSpecs,
//                ImageUrl = fileName,
//                CustomerId = customer.Id, // الربط الصحيح مع جدول الـ Customer
//                IsActive = dto.IsActive
//            };

//            _context.Products.Add(product);
//            await _context.SaveChangesAsync();

//            return Ok(new { message = "تم إضافة المنتج بنجاح", productId = product.ProductId });
//        }

//        // 4. حذف المنتج (لصاحب المنتج فقط)
//        [Authorize]
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteProduct(int id)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

//            var product = await _context.Products.FindAsync(id);

//            if (product == null) return NotFound("المنتج غير موجود");

//            // فحص الملكية: هل الـ CustomerId بتاع المنتج هو نفسه بتاع اليوزر اللي داخل؟
//            if (customer == null || product.CustomerId != customer.Id)
//                return Forbid("لا تملك صلاحية حذف هذا المنتج");

//            _context.Products.Remove(product);
//            await _context.SaveChangesAsync();
//            return Ok("تم الحذف بنجاح");
//        }
//        [Authorize]
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto dto)
//        {
//            // 1. التأكد من اليوزر والـ CustomerId
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

//            if (customer == null) return Forbid();

//            // 2. البحث عن المنتج
//            var productInDb = await _context.Products.FindAsync(id);
//            if (productInDb == null) return NotFound("المنتج غير موجود");

//            // 3. فحص الملكية (مهم جداً!)
//            if (productInDb.CustomerId != customer.Id)
//                return Forbid("لا تملك صلاحية تعديل هذا المنتج");

//            // 4. تحديث البيانات الأساسية
//            productInDb.Name = dto.Name;
//            productInDb.Price = dto.Price;
//            productInDb.Quantity = dto.Quantity;
//            productInDb.Brand = dto.Brand;
//            productInDb.Category = dto.Category;
//            productInDb.IsActive = dto.IsActive;

//            // 5. إعادة تجميع المواصفات الفنية في الـ Description
//            productInDb.Description = $"Processor: {dto.Processor} | " +
//                                       $"RAM: {dto.Ram} | " +
//                                       $"Storage: {dto.Storage} | " +
//                                       $"Graphics: {dto.GraphicsCard} | " +
//                                       $"Screen: {dto.Screen} | " +
//                                       $"OS: {dto.OS}";

//            // 6. معالجة الصورة (لو اليوزر رفع صورة جديدة بس)
//            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
//            {
//                // مسح الصورة القديمة (اختياري لتوظيف المساحة)
//                // توليد اسم جديد وحفظه
//                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
//                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);
//                using (var stream = new FileStream(path, FileMode.Create))
//                {
//                    await dto.ImageFile.CopyToAsync(stream);
//                }
//                productInDb.ImageUrl = fileName;
//            }

//            await _context.SaveChangesAsync();
//            return Ok(new { message = "تم تحديث بيانات المنتج ومواصفاته بنجاح" });
//        }
//    }

//}