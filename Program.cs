
using Projeto_Trainee_Hub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddControllersWithViews();

// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession();
// builder.Services.AddHttpContextAccessor();

// builder.Services.AddScoped<UsuariosRepository>();

// builder.Services.AddDbContext<MasterContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddScoped<UsuariosRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseSession();
// app.UseHttpsRedirection();
app.UseRouting();

// app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Encarregado}/{action=SSModulos}/{id?}")
    .WithStaticAssets();


app.Run();
