using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; } // المواصفات الفنية

        [Required]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int Quantity { get; set; }

        public string? Brand { get; set; }

        public string? Category { get; set; } // (Gaming, Student, Business...)

        public bool IsActive { get; set; } = true;

        // --- التعديل الجوهري هنا ---
        // الربط مباشرة مع الـ ApplicationUser (صاحب المنتج)
        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser Owner { get; set; }

        // العلاقات التانية (لو لسه محتاجاهم)
        public ICollection<OrderProd>? OrderProducts { get; set; }
        public ICollection<Rental>? Rentals { get; set; }
        public ICollection<MaintenanceRequest>? maintenanceRequests { get; set; }

    }
}