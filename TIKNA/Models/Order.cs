using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // --- التعديل الجوهري هنا ---
        // الربط مع المشتري (ApplicationUser) مباشرة
        [Required]
        public string BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public virtual ApplicationUser Buyer { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // حالة الطلب (Pending, Shipped, Delivered, Canceled)
        [Required]
        public string Status { get; set; } = "Pending";

        // علاقة Many-to-Many مع المنتجات من خلال جدول الوسيط
        public virtual ICollection<OrderProd> OrderProducts { get; set; } = new HashSet<OrderProd>();

        // الربط مع الدفع (اختياري في البداية)
        // public int? PaymentId { get; set; }
        // public virtual Payment? Payment { get; set; }
    }
}