// 1. للإضافة للسلة أو المزامنة (Sync)

public class CartItemReadDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; } // الحقل الجديد للكمية المتاحة
}

public class CartItemUpdateDTO


{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

// 2. لعرض السلة (اللي هيروح للـ JavaScript عشان يرسم الصفحة)
