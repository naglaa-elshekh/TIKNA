using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TIKNA.Models;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. علاقة الـ OrderProd (Many-to-Many)
        builder.Entity<OrderProd>().HasKey(op => new { op.OrderId, op.ProductId });

        builder.Entity<OrderProd>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.NoAction); // تغيير لـ NoAction

        // 2. علاقة الـ Cart بالمستخدم
        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction); // تغيير لـ NoAction

        // 3. علاقة الـ CartItem (لب المشكلة)
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.NoAction); // كسر المسار الأول

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.NoAction); // كسر المسار الثاني

        // 4. علاقات الـ Payment والصيانة والإيجار (تأمين إضافي)
        builder.Entity<Payment>()
            .HasOne(p => p.Order).WithOne().HasForeignKey<Payment>(p => p.OrderId).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<MaintenanceRequest>()
            .HasOne(m => m.Product).WithMany(p => p.maintenanceRequests).HasForeignKey(m => m.ProductId).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Rental>()
            .HasOne(r => r.Product).WithMany(p => p.Rentals).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.NoAction);

        // 5. ضبط الدقة المالية
        builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        builder.Entity<Order>().Property(p => p.TotalPrice).HasPrecision(18, 2);
        builder.Entity<OrderProd>().Property(op => op.UnitPrice).HasPrecision(18, 2);
        builder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
    
   

        // 1. Seed Roles
        string adminRoleId = Guid.NewGuid().ToString();
        string studentRoleId = Guid.NewGuid().ToString();
        string companyRoleId = Guid.NewGuid().ToString();

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" },
            new IdentityRole { Id = companyRoleId, Name = "Company", NormalizedName = "COMPANY" }
        );

        // 2. Seed Admin User
        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin@tikna.com",
            NormalizedUserName = "ADMIN@TIKNA.COM",
            Email = "admin@tikna.com",
            NormalizedEmail = "ADMIN@TIKNA.COM",
            EmailConfirmed = true,
            UserType = "Admin",
            ApprovalStatus = "Approved",

            // الحل هنا: لازم تدي قيمة لكل خاصية "Required" ضفتيها في الـ ApplicationUser
            Address = "Main Admin Office",
            Name = "System Admin",
        };

        PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
        adminUser.PasswordHash = ph.HashPassword(adminUser, "Admin@123");

        builder.Entity<ApplicationUser>().HasData(adminUser);

        // 3. Assign Admin to Role
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = adminRoleId,
            UserId = adminUser.Id
        });
    }
}