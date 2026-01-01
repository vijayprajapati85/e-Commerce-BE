using DependencyRegister;
using ProductSale.Lib.App.Utility;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.ServiceRegisters
{
    public class RepositoriesRegister : IDependencyRegister
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IFileHelper, FileHelper>();
        }
    }
}
