using DependencyRegister;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using ProductSale.Lib.App.Models;
using System.Text;
using FluentEmail.MailKitSmtp;
using Hangfire;
using ProductSale.Lib.App.Services;

namespace ProductSale
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    // Set JWT Bearer as the default scheme for authenticating and challenging requests
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.AddAuthorization();
            services.AddMemoryCache();
            services.AddOptions();

            var smtpOption = Configuration.GetSection(nameof(SmtpClientOptions)).Get<SmtpClientOptions>();
            smtpOption!.SocketOptions = MailKit.Security.SecureSocketOptions.StartTls;

            services.AddFluentEmail(
                defaultFromEmail: Configuration["MailSettings:Mail"],
                defaultFromName: Configuration["MailSettings:DisplayName"]
           )
                .AddRazorRenderer()
                .AddMailKitSender(smtpOption);

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            var connectionString = Configuration["HangfireConnection"];

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString));
            services.AddHangfireServer();

            services.AddRegisters(Configuration, typeof(Startup).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("./swagger/v1/swagger.json", "jiyazon.com V1");
            //});
            //}
            app.UseStaticFiles();

            // Serve files from the physical "images" folder at URL path /images
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "images")),
                RequestPath = "/images"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "images")),
                RequestPath = "/images"
            });

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            RecurringJob.AddOrUpdate<IJobSchedulerService>("CartReminder", 
                service => service.CartReminderJob(), 
                Cron.Daily(23, 55),
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

            RecurringJob.AddOrUpdate<IJobSchedulerService>("NewProductsReminder",
                service => service.NewProductReminderJob(),
                Cron.Daily(23, 30),
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
        }
    }
}