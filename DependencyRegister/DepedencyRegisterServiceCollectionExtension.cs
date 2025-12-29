using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DependencyRegister
{
    public static class DepedencyRegisterServiceCollectionExtension
    {
        public static IServiceCollection AddRegisters(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            var registers = assembly.ExportedTypes
                .Where(t => typeof(IDependencyRegister).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .Select(Activator.CreateInstance)
                .Cast<IDependencyRegister>()
                .ToList();

            foreach (var register in registers)
            {
                register.Register(services, configuration);
            }

            return services;
        }
    }
}
