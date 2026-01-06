using Serilog;

namespace ProductSale
{
    public static class Program
    {
        //private static IConfigurationRefresher _referesher;

        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
                //_referesher?.TryRefreshAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureConfigurations)
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("images");
                    //webBuilder.ConfigureLogging(logging =>
                    //{
                    //    logging.ClearProviders();
                    //    logging.addNlog
                    //});
                });
        }

        private static void ConfigureConfigurations(IConfigurationBuilder config)
        {
            config.Build();
        }
    }
}
