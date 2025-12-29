using DependencyRegister;
using Microsoft.Extensions.FileProviders;

namespace ProductSale
{
    public class Startup
    {
        private IHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Environment = environment;
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
            services.AddRegisters(Configuration, typeof(Startup).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
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