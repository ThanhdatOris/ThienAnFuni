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
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
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
                new Category { Id = 1, Name = "Nội thất phòng khách", RoomType = "Phòng khách", UsageType = "Gia đình", IsActive = true },
                new Category { Id = 2, Name = "Ghế sofa", ParentId = 1, RoomType = "Phòng khách", UsageType = "Gia đình", IsActive = true },
                new Category { Id = 3, Name = "Nội thất phòng ngủ", RoomType = "Phòng ngủ", UsageType = "Gia đình", IsActive = true }
            );

            modelBuilder.Entity<Supplier>().HasData(
                new Supplier
                {
                    Id = 1,
                    DisplayName = "Công ty TNHH Nội thất Đại Phát",
                    Address = "Số 123, Đường Phạm Văn Đồng, Quận Bình Thạnh, TP.HCM",
                    Email = "contact@noithatdaiphat.com",
                    PhoneNumber = "0901234567",
                    Image = "noithatdaiphat_logo.jpg",
                    IsActive = true
                },
                new Supplier
                {
                    Id = 2,
                    DisplayName = "Công ty CP Gỗ Thành Đạt",
                    Address = "Số 45, Đường Lý Thái Tổ, Quận 3, TP.HCM",
                    Email = "info@gothanhdat.vn",
                    PhoneNumber = "0912345678",
                    Image = "gothanhdat_logo.jpg",
                    IsActive = true
                },
                new Supplier
                {
                    Id = 3,
                    DisplayName = "Công ty TNHH Đồ Gỗ Minh Đức",
                    Address = "Số 67, Đường Trần Hưng Đạo, Quận 1, TP.HCM",
                    Email = "minhduc@dogomd.com",
                    PhoneNumber = "0923456789",
                    Image = "dogomd_logo.jpg",
                    IsActive = false
                }
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
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Xám",
                    Type = "Nội thất",
                    Brand = "Sofaland",
                    WarrantyPeriod = "1 năm",
                    Description = "Ghế sofa đơn cho phòng khách",
                    IsActive = true,
                    MainImg = "default.png",
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
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Nâu",
                    Type = "Nội thất",
                    Brand = "Woodland",
                    WarrantyPeriod = "2 năm",
                    Description = "Kệ tivi bằng gỗ sồi chất lượng cao",
                    IsActive = true,
                    MainImg = "default.png",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Tủ Quần Áo Phòng Ngủ",
                    Price = 7500000,
                    Unit = "Cái",
                    Material = "Gỗ công nghiệp MDF",
                    Dimension = "180x60x220 cm",
                    Standard = "(*) Tiêu chuẩn E1 Châu Âu, đảm bảo không phát thải khí độc hại, thân thiện với môi trường",
                    Color = "Trắng",
                    Type = "Nội thất",
                    Brand = "Everwood",
                    WarrantyPeriod = "3 năm",
                    Description = "Tủ quần áo gỗ MDF bền đẹp, nhiều ngăn tiện dụng",
                    IsActive = true,
                    MainImg = "default.png",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Giường Ngủ Đôi",
                    Price = 5500000,
                    Unit = "Cái",
                    Material = "Gỗ tự nhiên",
                    Dimension = "200x180x40 cm",
                    Standard = "(*) Chứng nhận FSC từ tổ chức quốc tế, đảm bảo nguồn gốc gỗ bền vững",
                    Color = "Vàng nhạt",
                    Type = "Nội thất",
                    Brand = "WoodenDream",
                    WarrantyPeriod = "2 năm",
                    Description = "Giường ngủ đôi gỗ tự nhiên chắc chắn, phong cách hiện đại",
                    IsActive = true,
                    MainImg = "default.png",
                    CategoryId = 3
                }

            );
        }

    }
}
