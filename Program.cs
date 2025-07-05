using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThienAnFuni.Models;
using ThienAnFuni.Helpers;
using ThienAnFuni.Services;
using ThienAnFuni.Configurations;
using ThienAnFuni.Models.Momo;
using ThienAnFuni.Services.Momo;
using ThienAnFuni.Models.VNPay;
using ThienAnFuni.Services.VNPay;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình MomoSettings
builder.Services.Configure<MomoSettings>(builder.Configuration.GetSection("MomoSettings"));

// Cấu hình VNPaySettings
builder.Services.Configure<VNPaySettings>(builder.Configuration.GetSection("VNPaySettings"));

// Đăng ký HttpClientFactory và MomoService
builder.Services.AddHttpClient("MomoClient", client =>
{
    // Cấu hình chung cho HttpClient nếu cần, ví dụ Timeout
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IMomoService, MomoService>();
builder.Services.AddScoped<IVNPayService, VNPayService>();

// Cấu hình Session (nếu bạn dùng Session để lưu OrderId, RequestId)
builder.Services.AddDistributedMemoryCache(); // Cần thiết cho session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Thời gian timeout của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Đánh dấu cookie session là cần thiết
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("GoogleLogin");
        options.ClientId = googleAuthNSection["GOOGLE_CLIENT_ID"];
        options.ClientSecret = googleAuthNSection["GOOGLE_CLIENT_SECRET"];
        options.CallbackPath = googleAuthNSection["CallbackPath"];
        //options.CallbackPath = "/connect/google/check"; 
        options.Scope.Add("profile"); // Cần scope này để lấy ảnh
        options.Scope.Add("email");
        options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
        options.SaveTokens = true;
    });


// SendMail
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<ThienAnFuni.Services.IEmailSender, EmailSender>();

// Load configurations from appsettings.json and appsettings.Local.json
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);


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
    options.SignIn.RequireConfirmedEmail = false; // Không yêu cầu xác thực email
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


// ----------------------------- BUILD APP --------------------------------
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
app.UseStaticFiles(); // cho phép truy cập tệp tĩnh

app.UseSession(); // Session

app.UseAuthorization();

app.UseWebSockets(); //  UseWebSockets

// Error 404
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Account/Error404");
    }
});

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