using Microsoft.EntityFrameworkCore;

namespace ThienAnFuni.Models
{
    public class TAF_DbContext : DbContext
    {
        public TAF_DbContext(DbContextOptions<TAF_DbContext> options)
        : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SaleStaff> SaleStaffs { get; set; }
        public DbSet<Manager> Managers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Nội thất phòng khách", RoomType = "Phòng khách", UsageType = "Gia đình", Active = true },
                new Category { Id = 2, Name = "Ghế sofa", ParentId = 1, RoomType = "Phòng khách", UsageType = "Gia đình", Active = true }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Ghế Sofa Đơn",
                    Price = 1200000,
                    Unit = "Cái",
                    Material = "Vải nỉ",
                    Dimension = "120x80x75 cm",
                    Standard = "Hàng xuất khẩu",
                    Color = "Xám",
                    Type = "Nội thất",
                    Brand = "Sofaland",
                    WarrantyPeriod = "1 năm",
                    Description = "Ghế sofa đơn cho phòng khách",
                    Active = true,
                    MainImg = "sofa_don.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 2,
                    Name = "Kệ Tivi Phòng Khách",
                    Price = 3500000,
                    Unit = "Cái",
                    Material = "Gỗ sồi",
                    Dimension = "200x50x60 cm",
                    Standard = "Hàng nhập khẩu",
                    Color = "Nâu",
                    Type = "Nội thất",
                    Brand = "Woodland",
                    WarrantyPeriod = "2 năm",
                    Description = "Kệ tivi bằng gỗ sồi chất lượng cao",
                    Active = true,
                    MainImg = "ke_tivi.jpg",
                    CategoryId = 1
                }
            );
        }

    }
}
