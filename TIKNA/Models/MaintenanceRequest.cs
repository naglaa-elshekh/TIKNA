using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class MaintenanceRequest
    {
        public int MaintenanceRequestId { get; set; }

        // المنتج الذي يحتاج صيانة
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // العميل الذي طلب الصيانة
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        // وصف المشكلة
        public string IssueDescription { get; set; }

        public DateTime RequestDate { get; set; }

        // Payment for maintenance
        public Payment Payment { get; set; }
    }
}

