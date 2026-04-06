using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Brand { get; set; }

    // فئة اللاب (نوع الاستخدام)
    [Required(ErrorMessage = "يرجى تحديد فئة اللاب (Gaming, Business, etc)")]
    public string Category { get; set; }

    // المواصفات الفنية
    public string Processor { get; set; }
    public string Ram { get; set; }
    public string Storage { get; set; }
    public string GraphicsCard { get; set; }
    public string Screen { get; set; }
    public string OS { get; set; }

    public IFormFile? ImageFile { get; set; }
    public bool IsActive { get; set; } = true;
}

    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }

        // المواصفات الفنية (عشان نحدث الـ Description)
        public string Processor { get; set; }
        public string Ram { get; set; }
        public string Storage { get; set; }
        public string GraphicsCard { get; set; }
        public string Screen { get; set; }
        public string OS { get; set; }

        public IFormFile? ImageFile { get; set; } // اختيارية في التعديل
        public bool IsActive { get; set; }
    }