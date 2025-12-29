using DependencyRegister;

namespace ProductSale.ServiceRegisters
{
    public class CorsRegister : IDependencyRegister
    {
        public void Register(IServiceCollection services, IConfiguration configuration) { 
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
            });
        }
    }
}
