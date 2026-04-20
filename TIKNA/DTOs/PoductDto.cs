using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models.Dtos
{
    public class ProductDto
    {
        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "الماركة مطلوبة")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "الموديل مطلوب")]
        public string Model { get; set; } // أضفنا الموديل هنا

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "يرجى تحديد فئة الجهاز")]
        public string Category { get; set; } // (Gaming, Business, etc)

        public string? Condition { get; set; } // (جديد، مستعمل، مجدد)

        // المواصفات الفنية (تطابق حقول الـ Model)
        public string? Processor { get; set; } // سيتم تخزينه في CPU
        public string? Ram { get; set; }
        public string? Storage { get; set; }
        public string? GraphicsCard { get; set; } // سيتم تخزينه في GPU
        public string? Screen { get; set; }        // سيتم تخزينه في ScreenSize
        public string? OS { get; set; }
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }

        // خيارات إضافية
        public bool ForSale { get; set; } = true;
        public bool ForRent { get; set; } = false;
        public decimal? RentalPricePerDay { get; set; }
    }
    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string? Condition { get; set; }

        // المواصفات التقنية
        public string? Processor { get; set; }
        public string? Ram { get; set; }
        public string? Storage { get; set; }
        public string? GraphicsCard { get; set; }
        public string? Screen { get; set; }
        public string? OS { get; set; }
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; } // اختياري: لو المستخدم مرفعش صورة جديدة نفضل على القديمة
        public bool IsActive { get; set; }

        public bool ForSale { get; set; }
        public bool ForRent { get; set; }
        public decimal? RentalPricePerDay { get; set; }
    }
}