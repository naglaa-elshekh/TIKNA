using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TIKNA.Models;

namespace TIKNA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProd> OrderProducts { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // حل مشكلة الـ Multiple Cascade Paths لطلبات الإيجار
            builder.Entity<RentalRequest>()
                .HasOne(rr => rr.Product)
                .WithMany() // أو حسب علاقتك في موديل المنتج
                .HasForeignKey(rr => rr.ProductId)
                .OnDelete(DeleteBehavior.NoAction); // هذا هو السطر المنقذ

            builder.Entity<RentalRequest>()
                .HasOne(rr => rr.User)
                .WithMany()
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // تأكدي أيضاً من إضافة ضبط الدقة المالية الذي ظهر في التحذير السابق
            builder.Entity<RentalRequest>().Property(r => r.TotalPrice).HasPrecision(18, 2);

            // 1. علاقة الـ OrderProd (Many-to-Many)
            builder.Entity<OrderProd>().HasKey(op => new { op.OrderId, op.ProductId });
            builder.Entity<RentalRequest>().Property(r => r.TotalPrice).HasPrecision(18, 2);

            builder.Entity<OrderProd>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            builder.Entity<OrderProd>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. علاقات الـ Payment (One-to-One)
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

            // 3. علاقات المنتجات (الصيانة والإيجار)
           
            builder.Entity<Rental>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Rentals)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. علاقة الـ Cart والـ CartItem
            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<MaintenanceRequest>()
        .HasOne(m => m.Student) // يجب أن يطابق الاسم في الموديل فوق
        .WithMany(u => u.MaintenanceRequests)
        .HasForeignKey(m => m.UserId)
        .OnDelete(DeleteBehavior.Restrict);

            // علاقة المركز
            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Center) // يجب أن يطابق الاسم في الموديل فوق
                .WithMany()
                .HasForeignKey(m => m.CenterId)
                .OnDelete(DeleteBehavior.Restrict);



            // 5. ضبط الدقة المالية والـ Enums
            builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            builder.Entity<Product>().Property(p => p.RentalPricePerDay).HasPrecision(18, 2);
            builder.Entity<Order>().Property(p => p.TotalPrice).HasPrecision(18, 2);
            builder.Entity<OrderProd>().Property(op => op.UnitPrice).HasPrecision(18, 2);
            builder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

            builder.Entity<MaintenanceRequest>(entity =>
            {
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.EstimatedPriceMin).HasPrecision(18, 2);
                entity.Property(e => e.EstimatedPriceMax).HasPrecision(18, 2);
            });

            // 6. Seed Roles
            string adminRoleId = "1";
            string studentRoleId = "2";
            string companyRoleId = "3";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" },
                new IdentityRole { Id = companyRoleId, Name = "Company", NormalizedName = "COMPANY" }
            );

            // 7. Seed Admin User
            var adminUser = new ApplicationUser
            {
                Id = "admin-id-123",
                UserName = "admin@tikna.com",
                NormalizedUserName = "ADMIN@TIKNA.COM",
                Email = "admin@tikna.com",
                NormalizedEmail = "ADMIN@TIKNA.COM",
                EmailConfirmed = true,
                UserType = "Admin",
                ApprovalStatus = "Approved",
                Address = "Main Admin Office",
                Name = "System Admin",
            };

            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Admin@123");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUser.Id
            });
        }
    }
}