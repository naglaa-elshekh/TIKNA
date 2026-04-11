using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TIKNA.Models
{
    public class ApplicationUser : IdentityUser
    {
        // تم التوحيد لتشمل اسم الفرد أو اسم الكيان (الشركة/المركز)
        [Required]
        public string Name { get; set; }

        [Required]
        public string UserType { get; set; } // (Student, Company, Admin)

        [Required]
        public string Address { get; set; }

        // بيانات الطالب (تكون Null لو المستخدم شركة)
        public string? University { get; set; }
        public string? Faculty { get; set; }

        // بيانات الشركة (تكون Null لو المستخدم فرد)
        public string? CommercialRegister { get; set; }
        public string? Bio { get; set; }
        public string? CompanyServiceType { get; set; }

        // حالة القبول (Default: Approved للطالب، ويتم تغييرها لـ Pending في الـ Register لو شركة)
        public string ApprovalStatus { get; set; } = "Approved";

        public ICollection<Product>? Products { get; set; }
        public ICollection<MaintenanceRequest>? MaintenanceRequests { get; set; }
    }
}