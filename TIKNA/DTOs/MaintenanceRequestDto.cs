using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TIKNA.DTOs
{
    public class MaintenanceRequestDto
    {
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string DeviceAge { get; set; }
        public string ProblemType { get; set; }
        public string Description { get; set; }
        public string ServiceType { get; set; }
        public DateTime PreferredDate { get; set; }
       
       
            // ... الحقول اللي كتبتيها (Brand, Model, etc.)
            [Required]
            public string CenterId { get; set; } // لازم الطالب يبعت الـ ID بتاع المركز اللي اختاره
        }
    }
public class MaintenanceUpdateDto
{

    [JsonPropertyName("finalPrice")]
    public decimal FinalPrice { get; set; }
    public string Note { get; set; }
}