using TIKNA.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } // هنا بنجمع المواصفات الفنية
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }

    // الخانات اللي ناقصاكي عشان الـ UI (الصورة)
    public string? Brand { get; set; }
    public string? Category { get; set; } // نوع اللاب (Gaming, Student...)
    public bool IsActive { get; set; } = true;

    // الربط مع صاحب المنتج (سواء كان طالب أو شركة)
    public int CustomerId { get; set; }
    public Customer Owner { get; set; }

    public ICollection<OrderProd> OrderProducts { get; set; }
    public ICollection<Rental> Rentals { get; set; }
    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
}