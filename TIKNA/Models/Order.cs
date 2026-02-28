
namespace TIKNA.Models
{
  
using TIKNA.Models;

    public class Order
    {
        public int OrderId { get; set; }

        // FK إلى المشتري (Customer)
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; }

        // علاقة Many-to-Many مع المنتجات
        public ICollection<OrderProd> OrderProducts { get; set; }

        // Payment
        public Payment? Payment { get; set; }
    }
}