using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TIKNA.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Brand { get; set; }

    [Required]
    public string Model { get; set; }

    public string? Description { get; set; }

    // الحقول التقنية (تأكدي من وجودها بهذه المسميات)
    public string? CPU { get; set; }
    public string? RAM { get; set; }
    public string? Storage { get; set; }
    public string? GPU { get; set; }
    public string? ScreenSize { get; set; }
    public string? Color { get; set; }
    public string? Condition { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Quantity { get; set; } = 1;
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }

    // حالات البيع والإيجار
    public bool ForSale { get; set; } = true;
    public bool ForRent { get; set; } = false;
    public decimal? RentalPricePerDay { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public virtual ApplicationUser Owner { get; set; }

    public virtual ICollection<OrderProd>? OrderProducts { get; set; }
    public virtual ICollection<MaintenanceRequest>? MaintenanceRequests { get; set; }
    public virtual ICollection<Rental>? Rentals { get; set; }


}