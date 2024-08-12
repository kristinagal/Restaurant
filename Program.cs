namespace Restaurant
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string folderPath = "C:\\Users\\Kristina\\source\\repos\\Restaurant\\Files";
            var restaurantApp = new RestaurantApplication(folderPath);
            restaurantApp.Start();

            /*
             Problemos:
            stalas nepazymimas kaip uzimtas
            current balance - rodyti čekio info?

             
             */

        }
    }
}
