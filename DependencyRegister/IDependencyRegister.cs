using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyRegister
{
    public interface IDependencyRegister
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}
