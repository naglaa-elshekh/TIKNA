using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class Customer
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } // اسم الطالب في جدول العملاء

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        // الربط مع الـ Identity
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
        //public ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();
        //public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new HashSet<MaintenanceRequest>();
        //public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}