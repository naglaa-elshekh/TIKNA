using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int MaintenanceRequestId { get; set; }

        // --- 1. المنتج (الجهاز) اللي محتاج صيانة ---
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // --- 2. المستخدم (طالب الصيانة) ---
        // غيرناه لـ string عشان يربط مع الـ ApplicationUser
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // --- 3. تفاصيل الطلب ---
        [Required(ErrorMessage = "برجاء وصف المشكلة بالتفصيل")]
        public string IssueDescription { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        // حالة الطلب (مثلاً: Pending, InProgress, Completed, Cancelled)
        public string Status { get; set; } = "Pending";

        // الربط مع الدفع (اختياري حالياً)
         public virtual Payment? Payment { get; set; }
    }
}