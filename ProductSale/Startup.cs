using DependencyRegister;
using Microsoft.Extensions.FileProviders;
using ProductSale.Lib.App.Models;

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
            services.AddAuthorization(options =>
            {
                options.InvokeHandlersAfterFailure = false;
            });
            services.AddOptions();
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
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

            app.UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}