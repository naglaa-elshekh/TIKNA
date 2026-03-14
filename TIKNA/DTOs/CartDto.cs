// للإضافة للسلة
public class AddToCartDTO
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public int Quantity { get; set; } = 1;
}

// لعرض السلة
public class CartItemReadDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
}


public class UpdateQuantityDTO
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public int NewQuantity { get; set; }
}