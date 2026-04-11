using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        // --- 1. المنتج المستأجر ---
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // --- 2. المستأجر (ApplicationUser) ---
        // تم التغيير لـ string للربط مع الـ Identity
        [Required]
        public string renterId { get; set; }

        [ForeignKey("renterId")]
        public virtual ApplicationUser Renter { get; set; }

        // --- 3. تفاصيل مدة الإيجار ---
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; }

        // حالة الإيجار (مثلاً: Active, Returned, Overdue, Cancelled)
        public string Status { get; set; } = "Active";

        // خاصية إضافية (Read-only) لحساب عدد الأيام أوتوماتيكياً
        public int TotalDays => (EndDate - StartDate).Days;

        // الربط مع الدفع (اختياري حالياً)
        // public int? PaymentId { get; set; }
        // public virtual Payment? Payment { get; set; }
    }
}