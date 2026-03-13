using System.ComponentModel.DataAnnotations;

public class ProductCreateDto
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(100)]
    public string Name { get; set; }
    public string Description { get; set; }
    [Range(0.1, 100000, ErrorMessage = "السعر لازم يكون بين 0.1 و 100,000")]


    public decimal Price { get; set; }
    public IFormFile? ImageFile { get; set; }
}
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public decimal Price { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string OwnerName { get; set; } // اسم المالك بس بدل الكائن كله
}

public class ProductUpdateDto
{
    // بنحط بس الحقول اللي "المنطقي" إنها تتغير
    [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(100)]
    public string Name { get; set; }
    public string Description { get; set; }
    [Range(0.1, 100000, ErrorMessage = "السعر لازم يكون بين 0.1 و 100,000")]
    public decimal Price { get; set; }
    //public string ImageUrl { get; set; }
    //public int CategoryId { get; set; }
}
