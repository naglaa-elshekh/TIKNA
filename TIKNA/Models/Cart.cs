using TIKNA.Models;

public class Cart
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    // الـ Collection اللي الـ WithMany بيشاور عليها
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

public class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }
    public Cart Cart { get; set; } // الـ Navigation Property للسلة

    public int ProductId { get; set; }
    public Product Product { get; set; } // الـ Navigation Property للمنتج

    public int Quantity { get; set; } = 1;
}