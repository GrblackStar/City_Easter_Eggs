#region Using

using AspNetCore.ReCaptcha;
using City_Easter_Eggs.Controllers;
using City_Easter_Eggs.Data;
using Microsoft.AspNetCore.Identity;
using SecurityStampValidator = City_Easter_Eggs.Helpers.SecurityStampValidator;

#endregion

namespace City_Easter_Eggs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ApplicationDbContext.SetupDatabase(builder);
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            }).AddIdentityCookies(o => { });
            builder.Services.AddScoped<ISecurityStampValidator, SecurityStampValidator>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<PointsService>();
            builder.Services.AddScoped<MapsService>();

            builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));
            builder.Services.AddRazorPages();

#if !DEBUG
            var certificatePassword = builder.Configuration["ssl_password"];

            builder.WebHost.UseKestrel(options =>
            {
                options.Listen(System.Net.IPAddress.Any, 80);
                options.Listen(System.Net.IPAddress.Any, 443, listenOptions =>
                {
                    listenOptions.UseHttps("certificate.pfx", certificatePassword);
                });
            });
#endif

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            //app.MapControllers();

            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}