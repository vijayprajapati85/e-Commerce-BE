using ProductSale.Lib.App.Models.Email;

namespace ProductSale.Lib.Infra.Repo
{
    public interface IContactInfoRepository
    {
        public Task<int> Submit(ContactInfo contactInfo);
    }
}
