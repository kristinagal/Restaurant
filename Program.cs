using Microsoft.Extensions.Hosting;
using Restaurant.Services;
using System;
using System.Net;
using System.Net.Mail;

namespace Restaurant
{
    internal class Program
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
