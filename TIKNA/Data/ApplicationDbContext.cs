using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TIKNA.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProd> OrderProducts { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);



        builder.Entity<OrderProd>()
    .HasKey(op => new { op.OrderId, op.ProductId });



        builder.Entity<OrderProd>()
        .HasOne(op => op.Order)
        .WithMany(o => o.OrderProducts)
        .HasForeignKey(op => op.OrderId);

        builder.Entity<Payment>()
     .HasOne(p => p.Order)
     .WithOne(o => o.Payment)
     .HasForeignKey<Payment>(p => p.OrderId)
     .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payment>()
            .HasOne(p => p.Rental)
            .WithOne(r => r.Payment)
            .HasForeignKey<Payment>(p => p.RentalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payment>()
            .HasOne(p => p.MaintenanceRequest)
            .WithOne(m => m.Payment)
            .HasForeignKey<Payment>(p => p.MaintenanceRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // العلاقات Product → MaintenanceRequest / Rental / OrderProducts
        builder.Entity<MaintenanceRequest>()
            .HasOne(m => m.Product)
            .WithMany(p => p.MaintenanceRequests)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rental>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Rentals)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrderProd>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
      
        // ===========================
        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Entity<OrderProd>()
            .Property(op => op.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);
    }
}
