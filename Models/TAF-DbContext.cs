using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Helpers;

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
            modelBuilder.Entity<Manager>().HasData(
            new Manager
            {
                Id = 1,
                Username = "sinoo",
                Password = PasswordHelper.HashPassword("123456"),
                FullName = "Sinoo",
                PhoneNumber = "0123456789",
                Address = "123 Main St",
                Gender = "Nam",
                DateOfBirth = new DateTime(2004, 5, 12),
                CitizenId = "123456789"
            },
            new Manager
            {
                Id = 2,
                Username = "vyvy",
                FullName = "Nu Thao Vy",
                Password = PasswordHelper.HashPassword("123456"),
                PhoneNumber = "0987654321",
                Address = "456 Another St",
                Gender = "Nữ",
                DateOfBirth = new DateTime(2004, 8, 25),
                CitizenId = "987654321"
            }
                );

            modelBuilder.Entity<SaleStaff>().HasData(
            new SaleStaff
            {
                Id = 3,
                Username = "tramanh",
                Password = PasswordHelper.HashPassword("123456"),
                FullName = "Huynh Thị Trâm Anh",
                PhoneNumber = "0123456789",
                Address = "123 Sóc Trăng",
                Gender = "Nữ",
                DateOfBirth = new DateTime(1995, 5, 12),
                CitizenId = "999456789",
                IssuingDate = new DateTime(2015, 5, 12),
                IssuingPlace = "Sóc Trăng"

            },
            new SaleStaff
            {
                Id = 4,
                Username = "vanminh",
                FullName = "Nguyễn Văn Minh",
                Password = PasswordHelper.HashPassword("123456"),
                PhoneNumber = "0987654321",
                Address = "456 An Giang",
                Gender = "Nam",
                DateOfBirth = new DateTime(1990, 8, 25),
                CitizenId = "9876234521",
                IssuingDate = new DateTime(2015, 8, 25),
                IssuingPlace = "An Giang"
            }
                );

            modelBuilder.Entity<Customer>().HasData(
               new Customer
               {
                   Id = 5,
                   Username = "hongdaocus",
                   Password = PasswordHelper.HashPassword("123456"),
                   FullName = "Huỳnh Hồng Đào",
                   PhoneNumber = "012322289",
                   Address = "123 Kiên Giang",
                   Gender = "Nữ",
                   DateOfBirth = new DateTime(1995, 5, 12),


               },
               new Customer
               {
                   Id = 6,
                   Username = "teoemcus",
                   FullName = "Nguyễn Văn Tèo",
                   Password = PasswordHelper.HashPassword("123456"),
                   PhoneNumber = "0987111321",
                   Address = "456 Hậu Giang",
                   Gender = "Nam",
                   DateOfBirth = new DateTime(1990, 8, 25),
               }
               );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Phòng khách", IsActive = true },
                new Category { Id = 2, Name = "Ghế sofa", ParentId = 1, IsActive = true },
                new Category { Id = 3, Name = "Bàn sofa", ParentId = 1, IsActive = true },

                new Category { Id = 4, Name = "Phòng ngủ", IsActive = true },
                new Category { Id = 5, Name = "Giường", ParentId = 4, IsActive = true },
                new Category { Id = 6, Name = "Bàn Trang Điểm", ParentId = 4, IsActive = true },

                new Category { Id = 7, Name = "Phòng làm việc", IsActive = true },
                new Category { Id = 8, Name = "Bàn làm việc", ParentId = 7, IsActive = true },
                new Category { Id = 9, Name = "Ghế văn phòng", ParentId = 7, IsActive = true }


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
                    Name = "Ghế Đôn Sofa HOBRO 301",
                    Price = 3990000,
                    Unit = "Cái",
                    Material = "Vải Polyester: Ghế có màu xám đậm nâng tầm thẩm mỹ và không gian nội thất phòng khách của bạn; Gỗ cao su tự nhiên: Nội thất Thiên Ân sử dụng chất liệu gỗ cao su giúp ghế sofa gỗ HOBRO có khả năng chịu lực tốt và độ bền cao.",
                    Dimension = "900 x 900 x 400 cm",
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Màu nâu",
                    Brand = "Sofaland",
                    WarrantyPeriod = "1 năm",
                    Description = "Dễ dàng vệ sinh: Nệm Sofa bằng vải polyester chống bụi, kháng ẩm mốc và có thể dễ dàng tháo bọc nệm để vệ sinh. Đệm ghế sofa có màu xám tạo nên vẻ hiện đại nhưng không kém phần sang trọng cho căn phòng của bạn.; Ghế Sofa đơn có kích thước tiêu chuẩn: Ghế Đôn có kích thước rộng rãi , với thiết kế tinh giản giúp chúng gọn nhẹ và tiết kiệm được diện tích căn phòng, bạn cũng có thể ngồi một cách thoải mái và bài trí trong nhiều không gian khác nhau.; Mút đệm cao cấp, dày dặn và chống cháy: Độ bền cao, chống xẹp lún và đạt tiêu chuẩn của Mỹ về khả năng chống cháy.; Độ cao chân ghế vừa phải: Chân ghế có chiều cao hợp lý nên Robot hút bụi có thể lau dọn phía dưới sàn một cách dễ dàng; Chân ghế cao su: Sử dụng chất liệu gỗ cao su tự nhiên với khả năng chịu lực cao.",
                    IsActive = true,
                    IsImport = false,
                    MainImg = "gheSofa_HOBRO301trai.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 2,
                    Name = "Ghế Sofa HOBRO 301 (180)",
                    Price = 13990000,
                    Unit = "Cái",
                    Material = "Vải Polyester: Ghế có màu xám đậm nâng tầm thẩm mỹ và không gian nội thất phòng khách nhà bạn.; Gỗ cao su tự nhiên: Nội thất Thiên Ân sử dụng chất liệu gỗ cao su giúp ghế sofa gỗ VLINE có khả năng chịu lực tốt và độ bền cao.",
                    Dimension = "900 x 1800 x 700 cm",
                    Standard = "(*) Tiêu chuẩn California Air Resources Board xuất khẩu Mỹ, đảm bảo gỗ không độc hại, an toàn cho sức khỏe",
                    Color = "Nâu",
                    Brand = "Woodland",
                    WarrantyPeriod = "2 năm",
                    Description = "Các điểm tựa trên ghế có thiết kế bo tròn: Phần đệm tay và đệm lưng của ghế sofa được thiết kế bo tròn mềm mại, tạo cảm giác thoải mái cho người dùng mỗi khi ngồi. Vừa vặn tiếp xúc, nâng đỡ khung xương cơ thể, tạo cảm giác thoải mái mỗi khi ngồi hoặc nằm.; Đệm ghế có khóa dán chống trượt: ",
                    IsActive = true,
                    IsImport = false,
                    MainImg = "gheSofa_CaoSuTN_VLINE601phai_nau.jpg",
                    CategoryId = 2
                },

                // Phòng ngủ: Giường
                new Product
                {
                    Id = 11,
                    Name = "Giường Ngủ Gỗ Tự Nhiên Mây Mắt Cáo THIÊN ÂN FIJI 401",
                    Price = 12490000,
                    Unit = "Cái",
                    Material = "Gỗ tràm: Giường ngủ được chế tác từ chất liệu gỗ tràm giúp gia tăng khả năng chịu lực tốt và độ bền cao, đảm bảo sự vững chắc và chống cong vênh, mối mọt trong suốt thời gian sử dụng.; Mây tre đan: Đầu giường sử dụng bằng mây mắt cáo tự nhiên chất lượng cao, chống mối mọt, côn trùng được làm thủ công từ nghệ nhân và thợ lành nghề giúp tạo cảm giác thoải mái, gần gũi với thiên nhiên cho căn phòng của bạn.",
                    Dimension = " Dài 210cm x Rộng 167/187cm x Cao 90cm (1m6)",
                    Standard = "(*) Tiêu chuẩn E1 Châu Âu, đảm bảo không phát thải khí độc hại, thân thiện với môi trường",
                    Color = "Nâu",
                    Brand = "Everwood",
                    WarrantyPeriod = "3 năm",
                    Description = "Thiết kế giường ngủ truyền thống và sang trọng: Giường FIJI được thiết kế tối giản kết hợp với vẻ đẹp của chất liệu mây đan tự nhiên chắc chắn sẽ thổi thêm nét xanh và mang đến cho không gian phòng ngủ của bạn vẻ đẹp Á Đông mới. Bạn có thể cảm nhận sự tươi mới như đang thong dong giữa một cánh rừng xanh mát bất tận. Ngoài ra, giường FIJI còn là một lựa chọn tuyệt vời cho những ai yêu thích sự tự nhiên của phong cách Tropical. ; Giường ngủ gỗ có kích thước 1m6/1m8 - Phổ biến với người dùng Việt: Giường ngủ gỗ FIJI với kích thước chiều dài 1m6/1m8 theo tiêu chuẩn gia đình và khách sạn, đây là kích thước thông dụng nhất hiện nay phù hợp với nhiều không gian.;  Giường ngủ mang thiết kế bo tròn: Thiết kế bo tròn các góc cạnh vừa tạo đường nét mềm mại, sự tinh tế trong thiết kế. Đặc biệt hạn chế thương tích khi va đập, rất an toàn cho gia đình có trẻ nhỏ và người lớn tuổi.; Kết cấu đầu giường đầu giường chắc chắn, bền bỉ: Với kết cấu đầu giường mới đã được cải tiến thêm tấm gỗ MDF hỗ trợ giúp độ chịu lực tăng lên gấp nhiều lần, đồng thời cũng giúp tăng tuổi thọ của mây và bền bỉ hơn với thời gian.; Bộ khung kim loại chắc chắn: Khung đỡ cố định được lắp ghép khít nối với nhau cùng thanh chịu lực ở vạt giường, tăng khả năng chịu lực cho giường ngủ FIJI.; Tấm phản chắc chắn: Giường ngủ gỗ FIJI có thiết kế nguyên tấm, phản giường giúp kết cấu của toàn bộ giường trở nên vững chắc hơn, có thể chịu lực các tấm nệm dày lên đến 30cm. Mây tự nhiên - Nguyên liệu truyền thống từ làng nghề Việt: Với ưu điểm là vẻ đẹp mộc mạc, tự nhiên, thân thiện với môi trường, những nhà thiết kế của THIÊN ÂN đã tinh tế kết hợp chất liệu gỗ và mây tự nhiên họa tiết mắt cáo để mang đến cho không gian sống vẻ đẹp hài hòa, ấm áp.; Dễ dàng vệ sinh sạch sẽ: Chiều cao giường ngủ hợp lý phù hợp cho các thiết bị robot hút bụi, máy hút bụi,... có thể dễ dàng vệ sinh. ; Chân giường chịu lực cao: Chân giường bằng gỗ tràm tự nhiên cùng logo thương hiệu độc quyền của nội thất THIÊN ÂN, chống hàng giả, đạo nhái, đảm bảo an toàn, kết cấu chắc chắn.",
                    IsActive = true,
                    IsImport = false,
                    MainImg = "giuong_FIJI401.1.jpg",
                    CategoryId = 5
                },
                new Product
                {
                    Id = 12,
                    Name = "Giường Ngủ Gỗ Cao Su THIÊN ÂN HOBRO 301",
                    Price = 11890000,
                    Unit = "Cái",
                    Material = "Gỗ cao su tự nhiên: Sử dụng chất liệu gỗ cao su giúp giuờng ngủ có khả năng chịu lực tốt và độ bền cao.; Gỗ công nghiệp Plywood: Tấm phảm sử dụng chất liệu Plywood 12mm theo tiêu chuẩn CARBP2 vừa thân thiện với môi trường, đảm bảo sức khỏe và đặc biệt độ chịu lực tại 1 khu vực với diện tích 400 x 488mm lên tới 175kg khi dùng nệm trên 15cm.",
                    Dimension = "Dài 210cm x Rộng 171/ 191 cm; Cao đến đầu giường 90 cm; Gầm giường cao 16cm (1m8)",
                    Standard = "(*) Chứng nhận FSC từ tổ chức quốc tế, đảm bảo nguồn gốc gỗ bền vững",
                    Color = "Màu tự nhiên",
                    Brand = "WoodenDream",
                    WarrantyPeriod = "2 năm",
                    Description = "Kiểu dáng có thiết kế độc đáo, mới lạ: Những đường veneer đan xéo được các thợ thủ công lành nghề khéo léo ghép 1 cách tỉ mỉ và chỉnh chu đã tạo ra điểm nhấn vô cùng độc đáo ở phần đầu và thân giường HOBRO.; Nghệ thuật hình học: Sử dụng veneer tràm đặc biệt với kỹ thuật dán cao cấp tạo ra hiệu ứng 3D hình học đối xứng nhau.; Hiệu ứng màu sắc 3D: Với các đường veneer đan xen và đối xứng nhau đã tạo nên một sắc màu tổng thể có thể thay đổi tùy vào mỗi góc nhìn khác nhau. Khi nhìn chính diện sẽ thấy sự đối lập hoàn toàn giữa 2 màu veneer nhưng nhìn chéo sẽ thấy màu sắc giữa 2 mảng đối xứng gần dần dần tương đồng.; Giường ngủ có đa dạng kích thước: Sản phẩm được phân phối với 2 kích thước phổ biến gồm: 1m6 và 1m8 rất dễ dàng để lựa chọn và phù hợp với những nội thất phòng ngủ khác, tô điểm không gian nhà bạn.; Giường sở hữu kết cấu chắc chắn: Khung giường được lắp khít nối với nhau bằng những thanh vạt bản to chắc chắn đặc biệt còn được bổ sung thêm khung và chân sắt ở chính giữa tăng độ chịu lực lên gấp 3 lần mà vẫn đảm bảo được tính thẩm mỹ của giường.; Tấm phản chịu lực cao: Với tấm phản plywood 12mm độ chịu lực tại 1 khu vực có diện tích 400 x 488 lên tới 175kg. Đặc biệt, tấm phản đã cải tiến kết cấu mới với 4 tấm xếp dọc làm tăng khả năng chịu lực lên gấp 4 lần so với kết cấu cũ, đảm bảo kết cấu chắc chắn khi dùng nệm trên 15cm.; Kết cấu đầu giường vô cùng bền chắc: Đầu giường được gia cố thêm thanh đỡ giúp cho kết cấu giường vô cùng chắc chắn giúp bạn yên tâm hơn trong các hoạt động mạnh.; Kết câu chân giường vững chãi: Chân giường bằng gỗ thông cùng logo thương hiệu độc quyền của Nội Thất THIÊN ÂN, chống hàng giả, đạo nhái, đảm bảo an toàn, kết cấu chắc chắn.; Chân giường gắn thêm nút nhựa giúp giảm việc trầy xước nền nhà.; Thiết kế tối ưu: Khung giường ngủ được tính toán kỹ thuật hợp lý,vừa vặn ôm khít với nệm, tránh tình trạng nệm bị trượt.; Độ hoàn thiện sản phẩm cao: Những đường veneer được ghép đan xéo chỉnh chu với từng chi tiết nhỏ tạo nên họa tiết hình thôi vô cùng độc đáo. Từng góc cạnh bo cong cách hoàn hảo vừa đảm bảo tính thẩm mĩ vừa mang đến sự an toàn cho người dùng. Chiều cao chân giường phù hợp - Dễ dàng vệ sinh: Chiều cao từ mặt đất lên tới khung giường 16cm, rất phù hợp cho các thiết bị robot hút bụi, máy hút bụi,... có thể dễ dàng vệ sinh.; ",
                    IsImport = false,
                    IsActive = true,
                    MainImg = "giuong_HBRO301.jpg",
                    CategoryId = 5
                }

            );
        }

    }
}
