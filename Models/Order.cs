using System.Text;

namespace Restaurant.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public List<FoodOrder> FoodOrders { get; set; }
        public List<DrinkOrder> DrinkOrders { get; set; }
        public Employee Employee { get; set; }
        public Table Table { get; set; }
        public bool ClientWantsCheck { get; set; }
        public bool IsClosedOut { get; set; }

        public Order()
        {
            FoodOrders = new List<FoodOrder>();
            DrinkOrders = new List<DrinkOrder>();
            IsClosedOut = false;
        }

        public double GetTotalPrice()
        {
            double foodTotal = 0;
            double drinkTotal = 0;

            foreach (var foodOrder in FoodOrders)
            {
                foodTotal += foodOrder.FoodItem.Price * foodOrder.Quantity;
            }

            foreach (var drinkOrder in DrinkOrders)
            {
                drinkTotal += drinkOrder.DrinkItem.Price * drinkOrder.Quantity;
            }

            return foodTotal + drinkTotal;
        }

        public string GetOrderSummary()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Order Number: {OrderNumber}");
            sb.AppendLine($"Table Number: {Table.TableNumber}, seats: {Table.Seats}");
            sb.AppendLine($"Server: {Employee.ID}");
            sb.AppendLine("Food Orders:");
            foreach (var foodOrder in FoodOrders)
            {
                sb.AppendLine($"{foodOrder.FoodItem.Name} - ${foodOrder.FoodItem.Price:F2} x {foodOrder.Quantity}");
            }
            sb.AppendLine("Drink Orders:");
            foreach (var drinkOrder in DrinkOrders)
            {
                sb.AppendLine($"{drinkOrder.DrinkItem.Name} - ${drinkOrder.DrinkItem.Price:F2} x {drinkOrder.Quantity}");
            }
            sb.AppendLine($"Total Price: ${GetTotalPrice():F2}");
            return sb.ToString();
        }

        public string GetOrderSummaryForCustomer()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Order Number: {OrderNumber}");
            sb.AppendLine($"Server: {Employee.ID}");
            sb.AppendLine("Order items:");
            foreach (var foodOrder in FoodOrders)
            {
                sb.AppendLine($"{foodOrder.FoodItem.Name} - ${foodOrder.FoodItem.Price:F2} x {foodOrder.Quantity}");
            }
            foreach (var drinkOrder in DrinkOrders)
            {
                sb.AppendLine($"{drinkOrder.DrinkItem.Name} - ${drinkOrder.DrinkItem.Price:F2} x {drinkOrder.Quantity}");
            }
            sb.AppendLine($"Total Price: ${GetTotalPrice():F2}");
            return sb.ToString();
        }
    }
}
