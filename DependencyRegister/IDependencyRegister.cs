using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyRegister
{
    public interface IDependencyRegister
    {
        void Register(IServiceCollection services, IConfiguration configuration);
    }
}
