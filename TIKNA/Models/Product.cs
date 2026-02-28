using TIKNA.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    // سعر المنتج، Decimal لازم تحددي Precision إذا حابة في DB
    public decimal Price { get; set; }

    // FK إلى Owner (Customer)
    public int CustomerId { get; set; }
    public Customer Owner { get; set; }

    // علاقة Many-to-Many مع Orders
    public ICollection<OrderProd> OrderProducts { get; set; }

    // علاقة One-to-Many مع Rentals
    public ICollection<Rental> Rentals { get; set; }

    // علاقة One-to-Many مع MaintenanceRequests
    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
}
