using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    public class Employee
    {
        public string ID { get; set; }
    }

    public class FoodItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class DrinkItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class FoodOrder
    {
        public FoodItem FoodItem { get; set; }
        public int Quantity { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class DrinkOrder
    {
        public DrinkItem DrinkItem { get; set; }
        public int Quantity { get; set; }
        public DateTime TimeStamp { get; set; }
    }

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
            sb.AppendLine($"Table Number: {Table.TableNumber}");
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
    }
    public class Table
    {
        public int TableNumber { get; set; }
        public int Seats { get; set; }
        public bool IsAvailable { get; set; }
    }
}
