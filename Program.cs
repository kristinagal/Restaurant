using Restaurant.Services;


namespace Restaurant
{
    public class Program
    {
        static void Main(string[] args)
        {
            var emailService = new EmailService();

            string folderPath = "C:\\Users\\Kristina\\source\\repos\\Restaurant\\Files";

            var restaurantApp = new RestaurantService(folderPath, emailService);
            restaurantApp.Start();


        }
       
    }

}
