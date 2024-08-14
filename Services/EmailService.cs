using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Interfaces;

namespace Restaurant.Services
{
    public class EmailService : IEmailService
    {
        private const string restaurantEmail = "restoranas@restoranas.lt";
        public void SendEmailToCustomer(string toEmail, string subject, string body, string orderSummary)
        {
            Console.WriteLine();
            Console.WriteLine($"Sending check to {toEmail}...");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
            Console.WriteLine();
            Console.WriteLine(orderSummary);
            Console.ReadKey();
        }

        public void SendEmailToRestaurant(string subject, string orderSummary)
        {
            Console.WriteLine();
            Console.WriteLine($"Sending check to {restaurantEmail}...");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine(orderSummary);
            Console.ReadKey();
        }
    }
}
