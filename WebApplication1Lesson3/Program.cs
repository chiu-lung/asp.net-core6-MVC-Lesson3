using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;     // ConfigurationBuilder會用到這個命名空間
using WebApplication1Lesson3.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

////作法一：
//builder.Services.AddDbContext<MvcUserDbContext>(options => options.UseSqlServer("Server=.\\sqlexpress;Database=MVC_UserDB;Trusted_Connection=True;MultipleActiveResultSets=true"));
//// 這裡需要新增兩個命名空間，請使用「顯示可能的修正」讓系統自己加上。
//作法二： 讀取設定檔的內容
var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
IConfiguration config = configurationBuilder.Build();   // ConfigurationBuilder會用到 Microsoft.Extensions.Configuration命名空間
string DBconnectionString = config["ConnectionStrings:DefaultConnection"];  // appsettings.josn檔裡面的「資料庫連結字串」
builder.Services.AddDbContext<MvcUserDbContext>(options => options.UseSqlServer(DBconnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
