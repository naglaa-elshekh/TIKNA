namespace TIKNA.Models
{
    public enum MaintenanceStatus
    {
        Pending,     // قيد الانتظار
        InProgress,  // جاري الإصلاح
        Fixed,       // تم الإصلاح
        Delivered,   // تم التسليم
        Cancelled    // ملغي
    }
}