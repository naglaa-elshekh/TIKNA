//using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class OrderProd
    {
        public int OrderId { get; set; }


        public Order Order { get; set; }

public int ProductId { get; set; }
public Product Product { get; set; }

public int Quantity { get; set; }

// وحدة السعر في وقت الطلب (لأن السعر ممكن يتغير بعد كده)
public decimal UnitPrice { get; set; }
  }
 }