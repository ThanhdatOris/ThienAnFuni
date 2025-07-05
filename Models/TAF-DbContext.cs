using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models.Momo;

namespace ThienAnFuni.Models
{
    public class TAF_DbContext : IdentityDbContext<User>
    {
        public TAF_DbContext(DbContextOptions<TAF_DbContext> options)
        : base(options)
        {
        }

        public DbSet<MomoTransaction> MomoTransactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SaleStaff> SaleStaffs { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Sử dụng PasswordHasher để mã hóa mật khẩu khi tạo người dùng mẫu
            var passwordHasher = new PasswordHasher<User>();

            modelBuilder.Entity<Manager>().HasData(
                new Manager
                {
                    Id = "1",
                    UserName = "sinoo",
                    NormalizedUserName = "SINOO",
                    PasswordHash = passwordHasher.HashPassword(null, "123456"), // Sử dụng PasswordHasher
                    FullName = "Sinoo",
                    PhoneNumber = "0123456789",
                    Address = "123 Main St",
                    Gender = "Nam",
                    DateOfBirth = new DateTime(2004, 5, 12),
                    CitizenId = "123456789",
                    IsActive = true
                },
                new Manager
                {
                    Id = "2",
                    UserName = "vyvy",
                    NormalizedUserName = "VYVY",
                    PasswordHash = passwordHasher.HashPassword(null, "123456"),
                    FullName = "Nu Thao Vy",
                    PhoneNumber = "0987654321",
                    Address = "456 Another St",
                    Gender = "Nữ",
                    DateOfBirth = new DateTime(2004, 8, 25),
                    CitizenId = "987654321",
                    IsActive = true
                }
            );

            modelBuilder.Entity<SaleStaff>().HasData(
                new SaleStaff
                {
                    Id = "3",
                    UserName = "tramanh",
                    NormalizedUserName = "TRAMANH",
                    PasswordHash = passwordHasher.HashPassword(null, "123456"),
                    FullName = "Huynh Thị Trâm Anh",
                    PhoneNumber = "0123456789",
                    Address = "123 Sóc Trăng",
                    Gender = "Nữ",
                    DateOfBirth = new DateTime(1995, 5, 12),
                    CitizenId = "999456789",
                    IsActive = true,
                    Email = "tramanh@gmail.com",
                    StartDate = new DateTime(2021, 5, 12)
                },
                new SaleStaff
                {
                    Id = "4",
                    UserName = "vanminh",
                    NormalizedUserName = "VANMINH",
                    PasswordHash = passwordHasher.HashPassword(null, "123456"),
                    FullName = "Nguyễn Văn Minh",
                    PhoneNumber = "0987654321",
                    Address = "456 An Giang",
                    Gender = "Nam",
                    DateOfBirth = new DateTime(1990, 8, 25),
                    CitizenId = "9876234521",
                    IsActive = true,
                    Email = "",
                    StartDate = new DateTime(2023, 5, 12)
                }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = "5",
                    UserName = "hongdaocus",
                    NormalizedUserName = "HONGDAOCUS",
                    PasswordHash = passwordHasher.HashPassword(null, "123456"),
                    FullName = "Huỳnh Hồng Đào",
                    PhoneNumber = "012322289",
                    Address = "123 Kiên Giang",
                    Gender = "Nữ",
                    DateOfBirth = new DateTime(1995, 5, 12),
                    IsActive = true
                },
                new Customer
                {
                    Id = "6",
                    UserName = "teoemcus",
                    NormalizedUserName = "TEOEMCUS",
                    PasswordHash = passwordHasher.HashPassword(null, "123456"),
                    FullName = "Nguyễn Văn Tèo",
                    PhoneNumber = "0987111321",
                    Address = "456 Hậu Giang",
                    Gender = "Nam",
                    DateOfBirth = new DateTime(1990, 8, 25),
                    IsActive = true,
                    Email = "Khoalmht0@gmail.com"
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Phòng khách", Slug = "Phòng khách".ToSlug(), IsActive = true, Image = "cat-1.jpg" },
                new Category { Id = 2, Name = "Ghế sofa", Slug = "Ghế sofa".ToSlug(), ParentId = 1, IsActive = true },
                new Category { Id = 3, Name = "Bàn sofa", Slug = "Bàn sofa".ToSlug(), ParentId = 1, IsActive = true },

                new Category { Id = 4, Name = "Phòng ngủ", Slug = "Phòng ngủ".ToSlug(), IsActive = true, Image = "cat-2.jpg" },
                new Category { Id = 5, Name = "Giường", Slug = "Giường".ToSlug(), ParentId = 4, IsActive = true },
                new Category { Id = 6, Name = "Bàn Trang Điểm", Slug = "Bàn Trang Điểm".ToSlug(), ParentId = 4, IsActive = true },

                new Category { Id = 7, Name = "Phòng làm việc", Slug = "Phòng làm việc".ToSlug(), IsActive = true, Image = "cat-3.jpg" },
                new Category { Id = 8, Name = "Bàn làm việc", Slug = "Bàn làm việc".ToSlug(), ParentId = 7, IsActive = true },
                new Category { Id = 9, Name = "Ghế văn phòng", Slug = "Ghế văn phòng".ToSlug(), ParentId = 7, IsActive = true },

                new Category { Id = 10, Name = "Phòng ăn", Slug = "Phòng ăn".ToSlug(), IsActive = true, Image = "cat-4.jpg" },
                new Category { Id = 11, Name = "Bàn ăn", Slug = "Bàn ăn".ToSlug(), ParentId = 10, IsActive = true },
                new Category { Id = 12, Name = "Ghế ăn", Slug = "Ghế ăn".ToSlug(), ParentId = 10, IsActive = true }
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
            // Ghế sofa
                new Product
                {
                    Id = 1,
                    Name = "Ghế Đôn Sofa HOBRO 301",
                    Price = 3990000,
                    Unit = "Cái",
                    Material = "<p><strong>Vải Polyester</strong>: Ghế c&oacute; m&agrave;u x&aacute;m đậm&nbsp;n&acirc;ng tầm thẩm mỹ v&agrave; kh&ocirc;ng gian nội thất ph&ograve;ng kh&aacute;ch của bạn;</p>\r\n\r\n<p><strong>Gỗ cao su tự nhi&ecirc;n:</strong> Nội thất Thi&ecirc;n &Acirc;n sử dụng chất liệu gỗ cao su gi&uacute;p ghế sofa gỗ HOBRO c&oacute; khả năng chịu lực tốt v&agrave; độ bền cao.</p>\r\n",
                    Dimension = "900 x 900 x 400 cm",
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Nâu",
                    Brand = "Sofaland",
                    WarrantyPeriod = "1 năm",
                    Description = "Dễ dàng vệ sinh: Nệm Sofa bằng vải polyester chống bụi, kháng ẩm mốc và có thể dễ dàng tháo bọc nệm để vệ sinh. Đệm ghế sofa có màu xám tạo nên vẻ hiện đại nhưng không kém phần sang trọng cho căn phòng của bạn.; Ghế Sofa đơn có kích thước tiêu chuẩn: Ghế Đôn có kích thước rộng rãi , với thiết kế tinh giản giúp chúng gọn nhẹ và tiết kiệm được diện tích căn phòng, bạn cũng có thể ngồi một cách thoải mái và bài trí trong nhiều không gian khác nhau.; Mút đệm cao cấp, dày dặn và chống cháy: Độ bền cao, chống xẹp lún và đạt tiêu chuẩn của Mỹ về khả năng chống cháy.; Độ cao chân ghế vừa phải: Chân ghế có chiều cao hợp lý nên Robot hút bụi có thể lau dọn phía dưới sàn một cách dễ dàng; Chân ghế cao su: Sử dụng chất liệu gỗ cao su tự nhiên với khả năng chịu lực cao.",
                    IsActive = true,
                    IsImport = true,
                    MainImg = "gheSofa_HOBRO301trai.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 2,
                    Name = "Ghế Sofa HOBRO 301 (180)",
                    Price = 13990000,
                    Unit = "Cái",
                    Material = "<p><strong>Vải Polyester:</strong> Ghế c&oacute; m&agrave;u x&aacute;m đậm n&acirc;ng tầm thẩm mỹ v&agrave; kh&ocirc;ng gian nội thất ph&ograve;ng kh&aacute;ch nh&agrave; bạn.</p>\r\n\r\n<p><strong>Gỗ cao su tự nhi&ecirc;n:</strong> Nội thất Thi&ecirc;n &Acirc;n sử dụng chất liệu gỗ cao su gi&uacute;p ghế sofa gỗ VLINE c&oacute; khả năng chịu lực tốt v&agrave; độ bền cao.</p>\r\n",
                    Dimension = "900 x 1800 x 700 cm",
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Nâu",
                    Brand = "Woodland",
                    WarrantyPeriod = "2 năm",
                    Description = "Các điểm tựa trên ghế có thiết kế bo tròn: Phần đệm tay và đệm lưng của ghế sofa được thiết kế bo tròn mềm mại, tạo cảm giác thoải mái cho người dùng mỗi khi ngồi. Vừa vặn tiếp xúc, nâng đỡ khung xương cơ thể, tạo cảm giác thoải mái mỗi khi ngồi hoặc nằm.; Đệm ghế có khóa dán chống trượt: ",
                    IsActive = true,
                    IsImport = true,
                    MainImg = "gheSofa_CaoSuTN_VLINE601phai_nau.jpg",
                    CategoryId = 2
                },

                 // Bàn sofa
                 new Product
                 {
                     Id = 3,
                     Name = "Bàn Sofa HOBRO 301",
                     Price = 1990000,
                     Unit = "Cái",
                     Material = "<p>Sử dụng gỗ tr&agrave;m tự nhi&ecirc;n đảm bảo về độ chắc chắn cao, chống ch&ocirc;ng v&ecirc;nh, mối mọt cho tủ đầu giường nh&agrave; bạn.</p>\r\n\r\n<p><strong>MDF veneer gỗ tr&agrave;m/ PB:</strong> Sử dụng MDF veneer gỗ tr&agrave;m gi&uacute;p tăng gi&aacute; trị của tủ đầu giường HOBRO đặc biệt đem tới hiệu ứng gi&aacute;c quang học v&agrave; m&agrave;u sắc v&ocirc; c&ugrave;ng độc đ&aacute;o.</p>\r\n\r\n<p>Đ&acirc;y cũng l&agrave; đặc trưng của bộ sưu tập HOBRO. Hơn nữa, THI&Ecirc;N &Acirc;N sử dụng MDF v&agrave; PB đạt chuẩn CARB-P2 an to&agrave;n tuyệt đối cho người sức khỏe người d&ugrave;ng an to&agrave;n cho cả gia đ&igrave;nh bạn.</p>\r\n",
                     Dimension = "400 x 900 400 cm",
                     Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                     Color = "Nâu",
                     Brand = "Sofaland",
                     WarrantyPeriod = "1 năm",
                     Description = "Thiết kế tinh tế: Sự cơ bản về hình dáng kết hợp những lát vân veneer kỹ thuật tạo hình nhẹ nhàng. Bàn sofa Hobro trở thành mảnh ghép cuối cùng trong không gian sang trọng của bộ sưu tập…;Bền bỉ theo thời gian: Cấu trúc rắn chắc với độ dày lớn của khung gỗ. Bên canh đó, sự hoàn thiện chỉnh chu và tỉ mỉ của những đường veneer cũng giúp bảo vệ mặt bàn tuyệt đối.; Sáng tạo theo cách của bạn: Bàn Sofa dài bằng chiều rộng của Sofa Hobro, vừa làm bàn trà vừa làm bàn trang trí bênh cạnh Sofa. Sản phẩm này sẽ giúp bạn tạo nên những bố cục nội thất độc đáo sang tạo cho ngôi nhà.;",
                     IsActive = true,
                     IsImport = true,
                     MainImg = "banSofa_HOBRO301.jpg",
                     CategoryId = 3
                 },
                new Product
                {
                    Id = 4,
                    Name = "Bàn Trà Gỗ THIÊN ÂN OSLO 901",
                    Price = 2190000,
                    Unit = "Cái",
                    Material = "<p><strong>Gỗ cao su tự nhi&ecirc;n: </strong>B&agrave;n tr&agrave; chữ nhật OSLO l&agrave;m từ gỗ cao su gi&uacute;p sản phẩm c&oacute; khả năng chịu lực tốt v&agrave; độ bền cao.</p>\r\n\r\n<p><strong>Gỗ c&ocirc;ng nghiệp, Veneer gỗ sồi</strong>: Sử dụng chất liệu gỗ c&ocirc;ng nghiệp PB đạt chuẩn CARB-P2 v&agrave; chứng nhận FSC th&acirc;n thiện với m&ocirc;i trường.</p>\r\n",
                    Dimension = "Dài 95cm x Rộng 50cm x Cao 42cm",
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Nâu",
                    Brand = "Woodland",
                    WarrantyPeriod = "2 năm",
                    Description = "Thiết kế tối ưu tiện ích: Bàn trà có thiết kế thêm kệ ngăn bên dưới gia tăng khả năng chứa đồ đầy tiện lợi.; Mặt bàn Veneer gỗ sồi: Bề mặt sản phẩm được xử lý nhẵn mịn, veneer thêm vân gỗ sồi mang màu sắc đẹp tự nhiên.; Các góc cạnh bàn được bo tròn: Từng đường nét góc cạnh được hoàn thiện một cách tỉ mỉ đem lại vẻ đẹp hoàn thiện cho sản phẩm.; Chân bàn gỗ cao su: Phần chân bàn trà sofa được làm hoàn toàn bằng gỗ cao su tự nhiên giúp khung bàn hoàn toàn chắc chắn, chịu tải trọng lớn từ nhiều món đồ trang trí như chậu cây, sách báo...; Độ rộng giữa kệ ngăn và mặt bàn rộng: Khoảng cách giữa ngăn kệ và mặt bàn khá lớn, thuận tiện cho việc vệ sinh, lau chùi.",
                    IsActive = true,
                    IsImport = true,
                    MainImg = "banSofa_OSLO901.jpg",
                    CategoryId = 3
                },

                // Phòng ngủ: 
                // Giường
                new Product
                {
                    Id = 11,
                    Name = "Giường Ngủ Gỗ Tự Nhiên Mây Mắt Cáo THIÊN ÂN FIJI 401",
                    Price = 12490000,
                    Unit = "Cái",
                    Material = "<p><strong>Gỗ tr&agrave;m: </strong>Giường ngủ được chế t&aacute;c từ chất liệu gỗ tr&agrave;m gi&uacute;p gia tăng khả năng chịu lực tốt v&agrave; độ bền cao, đảm bảo sự vững chắc v&agrave; chống cong v&ecirc;nh, mối mọt trong suốt thời gian sử dụng.</p>\r\n\r\n<p><strong>M&acirc;y tre đan: </strong>Đầu giường sử dụng bằng m&acirc;y mắt c&aacute;o tự nhi&ecirc;n chất lượng cao, chống mối mọt, c&ocirc;n tr&ugrave;ng được l&agrave;m thủ c&ocirc;ng từ nghệ nh&acirc;n v&agrave; thợ l&agrave;nh nghề gi&uacute;p tạo cảm gi&aacute;c thoải m&aacute;i, gần gũi với thi&ecirc;n nhi&ecirc;n cho căn ph&ograve;ng của bạn.</p>\r\n",
                    Dimension = " Dài 210cm x Rộng 167/187cm x Cao 90cm (1m6)",
                    Standard = "(*) Tiêu chuẩn E1 Châu Âu, đảm bảo không phát thải khí độc hại, thân thiện với môi trường",
                    Color = "Nâu",
                    Brand = "Everwood",
                    WarrantyPeriod = "3 năm",
                    Description = "Thiết kế giường ngủ truyền thống và sang trọng: Giường FIJI được thiết kế tối giản kết hợp với vẻ đẹp của chất liệu mây đan tự nhiên chắc chắn sẽ thổi thêm nét xanh và mang đến cho không gian phòng ngủ của bạn vẻ đẹp Á Đông mới. Bạn có thể cảm nhận sự tươi mới như đang thong dong giữa một cánh rừng xanh mát bất tận. Ngoài ra, giường FIJI còn là một lựa chọn tuyệt vời cho những ai yêu thích sự tự nhiên của phong cách Tropical. ; Giường ngủ gỗ có kích thước 1m6/1m8 - Phổ biến với người dùng Việt: Giường ngủ gỗ FIJI với kích thước chiều dài 1m6/1m8 theo tiêu chuẩn gia đình và khách sạn, đây là kích thước thông dụng nhất hiện nay phù hợp với nhiều không gian.;  Giường ngủ mang thiết kế bo tròn: Thiết kế bo tròn các góc cạnh vừa tạo đường nét mềm mại, sự tinh tế trong thiết kế. Đặc biệt hạn chế thương tích khi va đập, rất an toàn cho gia đình có trẻ nhỏ và người lớn tuổi.; Kết cấu đầu giường đầu giường chắc chắn, bền bỉ: Với kết cấu đầu giường mới đã được cải tiến thêm tấm gỗ MDF hỗ trợ giúp độ chịu lực tăng lên gấp nhiều lần, đồng thời cũng giúp tăng tuổi thọ của mây và bền bỉ hơn với thời gian.; Bộ khung kim loại chắc chắn: Khung đỡ cố định được lắp ghép khít nối với nhau cùng thanh chịu lực ở vạt giường, tăng khả năng chịu lực cho giường ngủ FIJI.; Tấm phản chắc chắn: Giường ngủ gỗ FIJI có thiết kế nguyên tấm, phản giường giúp kết cấu của toàn bộ giường trở nên vững chắc hơn, có thể chịu lực các tấm nệm dày lên đến 30cm. Mây tự nhiên - Nguyên liệu truyền thống từ làng nghề Việt: Với ưu điểm là vẻ đẹp mộc mạc, tự nhiên, thân thiện với môi trường, những nhà thiết kế của THIÊN ÂN đã tinh tế kết hợp chất liệu gỗ và mây tự nhiên họa tiết mắt cáo để mang đến cho không gian sống vẻ đẹp hài hòa, ấm áp.; Dễ dàng vệ sinh sạch sẽ: Chiều cao giường ngủ hợp lý phù hợp cho các thiết bị robot hút bụi, máy hút bụi,... có thể dễ dàng vệ sinh. ; Chân giường chịu lực cao: Chân giường bằng gỗ tràm tự nhiên cùng logo thương hiệu độc quyền của nội thất THIÊN ÂN, chống hàng giả, đạo nhái, đảm bảo an toàn, kết cấu chắc chắn.",
                    IsActive = true,
                    IsImport = true,
                    MainImg = "giuong_FIJI401.1.jpg",
                    CategoryId = 5
                },
                new Product
                {
                    Id = 12,
                    Name = "Giường Ngủ Gỗ Cao Su THIÊN ÂN HOBRO 301",
                    Price = 11890000,
                    Unit = "Cái",
                    Material = "<p><strong>Gỗ cao su tự nhi&ecirc;n: </strong>Sử dụng chất liệu gỗ cao su gi&uacute;p giuờng ngủ c&oacute; khả năng chịu lực tốt v&agrave; độ bền cao.</p>\r\n\r\n<p><strong>Gỗ c&ocirc;ng nghiệp Plywood:</strong> Tấm phảm sử dụng chất liệu Plywood 12mm theo ti&ecirc;u chuẩn CARBP2 vừa th&acirc;n thiện với m&ocirc;i trường, đảm bảo sức khỏe v&agrave; đặc biệt độ chịu lực tại 1 khu vực với diện t&iacute;ch 400 x 488mm l&ecirc;n tới 175kg khi d&ugrave;ng nệm tr&ecirc;n 15cm.</p>\r\n",
                    Dimension = "Dài 210cm x Rộng 171/ 191 cm; Cao đến đầu giường 90 cm; Gầm giường cao 16cm (1m8)",
                    Standard = "(*) Chứng nhận FSC từ tổ chức quốc tế, đảm bảo nguồn gốc gỗ bền vững",
                    Color = "Màu tự nhiên",
                    Brand = "WoodenDream",
                    WarrantyPeriod = "2 năm",
                    Description = "Kiểu dáng có thiết kế độc đáo, mới lạ: Những đường veneer đan xéo được các thợ thủ công lành nghề khéo léo ghép 1 cách tỉ mỉ và chỉnh chu đã tạo ra điểm nhấn vô cùng độc đáo ở phần đầu và thân giường HOBRO.; Nghệ thuật hình học: Sử dụng veneer tràm đặc biệt với kỹ thuật dán cao cấp tạo ra hiệu ứng 3D hình học đối xứng nhau.; Hiệu ứng màu sắc 3D: Với các đường veneer đan xen và đối xứng nhau đã tạo nên một sắc màu tổng thể có thể thay đổi tùy vào mỗi góc nhìn khác nhau. Khi nhìn chính diện sẽ thấy sự đối lập hoàn toàn giữa 2 màu veneer nhưng nhìn chéo sẽ thấy màu sắc giữa 2 mảng đối xứng gần dần dần tương đồng.; Giường ngủ có đa dạng kích thước: Sản phẩm được phân phối với 2 kích thước phổ biến gồm: 1m6 và 1m8 rất dễ dàng để lựa chọn và phù hợp với những nội thất phòng ngủ khác, tô điểm không gian nhà bạn.; Giường sở hữu kết cấu chắc chắn: Khung giường được lắp khít nối với nhau bằng những thanh vạt bản to chắc chắn đặc biệt còn được bổ sung thêm khung và chân sắt ở chính giữa tăng độ chịu lực lên gấp 3 lần mà vẫn đảm bảo được tính thẩm mỹ của giường.; Tấm phản chịu lực cao: Với tấm phản plywood 12mm độ chịu lực tại 1 khu vực có diện tích 400 x 488 lên tới 175kg. Đặc biệt, tấm phản đã cải tiến kết cấu mới với 4 tấm xếp dọc làm tăng khả năng chịu lực lên gấp 4 lần so với kết cấu cũ, đảm bảo kết cấu chắc chắn khi dùng nệm trên 15cm.; Kết cấu đầu giường vô cùng bền chắc: Đầu giường được gia cố thêm thanh đỡ giúp cho kết cấu giường vô cùng chắc chắn giúp bạn yên tâm hơn trong các hoạt động mạnh.; Kết câu chân giường vững chãi: Chân giường bằng gỗ thông cùng logo thương hiệu độc quyền của Nội Thất THIÊN ÂN, chống hàng giả, đạo nhái, đảm bảo an toàn, kết cấu chắc chắn.; Chân giường gắn thêm nút nhựa giúp giảm việc trầy xước nền nhà.; Thiết kế tối ưu: Khung giường ngủ được tính toán kỹ thuật hợp lý,vừa vặn ôm khít với nệm, tránh tình trạng nệm bị trượt.; Độ hoàn thiện sản phẩm cao: Những đường veneer được ghép đan xéo chỉnh chu với từng chi tiết nhỏ tạo nên họa tiết hình thôi vô cùng độc đáo. Từng góc cạnh bo cong cách hoàn hảo vừa đảm bảo tính thẩm mĩ vừa mang đến sự an toàn cho người dùng. Chiều cao chân giường phù hợp - Dễ dàng vệ sinh: Chiều cao từ mặt đất lên tới khung giường 16cm, rất phù hợp cho các thiết bị robot hút bụi, máy hút bụi,... có thể dễ dàng vệ sinh.; ",
                    IsActive = true,
                    IsImport = true,
                    MainImg = "giuong_HBRO301.jpg",
                    CategoryId = 5
                },
                 // Bàn trang điểm: 13
                 new Product
                 {
                     Id = 13,
                     Name = "Bàn Trang Điểm Gỗ Đa Năng THIÊN ÂN VIENNA 202 Màu Tự Nhiên",
                     Price = 13990000,
                     Unit = "Cái",
                     Material = "",
                     Dimension = "Dài 100cm x Rộng 40cm x Cao 75cm",
                     Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                     Color = "Nâu",
                     Brand = "Woodland",
                     WarrantyPeriod = "2 năm",
                     Description = "",
                     IsActive = true,
                     IsImport = true,
                     MainImg = "btd_GoDN_VIENNA202_nau.jpg",
                     CategoryId = 6
                 },
            // Phòng Ăn:
            // Bàn ăn 14 15
            new Product
            {
                Id = 14,
                Name = "Bàn Ăn Gỗ Cao Su THIÊN ÂN OSLO 901",
                Price = 13990000,
                Unit = "Cái",
                Material = "",
                Dimension = "Dài 1m4 x Rộng 75cm x Cao 73cm",
                Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                Color = "Nâu",
                Brand = "Woodland",
                WarrantyPeriod = "2 năm",
                Description = "default.png",
                IsActive = true,
                IsImport = true,
                MainImg = "banan_GoCaoSu_OSLO901_nau.jpg",
                CategoryId = 11
            },
            new Product
            {
                Id = 15,
                Name = "Bàn Ăn Gỗ Cao Su Tự Nhiên THIÊN ÂN VLINE 601 1m6",
                Price = 13990000,
                Unit = "Cái",
                Material = "",
                Dimension = "Dài 160cm x Rộng 75cm x Cao 65cm",
                Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                Color = "Nâu",
                Brand = "Woodland",
                WarrantyPeriod = "2 năm",
                Description = "default.png",
                IsActive = true,
                IsImport = false,
                MainImg = "banan_GoCaoSuTN_VLINE601_1m6_nau.jpg",
                CategoryId = 11
            },
            // Ghế ăn 16 17
             new Product
             {
                 Id = 16,
                 Name = "Ghế Ăn Gỗ Cao Su Tự Nhiên MOHO ODESSA",
                 Price = 13990000,
                 Unit = "Cái",
                 Material = "",
                 Dimension = " Dài 43cm x Rộng 51cm x Cao đến phần ngồi/lưng tựa 43cm/92cm",
                 Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                 Color = "Nâu",
                 Brand = "Woodland",
                 WarrantyPeriod = "2 năm",
                 Description = "",
                 IsActive = true,
                 IsImport = true,
                 MainImg = "ghean_GoCaoSuTN_ODESSA_nau.jpg",
                 CategoryId = 12
             },
              new Product
              {
                  Id = 17,
                  Name = "Ghế Ăn Gỗ Cao Su Tự Nhiên MOHO VLINE 601",
                  Price = 13990000,
                  Unit = "Cái",
                  Material = "",
                  Dimension = "Dài 50cm x Rộng 56cm x Cao đến đệm ngồi/lưng tựa 37cm/70cm",
                  Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                  Color = "Nâu",
                  Brand = "Woodland",
                  WarrantyPeriod = "2 năm",
                  Description = "",
                  IsActive = true,
                  IsImport = false,
                  MainImg = "btd_GoDN_VIENNA202_nau.jpg",
                  CategoryId = 12
              },

            // Phòng làm việc:
            // Bàn làm việc id 18 19
             new Product
             {
                 Id = 18,
                 Name = "Bàn Làm Việc Gỗ MOHO FYN 601 Màu Tự Nhiên",
                 Price = 13990000,
                 Unit = "Cái",
                 Material = "",
                 Dimension = "Kích thước bàn: Dài 120cm x Rộng 60cm x Cao 74cm; Kích thước hộc kéo: Dài 23cm x Rộng 40cm x Cao 7cm",
                 Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                 Color = "Nâu",
                 Brand = "Woodland",
                 WarrantyPeriod = "2 năm",
                 Description = "",
                 IsActive = true,
                 IsImport = true,
                 MainImg = "blv_FYN601_tn.jpg",
                 CategoryId = 8
             },
              new Product
              {
                  Id = 19,
                  Name = "Bàn Máy Tính Gỗ MOHO WORKS 702",
                  Price = 13990000,
                  Unit = "Cái",
                  Material = "",
                  Dimension = "Bàn và chân bài: Dài 120cm x Rộng 62cm x Cao 72cm; Giá đỡ ổ điện: Dài 33cm x Rộng 9.74cm x Sâu 12cm; Trọng lượng chịu tải: 50~70 kg, tối đa 100kg khi phân phối đều khối lượng trên mặt bàn (1m4)",
                  Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                  Color = "Nâu",
                  Brand = "Woodland",
                  WarrantyPeriod = "2 năm",
                  Description = "", // này giữ nguyên
                  IsActive = true, // này giữ nguyên
                  IsImport = true, // này giữ nguyên
                  MainImg = "bmt_WORKS702_trang.jpg",
                  CategoryId = 8
              },
            // Ghế văn phòng id 20 21
             new Product
             {
                 Id = 20,
                 Name = "Ghế Xoay Văn Phòng Ngả Lưng MOHO JEFE 701",
                 Price = 2990000,
                 Unit = "Cái",
                 Material = "",
                 Dimension = "Dài 47cm x Rộng 65cm x Cao 108-126cm",
                 Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                 Color = "Đen",
                 Brand = "Woodland",
                 WarrantyPeriod = "2 năm",
                 Description = "",
                 IsActive = true,
                 IsImport = true,
                 MainImg = "ghexoayvp_JEFE701_den.2.jpg",
                 CategoryId = 9
             },

             new Product
             {
                 Id = 21,
                 Name = "Ghế Xoay Văn Phòng Tay Gập Thông Minh MOHO RIGA 701",
                 Price = 1690000,
                 Unit = "Cái",
                 Material = "",
                 Dimension = " Dài 52cm x Rộng 65cm x Cao 94-101cm",
                 Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                 Color = "Trắng",
                 Brand = "Woodland",
                 WarrantyPeriod = "2 năm",
                 Description = "",
                 IsActive = true,
                 IsImport = true,
                 MainImg = "ghexoayvp_RIGA701_trang.1.jpg",
                 CategoryId = 9
             }
            );

            // Seed ProductImage
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage
                {
                    Id = 1,
                    ProductId = 1,
                    ImgURL = "ae6e83fd-ee61-4762-9d90-0730ce25aad8.jpg"
                },
                new ProductImage
                {
                    Id = 2,
                    ProductId = 1,
                    ImgURL = "69ea8d94-4cca-4d08-88c5-9484c92a74e6.jpg"
                },
                new ProductImage
                {
                    Id = 3,
                    ProductId = 1,
                    ImgURL = "df5d0b56-abda-4e23-b13d-0993b643e5d7.jpg"
                },
                new ProductImage
                {
                    Id = 4,
                    ProductId = 1,
                    ImgURL = "deb1b150-83d9-4185-82de-2e7a06e317d2.jpg"
                },
                new ProductImage
                {
                    Id = 5,
                    ProductId = 1,
                    ImgURL = "5b24a4e5-22f4-4048-b7b7-7c07760443c5.jpg"
                },
                new ProductImage
                {
                    Id = 6,
                    ProductId = 1,
                    ImgURL = "12c850b8-97a2-4d43-9cd0-3d79c782f611.jpg"
                }

                );

            // Seed Order
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    CustomerPhoneNumber = "0123456789",
                    TotalPrice = 17980000,
                    TotalQuantity = 5,
                    Note = "Khách yêu cầu giao hàng nhanh.",
                    Address = "123 Đường Nguyễn Bỉnh Khiêm, Quận Bình Tân, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-1).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0001",
                    InvoiceDate = DateTime.Now.AddMonths(-1).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 2,
                    CustomerPhoneNumber = "0987654321",
                    TotalPrice = 24380000,
                    TotalQuantity = 7,
                    Note = "Khách yêu cầu giao hàng vào buổi tối.",
                    Address = "456 Đường XYZ, Quận ABC, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-2).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Bank_transfer,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0002",
                    InvoiceDate = DateTime.Now.AddMonths(-2).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 3,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 41800000,
                    TotalQuantity = 9,
                    Note = "Khách yêu cầu giao hàng vào cuối tuần.",
                    Address = "789 Đường DEF, Quận LMN, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-3).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Unpaid,
                    InvoiceNumber = "HD0003",
                    InvoiceDate = DateTime.Now.AddMonths(-3).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 4,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 79440000,
                    TotalQuantity = 6,
                    Note = "Yêu cầu giao hàng tận nhà.",
                    Address = "123 Đường Nguyễn Bỉnh Khiêm, Quận Bình Tân, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-4).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Pending,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Unpaid,
                    InvoiceNumber = "HD0004",
                    InvoiceDate = DateTime.Now.AddMonths(-4).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 5,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 111920000,
                    TotalQuantity = 8,
                    Note = "Khách yêu cầu giao hàng vào buổi sáng.",
                    Address = "456 Đường XYZ, Quận ABC, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-5).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0005",
                    InvoiceDate = DateTime.Now.AddMonths(-5).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 6,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 59910000,
                    TotalQuantity = 9,
                    Note = "Khách yêu cầu giao hàng vào cuối tuần.",
                    Address = "789 Đường DEF, Quận LMN, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-6).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0006",
                    InvoiceDate = DateTime.Now.AddMonths(-6).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 7,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 50420000,
                    TotalQuantity = 8,
                    Note = "Khách yêu cầu giao hàng nhanh.",
                    Address = "456 Đường XYZ, Quận ABC, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-7).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0007",
                    InvoiceDate = DateTime.Now.AddMonths(-7).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 8,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 29910000,
                    TotalQuantity = 9,
                    Note = "Khách yêu cầu giao hàng vào tận nhà.",
                    Address = "789 Đường DEF, Quận LMN, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-8).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0008",
                    InvoiceDate = DateTime.Now.AddMonths(-8).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 9,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 18540000,
                    TotalQuantity = 6,
                    Note = "Khách yêu cầu giao hàng nhanh.",
                    Address = "123 Đường Nguyễn Bỉnh Khiêm, Quận Bình Tân, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-9).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0009",
                    InvoiceDate = DateTime.Now.AddMonths(-9).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 10,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 48760000,
                    TotalQuantity = 4,
                    Note = "Khách yêu cầu giao hàng vào cuối tuần.",
                    Address = "123 Đường Nguyễn Bỉnh Khiêm, Quận Bình Tân, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-10).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0010",
                    InvoiceDate = DateTime.Now.AddMonths(-10).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 11,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 83940000,
                    TotalQuantity = 6,
                    Note = "Khách yêu cầu giao hàng nhanh.",
                    Address = "789 Đường DEF, Quận LMN, TP.HCM",
                    OrderDate = DateTime.Now.AddMonths(-11).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0011",
                    InvoiceDate = DateTime.Now.AddMonths(-11).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                },
                new Order
                {
                    Id = 12,
                    CustomerPhoneNumber = "0912345678",
                    TotalPrice = 97110000,
                    TotalQuantity = 9,
                    Note = "Khách yêu cầu giao hàng vào chiều thứ 7.",
                    Address = "108/45A/1 Trần Quang Diệu, P.An Thới, Q. Bình Thủy, TP. Cần Thơ",
                    OrderDate = DateTime.Now.AddMonths(-12).AddDays(-2),
                    OrderStatus = (int)ConstHelper.OrderStatus.Success,
                    PaymentMethod = (int)ConstHelper.PaymentMethod.Cash,
                    PaymentStatus = (int)ConstHelper.PaymentStatus.Paid,
                    InvoiceNumber = "HD0012",
                    InvoiceDate = DateTime.Now.AddMonths(-12).AddDays(-1),
                    CustomerId = "6",
                    SaleStaffId = "3",
                    ManagerId = "1"
                }




            );
            // Seed Order Detail
            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 2,
                    PriceAtOrder = 3990000
                },
                new OrderDetail
                {
                    Id = 2,
                    OrderId = 1, // Liên kết với Order Id
                    ProductId = 2, // Ví dụ: ID của sản phẩm khác
                    Quantity = 3,
                    PriceAtOrder = 13990000
                },

            // Seed OrderDetail for Order 2
                new OrderDetail
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 11, // Ví dụ sản phẩm khác
                    Quantity = 4,
                    PriceAtOrder = 12490000
                },
                new OrderDetail
                {
                    Id = 4,
                    OrderId = 2,
                    ProductId = 12, // Ví dụ sản phẩm khác
                    Quantity = 3,
                    PriceAtOrder = 11890000
                },

            // Seed OrderDetail for Order 3
                new OrderDetail
                {
                    Id = 5,
                    OrderId = 3,
                    ProductId = 3,
                    Quantity = 6,
                    PriceAtOrder = 1990000
                },
                new OrderDetail
                {
                    Id = 6,
                    OrderId = 3,
                    ProductId = 4,
                    Quantity = 3,
                    PriceAtOrder = 2190000
                },

            // Seed OrderDetail for Order 4
                new OrderDetail
                {
                    Id = 7,
                    OrderId = 4,
                    ProductId = 11,
                    Quantity = 3,
                    PriceAtOrder = 12490000
                },
                new OrderDetail
                {
                    Id = 8,
                    OrderId = 4,
                    ProductId = 13,
                    Quantity = 3,
                    PriceAtOrder = 13990000
                },

            // Seed OrderDetail for Order 5
                new OrderDetail
                {
                    Id = 9,
                    OrderId = 5,
                    ProductId = 14,
                    Quantity = 4,
                    PriceAtOrder = 13990000
                },
                new OrderDetail
                {
                    Id = 10,
                    OrderId = 5,
                    ProductId = 16,
                    Quantity = 4,
                    PriceAtOrder = 13990000
                },

            // Seed OrderDetail for Order 6
                new OrderDetail
                {
                    Id = 11,
                    OrderId = 6,
                    ProductId = 18,
                    Quantity = 3,
                    PriceAtOrder = 13990000 //41970000
                },
                new OrderDetail
                {
                    Id = 12,
                    OrderId = 6,
                    ProductId = 20,
                    Quantity = 6,
                    PriceAtOrder = 2990000 //17940000
                },

            // Seed OrderDetail for Order 7
                new OrderDetail
                {
                    Id = 13,
                    OrderId = 7,
                    ProductId = 21,
                    Quantity = 5,
                    PriceAtOrder = 1690000 //8450000
                },
                new OrderDetail
                {
                    Id = 14,
                    OrderId = 7,
                    ProductId = 19,
                    Quantity = 3,
                    PriceAtOrder = 13990000 //41970000
                },


            // Seed OrderDetail for Order 8
                new OrderDetail
                {
                    Id = 15,
                    OrderId = 8,
                    ProductId = 1,
                    Quantity = 6,
                    PriceAtOrder = 3990000
                },
                new OrderDetail
                {
                    Id = 16,
                    OrderId = 8,
                    ProductId = 3,
                    Quantity = 1,
                    PriceAtOrder = 1990000
                },

            // Seed OrderDetail for Order 9
                new OrderDetail
                {
                    Id = 17,
                    OrderId = 9,
                    ProductId = 4,
                    Quantity = 3,
                    PriceAtOrder = 2190000
                },
                new OrderDetail
                {
                    Id = 18,
                    OrderId = 9,
                    ProductId = 3,
                    Quantity = 1,
                    PriceAtOrder = 3990000
                },

            // Seed OrderDetail for Order 10
                new OrderDetail
                {
                    Id = 19,
                    OrderId = 10,
                    ProductId = 11,
                    Quantity = 2,
                    PriceAtOrder = 12490000
                },
                new OrderDetail
                {
                    Id = 20,
                    OrderId = 10,
                    ProductId = 12,
                    Quantity = 2,
                    PriceAtOrder = 11890000
                },

            // Seed OrderDetail for Order 11
                new OrderDetail
                {
                    Id = 21,
                    OrderId = 11,
                    ProductId = 14,
                    Quantity = 2,
                    PriceAtOrder = 13990000
                },
                new OrderDetail
                {
                    Id = 22,
                    OrderId = 11,
                    ProductId = 16,
                    Quantity = 4,
                    PriceAtOrder = 13990000
                },

            // Seed OrderDetail for Order 12
                new OrderDetail
                {
                    Id = 23,
                    OrderId = 12,
                    ProductId = 21,
                    Quantity = 2,
                    PriceAtOrder = 1690000 //3380000
                },
                new OrderDetail
                {
                    Id = 24,
                    OrderId = 12,
                    ProductId = 19,
                    Quantity = 2,
                    PriceAtOrder = 13990000 //27980000
                },
                new OrderDetail
                {
                    Id = 25,
                    OrderId = 12,
                    ProductId = 13,
                    Quantity = 3,
                    PriceAtOrder = 13990000 //41970000
                },
                new OrderDetail
                {
                    Id = 26,
                    OrderId = 12,
                    ProductId = 12,
                    Quantity = 2,
                    PriceAtOrder = 11890000 
                }
            );

            // Seeder cho bảng Shipment
            modelBuilder.Entity<Shipment>().HasData(
                new Shipment
                {
                    Id = 1,
                    ReceiptDate = new DateTime(2024, 11, 18),
                    TotalPrice = 340000000, // Tổng tiền của lô hàng
                    TotalQuantity = 60, // Tổng số lượng
                    Note = "Phiếu nhập hàng đợt 1",
                    SupplierId = 1, // Mã nhà cung cấp
                    ManagerId = "1" // Mã của Manager
                }
            );

            // Seeder cho bảng Goods
            modelBuilder.Entity<Goods>().HasData(
                new Goods
                {
                    Id = 1,
                    Quantity = 10,
                    ImportPrice = 1000000,
                    TotalPrice = 10000000,
                    ProductId = 3,
                    ShipmentId = 1
                },
                new Goods
                {
                    Id = 2,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 11,
                    ShipmentId = 1 
                },
                new Goods
                {
                    Id = 3,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 2,
                    ShipmentId = 1
                },
                new Goods
                {
                    Id = 4,
                    Quantity = 10,
                    ImportPrice = 1000000,
                    TotalPrice = 10000000,
                    ProductId = 4,
                    ShipmentId = 1
                },
                new Goods
                {
                    Id = 5,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 12,
                    ShipmentId = 1 
                },
                new Goods
                {
                    Id = 6,
                    Quantity = 10,
                    ImportPrice = 2000000,
                    TotalPrice = 20000000,
                    ProductId = 1,
                    ShipmentId = 1
                },
                new Goods
                {
                    Id = 7,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 13,
                    ShipmentId = 1
                }, 
                new Goods
                {
                    Id = 8,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 14,
                    ShipmentId = 1
                },
                new Goods
                {
                    Id = 9,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 16,
                    ShipmentId = 1
                },       
                new Goods
                {
                    Id = 10,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 17,
                    ShipmentId = 1
                },       
                new Goods
                {
                    Id = 11,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 18,
                    ShipmentId = 1
                },
                new Goods
                {
                    Id = 12,
                    Quantity = 10,
                    ImportPrice = 10000000,
                    TotalPrice = 100000000,
                    ProductId = 19,
                    ShipmentId = 1
                },   
                new Goods
                {
                    Id = 13,
                    Quantity = 10,
                    ImportPrice = 1200000,
                    TotalPrice = 12000000,
                    ProductId = 20,
                    ShipmentId = 1
                },   new Goods
                {
                    Id = 14,
                    Quantity = 10,
                    ImportPrice = 1200000,
                    TotalPrice = 12000000,
                    ProductId = 21,
                    ShipmentId = 1
                }

            );

        }
    }
}
