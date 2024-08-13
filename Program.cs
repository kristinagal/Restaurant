namespace Restaurant
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string folderPath = "C:\\Users\\Kristina\\source\\repos\\Restaurant\\Files";
            var restaurantApp = new Restaurant(folderPath);
            restaurantApp.Start();

            /*
             add option to get email
             
             */

        }
    }
}
