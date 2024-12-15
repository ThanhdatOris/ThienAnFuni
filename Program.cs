using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;
using ThienAnFuni.Helpers;
using ThienAnFuni.Services; 
using ThienAnFuni.Configurations;

var builder = WebApplication.CreateBuilder(args);

// SendMail
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<ThienAnFuni.Services.IEmailSender, EmailSender>();

// Load configurations from appsettings.json and appsettings.Local.json
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<TAF_DbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<TAF_DbContext>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình Identity và cấu hình lại các yêu cầu mật khẩu
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Tắt các yêu cầu mật khẩu mặc định
    options.Password.RequireDigit = false;  // Không yêu cầu chữ số
    options.Password.RequireLowercase = false;  // Không yêu cầu chữ cái viết thường
    options.Password.RequireUppercase = false;  // Không yêu cầu chữ cái viết hoa
    options.Password.RequireNonAlphanumeric = false;  // Không yêu cầu ký tự không phải chữ cái
    options.Password.RequiredLength = 6;  // Độ dài mật khẩu tối thiểu
    options.Password.RequiredUniqueChars = 1;  // Số ký tự duy nhất tối thiểu
})
.AddEntityFrameworkStores<TAF_DbContext>()  
.AddDefaultTokenProviders();

// Không có quyền thì bị đá vào Controller Account - Action:AccessDenied
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";  // Sẽ đá về UI này khi không đủ quyền truy cập
});

// Add session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Cấu hình middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Hiển thị trang lỗi chi tiết trong môi trường phát triển
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Hiển thị trang lỗi chung trong môi trường sản xuất
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Session

app.UseAuthorization();

app.UseWebSockets(); //  UseWebSockets

// Role sẽ được tạo khi app nó chạy nha bà con
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleInitializer.SeedRolesAsync(roleManager);

    // Gọi phương thức seed
    await SeedRolesAndUsers(userManager, roleManager);
}

// Tham số động
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task SeedRolesAndUsers(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    // Tạo các roles nếu chưa có (TEst lại hong cần thì bỏ)
    if (!await roleManager.RoleExistsAsync(ConstHelper.RoleManager))
    {
        await roleManager.CreateAsync(new IdentityRole(ConstHelper.RoleManager));
    }
    if (!await roleManager.RoleExistsAsync(ConstHelper.RoleSaleStaff))
    {
        await roleManager.CreateAsync(new IdentityRole(ConstHelper.RoleSaleStaff));
    }
    if (!await roleManager.RoleExistsAsync(ConstHelper.RoleCustomer))
    {
        await roleManager.CreateAsync(new IdentityRole(ConstHelper.RoleCustomer));
    }

    // Tạo người dùng và gán roles
    var managerUser = await userManager.FindByNameAsync("sinoo");
    if (managerUser == null)
    {
        managerUser = new User { UserName = "sinoo", FullName = "Sinoo" };
        var createResult = await userManager.CreateAsync(managerUser, "123456");
    }
    await userManager.AddToRoleAsync(managerUser, ConstHelper.RoleManager);

    var customerUser = await userManager.FindByNameAsync("teoemcus");
    //if (customerUser == null)
    //{
    //    customerUser = new User { UserName = "teoemcus", FullName = "Sinoo" };
    //    var createResult = await userManager.CreateAsync(customerUser, "123456");
    //}
    await userManager.AddToRoleAsync(customerUser, ConstHelper.RoleCustomer);

    var customerUser2 = await userManager.FindByNameAsync("hongdaocus");
    await userManager.AddToRoleAsync(customerUser2, ConstHelper.RoleCustomer);

    var customerUser3 = await userManager.FindByNameAsync("teoemcus");
    await userManager.AddToRoleAsync(customerUser3, ConstHelper.RoleCustomer);

    var saleStaffUser = await userManager.FindByNameAsync("tramanh");
    //if (saleStaffUser == null)
    //{
    //    saleStaffUser = new User { UserName = "tramanh", FullName = "Huynh Thị Trâm Anh" };
    //    var createResult = await userManager.CreateAsync(saleStaffUser, "123456");
    //}
    await userManager.AddToRoleAsync(saleStaffUser, ConstHelper.RoleSaleStaff);

    var saleStaffUser2 = await userManager.FindByNameAsync("vanminh");
    await userManager.AddToRoleAsync(saleStaffUser2, ConstHelper.RoleSaleStaff);
}