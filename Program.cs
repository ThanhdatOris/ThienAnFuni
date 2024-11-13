using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;
using ThienAnFuni.Helpers;
var builder = WebApplication.CreateBuilder(args);

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

// IdentityRole config
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TAF_DbContext>()
    .AddDefaultTokenProviders();

// Không có quyền thì bị đá vào Controller Account - Action:AccessDenied
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";  // Trang xử lý khi bị từ chối quyền truy cập
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
    // Tạo các roles nếu chưa có
    if (!await roleManager.RoleExistsAsync(ConstHelper.RoleManager))
    {
        await roleManager.CreateAsync(new IdentityRole(ConstHelper.RoleManager));
    }
    if (!await roleManager.RoleExistsAsync(ConstHelper.RoleSaleStaff))
    {
        await roleManager.CreateAsync(new IdentityRole(ConstHelper.RoleSaleStaff));
    }

    // Tạo người dùng và gán roles
    var managerUser = await userManager.FindByNameAsync("sinoo");
    if (managerUser == null)
    {
        managerUser = new User { UserName = "sinoo", FullName = "Sinoo" };
        var createResult = await userManager.CreateAsync(managerUser, "123456");
        if (createResult.Succeeded)
        {
        }
    }
            await userManager.AddToRoleAsync(managerUser, ConstHelper.RoleManager);

    var saleStaffUser = await userManager.FindByNameAsync("tramanh");
    if (saleStaffUser == null)
    {
        saleStaffUser = new User { UserName = "tramanh", FullName = "Huynh Thị Trâm Anh" };
        var createResult = await userManager.CreateAsync(saleStaffUser, "123456");
        if (createResult.Succeeded)
        {
        }
    }
            await userManager.AddToRoleAsync(saleStaffUser, ConstHelper.RoleSaleStaff);
}