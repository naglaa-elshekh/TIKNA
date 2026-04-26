using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class RentalRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } // ربط الطالب اللي بيأجر

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } // ربط الجهاز اللي هيتأجر

        [Required]
        public DateTime StartDate { get; set; } // تاريخ الاستلام

        [Required]
        public DateTime EndDate { get; set; } // تاريخ الترجيع

        public decimal TotalPrice { get; set; } // السعر النهائي المحسوب

        public string Status { get; set; } = "Pending"; // حالة الطلب

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}