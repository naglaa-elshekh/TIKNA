using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        // نوع الدفع: (Order, Rental, Maintenance)
        [Required]
        public string PaymentType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // حالة الدفع: (Pending, Completed, Failed, Refunded)
        public string Status { get; set; } = "Completed";

        // --- الربط مع العمليات المختلفة (كلهم Nullable لأن الدفع بيكون لعملية واحدة فقط) ---

        // 1. لو الدفع لطلب شراء
        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        // 2. لو الدفع لإيجار
        public int? RentalId { get; set; }
        [ForeignKey("RentalId")]
        public virtual Rental? Rental { get; set; }

        // 3. لو الدفع لطلب صيانة
        public int? MaintenanceRequestId { get; set; }
        [ForeignKey("MaintenanceRequestId")]
        public virtual MaintenanceRequest? MaintenanceRequest { get; set; }
    }
}