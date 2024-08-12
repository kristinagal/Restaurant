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

            stalas nepazymimas kaip uzimtas, order objektą kurti kai pasirenkamas stalas?
            current balance - rodyti visa čekio info?
            email siuntimas
            nunit
            kai programa restartuoja, visus employees padaryt neprisilogginusius
             
             */

        }
    }
}
