using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.App.Services
{
    public interface IContactInfoService
    {
        public Task<int> Submit(ContactInfo contactInfo);
    }
}
