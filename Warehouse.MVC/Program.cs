using Microsoft.AspNetCore.Authentication.Cookies;
using Rotativa.AspNetCore;
using Warehouse.MVC.Models;

namespace Warehouse.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5); // Session h?t h?n sau 5 phút không ho?t ??ng
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped<PdfService>();
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyOrigin() // Cho phép m?i ngu?n (có th? gi?i h?n domain n?u c?n)
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            builder.Services
     .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.LoginPath = "/Auth";
         options.AccessDeniedPath = "/Home/AccessDenied";
         options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Cookie h?t h?n sau 5 phút
         options.SlidingExpiration = false; // Không t? ??ng gia h?n cookie
     });


            builder.Services.AddAuthorization();
            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
     name: "default",
     pattern: "{controller=Auth}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
