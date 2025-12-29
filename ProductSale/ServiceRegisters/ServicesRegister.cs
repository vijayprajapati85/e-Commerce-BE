using DependencyRegister;
using ProductSale.Lib.App.Services;

namespace ProductSale.ServiceRegisters
{
    public class ServicesRegister : IDependencyRegister
    {
        public void Register(IServiceCollection services, IConfiguration configuration) { 
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ISubCategoryService, SubCategoryService>();
        }
    }
}
