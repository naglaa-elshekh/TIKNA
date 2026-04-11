using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class OrderProd
    {
        // إضافة ID منفصل بيسهل التعامل جداً مع الـ Entity Framework والـ APIs
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون 1 على الأقل")]
        public int Quantity { get; set; }

        // سعر الوحدة وقت الطلب (مهم جداً زي ما ذكرتِ عشان لو السعر اتغير في جدول Product)
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // خاصية إضافية (Read-only) لحساب إجمالي السعر لهذا المنتج داخل الطلب
        public decimal SubTotal => Quantity * UnitPrice;
    }
}