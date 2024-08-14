using System.Text.RegularExpressions;
using Restaurant.Interfaces;

namespace Restaurant.Services
{
    public class UiService : IUiService
    {
        public string GetEmployeeID()
        {
            Console.Write("Enter Employee ID: ");
            return Console.ReadLine();
        }
        public int GetMenuSelection(int min, int max)
        {
            int selection;
            while (true)
            {
                Console.Write($"Select an option ({min}-{max}): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out selection) && selection >= min && selection <= max)
                {
                    return selection;
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }
        public int GetQuantity()
        {
            int quantity;
            while (true)
            {
                Console.Write("Enter quantity: ");
                if (int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
                {
                    return quantity;
                }

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }
        public bool ConfirmAction(string message)
        {
            Console.Write($"{message} (Y/N): ");
            var response = Console.ReadLine()?.ToUpper();
            return response == "Y";
        }
        public string GetEmail()
        {
            string email;
            while (true)
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine();
                if (IsValidEmail(email))
                {
                    return email;
                }

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            if (!email.Contains("@") || !email.Contains("."))
                return false;

            // Regex pattern - tekstas@tekstas.tekstas, čia tekstas - be @ ir tarpu
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }


    }

}
