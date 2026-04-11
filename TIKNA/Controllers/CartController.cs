//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using TIKNA.Models;

//namespace TIKNA.Data
//{
//    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        // تسجيل الجداول في الداتابيز
//        public DbSet<Product> Products { get; set; }
//        public DbSet<Order> Orders { get; set; }
//        public DbSet<OrderProd> OrderProducts { get; set; }
//        public DbSet<Rental> Rentals { get; set; }
//        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
//        public DbSet<Payment> Payments { get; set; }
//        public DbSet<Cart> Carts { get; set; }
//        public DbSet<CartItem> CartItems { get; set; }

//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            // ضروري جداً لتشغيل جداول الـ Identity (Users, Roles)
//            base.OnModelCreating(builder);

//            // 1. إعداد الـ Primary Key لجدول الوسيط OrderProd (Many-to-Many)
//            builder.Entity<OrderProd>()
//                .HasKey(op => new { op.OrderId, op.ProductId });

//            // 2. علاقة المنتج بصاحبه (ApplicationUser)
//            builder.Entity<Product>()
//                .HasOne(p => p.Owner)
//                .WithMany(u => u.Products)
//                .HasForeignKey(p => p.OwnerId)
//                .OnDelete(DeleteBehavior.Cascade);

//            // 3. علاقة الطلب بالمشتري (ApplicationUser)
//            builder.Entity<Order>()
//                .HasOne(o => o.Buyer)
//                .WithMany() // المستخدم ممكن يكون له طلبات كتير
//                .HasForeignKey(o => o.BuyerId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // 4. علاقة السلة بالمستخدم (One-to-One)
//            builder.Entity<Cart>()
//                .HasOne(c => c.User)
//                .WithOne()
//                .HasForeignKey<Cart>(c => c.UserId)
//                .OnDelete(DeleteBehavior.Cascade);

//            // 5. علاقة الدفع (Payment) بالعمليات المختلفة
//            // استخدام Restrict لمنع تعارض الـ Cascade Paths في SQL Server
//            builder.Entity<Payment>()
//                .HasOne(p => p.Order)
//                .WithOne()
//                .HasForeignKey<Payment>(p => p.OrderId)
//                .OnDelete(DeleteBehavior.Restrict);

//            builder.Entity<Payment>()
//                .HasOne(p => p.Rental)
//                .WithOne()
//                .HasForeignKey<Payment>(p => p.RentalId)
//                .OnDelete(DeleteBehavior.Restrict);

//            builder.Entity<Payment>()
//                .HasOne(p => p.MaintenanceRequest)
//                .WithOne()
//                .HasForeignKey<Payment>(p => p.MaintenanceRequestId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // 6. علاقات المنتج (Product) مع الإيجار والصيانة
//            builder.Entity<MaintenanceRequest>()
//                .HasOne(m => m.Product)
//                .WithMany(p => p.MaintenanceRequests)
//                .HasForeignKey(m => m.ProductId)
//                .OnDelete(DeleteBehavior.Restrict);

//            builder.Entity<Rental>()
//                .HasOne(r => r.Product)
//                .WithMany(p => p.Rentals)
//                .HasForeignKey(r => r.ProductId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // 7. ضبط دقة الأرقام المالية (Decimal Precision)
//            builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
//            builder.Entity<Order>().Property(o => o.TotalPrice).HasPrecision(18, 2);
//            builder.Entity<OrderProd>().Property(op => op.UnitPrice).HasPrecision(18, 2);
//            builder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
//        }
//    }
//}