using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TIKNA.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } // ربط الطلب باليوزر

        [Required]
        public string DeviceType { get; set; } // لاب توب، تابلت.. إلخ

        [Required]
        public string ModelName { get; set; }

        public string ProblemDescription { get; set; }
        public string? ImagePath { get; set; }

        public string OrderNumber { get; set; } // TIKNA-M-XXXX

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;

        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? AdminComment { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}