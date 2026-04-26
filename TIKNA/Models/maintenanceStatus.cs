namespace TIKNA.Models
{
    public enum MaintenanceStatus
    {
        Pending,          // الطلب لسه واصل وبانتظار مراجعة الأدمن
        Approved,         // الأدمن وافق على الطلب ومستنيين استلام الجهاز
        InRepair,         // الجهاز حالياً مع الفني وبيتم تصليحه
        ReadyForPickup,   // الجهاز خلص تصليح ومستني الطالب يستلمه
        Completed,        // الطالب استلم جهازه والعملية تمت بنجاح
        Cancelled,        // الطلب اترفض أو اتلغى
        Delayed           // فيه مشكلة في قطع الغيار والطلب متأخر
    }
}