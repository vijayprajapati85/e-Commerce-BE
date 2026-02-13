namespace ProductSale.Lib.App.Services
{
    public interface IJobSchedulerService
    {
        public void CartReminderJob();
        public void NewProductReminderJob();
    }
}
