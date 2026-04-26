using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int Id { get; set; }



        public string UserId { get; set; } // عشان نعرف مين الطالب اللي بعت

        [Required]
        public string OrderNumber { get; set; } // رقم الطلب (مثلاً TIKNA-M-123)

        // --- بيانات الجهاز (من أول صور الفرونت) ---
        [Required]
        public string Brand { get; set; } // الماركة (Dell, HP, etc.)

        [Required]
        public string ModelName { get; set; } // اسم الموديل

        public string DeviceAge { get; set; } // عمر الجهاز (سنة، سنتين...)

        // --- تفاصيل المشكلة ---
        [Required]
        public string ProblemType { get; set; } // نوع العطل (Hardware, Software, Screen)

        [Required]
        public string Description { get; set; } // وصف المشكلة التفصيلي

        // --- خيارات الخدمة والموعد (من الصور الأخيرة) ---
        public string ServiceType { get; set; } // (في المركز، صيانة منزلية، استلام وتوصيل)

        [DataType(DataType.Date)]
        public DateTime PreferredDate { get; set; } // التاريخ اللي اختاره الطالب

        public string PreferredTimeSlot { get; set; } // الفترة (صباحاً، مساءً)

        // --- الحسابات (للتكلفة التقريبية) ---
        public decimal EstimatedPriceMin { get; set; } // السعر الأدنى المتوقع
        public decimal EstimatedPriceMax { get; set; } // السعر الأقصى المتوقع

        // --- حالة الطلب ---
        public string Status { get; set; } = "Pending"; // الحالة الافتراضية

        // الربط مع الدفع (اختياري حالياً)
         public virtual Payment? Payment { get; set; }
        public DateTime CreatedAt { get; internal set; }
        public Product Product { get; internal set; }
    }
}