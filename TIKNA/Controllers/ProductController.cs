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

        // 5. عرض المنتجات مع جلب اسم المالك
        [HttpGet("GetProducts") ]
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
                    p.ImageUrl,
                    OwnerName = p.Owner.Name,
                    p.Category
                })
                .ToListAsync();

            return Ok(products);
        }


        // 6. جلب تفاصيل منتج واحد بناءً على الـ ID
        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = "الجهاز غير موجود" });

            // إرجاع كل التفاصيل التي تحتاجها صفحة العرض
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
                product.Description,
                product.CPU,
                product.RAM,
                product.Storage,
                product.GPU,
                product.ScreenSize,
                product.Color,
                product.ImageUrl,
                product.ForSale,
                product.ForRent,
                product.RentalPricePerDay,
                OwnerName = product.Owner?.Name,
                OwnerPhone = product.Owner?.PhoneNumber
            });
        }
    }
}