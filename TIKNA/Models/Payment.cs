using System;
using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        // نوع الدفع: Order / Rental / Maintenance
        public string PaymentType { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        // FK → ممكن يكون null حسب النوع
        public int? OrderId { get; set; }
        public Order Order { get; set; }

        public int? RentalId { get; set; }
        public Rental Rental { get; set; }

        public int? MaintenanceRequestId { get; set; }
        public MaintenanceRequest MaintenanceRequest { get; set; }
    }
}


  
