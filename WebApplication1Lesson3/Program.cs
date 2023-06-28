using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;     // ConfigurationBuilder�|�Ψ�o�өR�W�Ŷ�
using WebApplication1Lesson3.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

////�@�k�@�G
//builder.Services.AddDbContext<MvcUserDbContext>(options => options.UseSqlServer("Server=.\\sqlexpress;Database=MVC_UserDB;Trusted_Connection=True;MultipleActiveResultSets=true"));
//// �o�̻ݭn�s�W��өR�W�Ŷ��A�ШϥΡu��ܥi�઺�ץ��v���t�Φۤv�[�W�C
//�@�k�G�G Ū���]�w�ɪ����e
var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
IConfiguration config = configurationBuilder.Build();   // ConfigurationBuilder�|�Ψ� Microsoft.Extensions.Configuration�R�W�Ŷ�
string DBconnectionString = config["ConnectionStrings:DefaultConnection"];  // appsettings.josn�ɸ̭����u��Ʈw�s���r��v
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
