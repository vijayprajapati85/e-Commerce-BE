using DependencyRegister;

namespace ProductSale.ServiceRegisters
{
    public class LibrariesRegister : IDependencyRegister
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}
