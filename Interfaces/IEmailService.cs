namespace Restaurant.Interfaces
{
    public interface IEmailService
    {
        void SendEmailToCustomer(string toEmail, string subject, string body, string orderSummary);
        void SendEmailToRestaurant(string subject, string orderSummary);
    }
}
