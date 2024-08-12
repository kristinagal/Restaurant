namespace Restaurant
{
    public class UiService
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
                if (int.TryParse(Console.ReadLine(), out selection) && selection >= min && selection <= max)
                {
                    return selection;
                }
                Console.WriteLine($"Invalid selection. Please enter a number between {min} and {max}.");
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
                Console.WriteLine("Invalid quantity. Please enter a positive integer.");
            }
        }

        public bool ConfirmAction(string message)
        {
            Console.Write($"{message} (Y/N): ");
            var response = Console.ReadLine()?.ToUpper();
            return response == "Y";
        }
    }

}
