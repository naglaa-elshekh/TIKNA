using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TIKNA.Models;
using TIKNA.Models.Dtos;// تأكدي من مسار الـ DTOs
using Microsoft.AspNetCore.Identity;
using TIKNA.Data;


namespace Tikna.Controllers
{
    [Route("api/[controller]")] // هذا يجعل المسار الأساسي api/Product
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

        [Authorize]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto dto)
        {
            // 1. الحصول على UserId من الـ Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

            // 2. معالجة رفع الصورة (إذا وجدت)
            string fileName = "default_product.png";
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fullPath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
            }

            // 3. إنشاء كائن المنتج وربط حقول الـ DTO بحقول الـ Model
            var product = new Product
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Model = dto.Model,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Category = dto.Category,
                Condition = dto.Condition,
                Description = dto.Description,

                // ربط المواصفات التقنية بالحقول المستقلة في الداتابيز
                CPU = dto.Processor,
                RAM = dto.Ram,
                Storage = dto.Storage,
                GPU = dto.GraphicsCard,
                ScreenSize = dto.Screen,
                Color = dto.OS, // أو يمكنك إضافة حقل OS للموديل لو لزم الأمر

                // حالات البيع والإيجار
                ForSale = dto.ForSale,
                ForRent = dto.ForRent,
                RentalPricePerDay = dto.RentalPricePerDay,

                ImageUrl = fileName,
                ApplicationUserId = userId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            // 4. الحفظ في قاعدة البيانات
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Ok(new { message = "تم إضافة الجهاز بنجاح", productId = product.ProductId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "حدث خطأ أثناء الحفظ", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetProducts")]

        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Owner)
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.ProductId,
                    p.Name,
                    p.Brand,
                    p.Model,
                    p.Price,
                    p.Quantity, // <--- ضيفي السطر ده هنا فوراً!
                    p.ImageUrl,
                    p.Category,
                    p.ForSale,
                    p.ForRent,
                    p.RentalPricePerDay,
                    OwnerName = p.Owner.Name ?? "مستخدم تيكنا"
                })
                .ToListAsync();

            return Ok(products);
        }

        // 3. جلب تفاصيل منتج واحد (محسن)
        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = "الجهاز غير موجود" });

            return Ok(new
            {
                product.ProductId,
                product.Name,
                product.Brand,
                product.Model,
                product.Price,
                product.Quantity,
                product.Category,
                product.Condition,
                CPU = product.CPU,
                RAM = product.RAM,
                Storage = product.Storage,
                GPU = product.GPU,
                product.ScreenSize,
                product.Color,
                product.ImageUrl,
                product.ForSale,
                product.ForRent,
                product.RentalPricePerDay,
                OwnerName = product.Owner?.Name ?? "غير معروف",
                OwnerPhone = product.Owner?.PhoneNumber ?? "غير متوفر"
            });

        }



        [Authorize]
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto dto)
        {
            // 1. الحصول على UserId للمستخدم الحالي
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. البحث عن المنتج والتأكد إنه يخص المستخدم الحالي
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id && p.ApplicationUserId == userId);

            if (product == null)
                return NotFound(new { message = "الجهاز غير موجود أو ليس لديك صلاحية تعديله" });

            // 3. تحديث البيانات الأساسية
            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Model = dto.Model;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.Category = dto.Category;
            product.Condition = dto.Condition;
            product.Description = dto.Description;
            product.CPU = dto.Processor;
            product.RAM = dto.Ram;
            product.Storage = dto.Storage;
            product.GPU = dto.GraphicsCard;
            product.ScreenSize = dto.Screen;
            product.ForSale = dto.ForSale;
            product.ForRent = dto.ForRent;
            product.RentalPricePerDay = dto.RentalPricePerDay;

            // 4. معالجة الصورة الجديدة (لو رفع صورة جديدة)
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // مسح الصورة القديمة لو مش هي الصورة الافتراضية
                if (product.ImageUrl != "default_product.png")
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", product.ImageUrl);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = fileName;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "تم تحديث بيانات الجهاز بنجاح" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "حدث خطأ أثناء التعديل", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetMyProducts")]
        public async Task<IActionResult> GetMyProducts()
        {
            // 1. الحصول على الـ ID الخاص بالمستخدم الحالي من التوكن
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "يجب تسجيل الدخول أولاً" });

            // 2. جلب المنتجات التي يملكها هذا المستخدم فقط
            var myProducts = await _context.Products
                .Where(p => p.ApplicationUserId == userId) // الفلترة هنا هي السر
                .OrderByDescending(p => p.CreatedAt) // عرض الأحدث أولاً
                .Select(p => new {
                    p.ProductId,
                    p.Name,
                    p.Brand,
                    p.Model,
                    p.Price,
                    p.Quantity,
                    p.ImageUrl,
                    p.Category,
                    p.IsActive, // عشان تعرفي الجهاز متاح ولا لأ
                    p.ForSale,
                    p.ForRent,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(myProducts);
        }
       
        [Authorize]
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id && p.ApplicationUserId == userId);

            if (product == null) return NotFound();

            // بدلاً من Remove، نغير الحالة فقط
            product.IsActive = false;

            await _context.SaveChangesAsync();
            return Ok(new { message = "تم إخفاء المنتج بنجاح" });
        }
    }
}