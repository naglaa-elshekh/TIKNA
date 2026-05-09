using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Student { get; set; }

        [Required]
        public string OrderNumber { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string ModelName { get; set; }

        public string DeviceAge { get; set; }

        [Required]
        public string ProblemType { get; set; }

        [Required]
        public string Description { get; set; }

        public string ServiceType { get; set; }

        [DataType(DataType.Date)]
        public DateTime PreferredDate { get; set; }


        // --- الحسابات (التكلفة التقريبية تظهر عند الإنشاء) ---
        public decimal EstimatedPriceMin { get; set; }
        public decimal EstimatedPriceMax { get; set; }

        // ************************************************************
        // --- الحقول الجديدة التي كانت ناقصة لإتمام عملية الدفع ---

        // 1. السعر النهائي الذي يحدده المركز بعد الفحص الفعلي
        public decimal? FinalPrice { get; set; }

        // 2. ملاحظات من المركز يشرح فيها العطل للعميل قبل الدفع
        public string? NoteFromCenter { get; set; }

        // 3. حالة الدفع (تتحول لـ True بعد نجاح العملية في صفحة الدفع)
        public bool IsPaid { get; set; } = false;

        // 4. مسار صورة الجهاز (إذا قررتِ تفعيل رفع الصور)
        public string? ImagePath { get; set; }
        // ************************************************************

        public string Status { get; set; } = "Pending";

        public virtual Payment? Payment { get; set; }

        public DateTime CreatedAt { get; set; } // تأكدي أنها set وليست internal لتتمكني من حفظها

        // ربط الطلب بالشركة (المركز)
        [Required]
        public string CenterId { get; set; }

        [ForeignKey("CenterId")]
        public virtual ApplicationUser Center { get; set; }
    }
}