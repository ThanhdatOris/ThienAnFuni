# Account:
- `Admin`: 
  - `Username`: `sinoo`
  - `Password`: `123456`

- `SaleStaff 1`:
  - `Username`: `tramanh`
  - `Password`: `123456`

# Lần pull đầu tiên
```
- Tạo database thienanFuni
- mở file appsettings.json đổi tên server
```

Dưới đây là hướng dẫn chi tiết từ đầu để cấu hình chức năng đăng ký, đăng nhập và phân quyền trong dự án ASP.NET Core 8.0 MVC với ba loại người dùng (Manager, SaleStaff, Customer):

### 1. Tạo Dự Án ASP.NET Core 8.0 MVC

### 2. Cài Đặt Thư Viện Identity

- **Identity** giúp bạn quản lý xác thực, phân quyền và đăng nhập/đăng xuất một cách dễ dàng.

- Mở **Package Manager Console** và chạy lệnh sau để cài đặt thư viện `Microsoft.AspNetCore.Identity.EntityFrameworkCore`:
   ```powershell
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

### 3. Cấu Hình Database Context

- Tạo **ApplicationDbContext** kế thừa từ `IdentityDbContext` để Identity quản lý cơ sở dữ liệu.

1. **Tạo lớp ApplicationDbContext**:
   ```csharp
   using Microsoft.AspNetCore.Identity;
   using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore;

   public class ApplicationDbContext : IdentityDbContext<User>
   {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

       public DbSet<Manager> Managers { get; set; }
       public DbSet<SaleStaff> SaleStaffs { get; set; }
       public DbSet<Customer> Customers { get; set; }
   }
   ```

2. **Cấu hình chuỗi kết nối trong `appsettings.json`**:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabaseName;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Thêm ApplicationDbContext vào `Program.cs`**:
   ```csharp
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

   builder.Services.AddIdentity<User, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();

   builder.Services.AddControllersWithViews();
   ```

### 4. Cập Nhật Model Người Dùng Với Identity

- Để phù hợp với cấu trúc của Identity, bạn cập nhật các model `User`, `Manager`, `SaleStaff`, và `Customer` theo định nghĩa của bạn ở trên.

### 5. Thiết Lập Bảng Role Cho Người Dùng

- Tạo một `RoleInitializer` để thêm các role cơ bản vào hệ thống khi chạy lần đầu:

   ```csharp
   using Microsoft.AspNetCore.Identity;
   using System.Threading.Tasks;

   public class RoleInitializer
   {
       public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
       {
           if (!await roleManager.RoleExistsAsync("Manager"))
           {
               await roleManager.CreateAsync(new IdentityRole("Manager"));
           }
           if (!await roleManager.RoleExistsAsync("SaleStaff"))
           {
               await roleManager.CreateAsync(new IdentityRole("SaleStaff"));
           }
           if (!await roleManager.RoleExistsAsync("Customer"))
           {
               await roleManager.CreateAsync(new IdentityRole("Customer"));
           }
       }
   }
   ```

- **Thêm lệnh gọi SeedRoles** trong `Program.cs`:
   ```csharp
   using Microsoft.AspNetCore.Identity;

   var app = builder.Build();

   using (var scope = app.Services.CreateScope())
   {
       var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
       await RoleInitializer.SeedRolesAsync(roleManager);
   }
   ```

### 6. Tạo AccountController

Tạo `AccountController` để quản lý các chức năng đăng ký, đăng nhập, và đăng xuất.

1. **Đăng ký người dùng**:
  

2. **Đăng nhập người dùng**:
  

3. **Đăng xuất**:
 

### 7. Phân Quyền Cho Các Controller

- Dùng `[Authorize(Roles = "RoleName")]` để phân quyền. Ví dụ:
   ```csharp
   [Authorize(Roles = "Manager")]
   public class AdminController : Controller
   {
       // Chỉ có Manager mới truy cập được
   }

   [Authorize(Roles = "SaleStaff")]
   public class SaleController : Controller
   {
       // Chỉ có SaleStaff mới truy cập được
   }
   ```

### 8. Migrate và Chạy Ứng Dụng

- Chạy các lệnh migration để tạo bảng trong database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

- Sau khi thiết lập xong, bạn có thể chạy ứng dụng. Các tài khoản Manager, SaleStaff, và Customer có thể đăng nhập và truy cập vào các trang được phân quyền riêng biệt.