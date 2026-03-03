using TIKNA.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }
    public int CustomerId { get; set; }
    public Customer Owner { get; set; }

    public ICollection<OrderProd> OrderProducts { get; set; }

    public ICollection<Rental> Rentals { get; set; }

    public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
}
