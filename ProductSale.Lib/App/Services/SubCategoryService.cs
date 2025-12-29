using ProductSale.Domain;
using ProductSale.Lib.App.Builder;
using ProductSale.Lib.App.Models;
using ProductSale.Lib.Domain;
using ProductSale.Lib.Infra.Repo;

namespace ProductSale.Lib.App.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ISubCategoryRepository _repository;
        public string? UserId { get; set; }
        public SubCategoryService(ISubCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SubCategoryDto>?> GetAllSubCategoryAsync()
        {
            try
            {
                List<SubCategory> categories = await _repository.GetAllSubCategoryAsync();
                return SubCategoryDtoMapping.SetSubCategory(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<SubCategoryDto?> GetByIdAsync(long id)
        {
            try
            {
                SubCategory category = await _repository.GetByIdAsync(id);
                return SubCategoryDtoMapping.SetSubCategory(category);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<int> UpsertSubCateogryAsync(SubCategoryRequest subCategory)
        {
            try
            {
               return await _repository.UpsertSubCateogryAsync(new SubCategory
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name,
                    CatId = subCategory.CatId,
                    IsActive = true,
                    CreatedBy = UserId,
                    CreatedDateTime = DateTime.Now,
                    UpdatedBy = UserId,
                    UpdatedDateTime = DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteSubCategoryAsync(long id)
        {
            try
            {
                return await _repository.DeleteSubCategoryAsync(new DeleteRequest
                {
                    Id = id,
                    UpdatedBy = UserId ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<List<SubCategoryDto>?> GetByCatIdAsync(long id)
        {
            try
            {
                List<SubCategory> categories = await _repository.GetByCatIdAsync(id);
                return SubCategoryDtoMapping.SetSubCategory(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
