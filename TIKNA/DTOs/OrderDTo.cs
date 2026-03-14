public class OrderCreateDTO
{
    public int CustomerId { get; set; }
    // قائمة بمعرفات المنتجات التي اختارها الطالب من السلة
    public List<int> ProductIds { get; set; }
}

public class OrderReadDTO
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }

    // قائمة بأسماء المنتجات وصورها عشان تتعرض في الفرونت إند
    public List<ProductSummaryDTO> Products { get; set; }
}

public class ProductSummaryDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
}
public class OrderUpdateStatusDTO
{
    public string NewStatus { get; set; }
}