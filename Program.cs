using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;

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

// Role sẽ được tạo khi app nó chạy nha bà con
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleInitializer.SeedRolesAsync(roleManager);
}

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

// Tham số động
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Add this route for the admin path, tham số cố định
//app.MapControllerRoute(
//    name: "admin",
//    pattern: "admin",
//    defaults: new { controller = "Managers", action = "Index" });

app.Run();
