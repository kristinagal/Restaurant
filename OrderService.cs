using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Restaurant
{
    public class OrderService
    {
        private readonly List<FoodItem> _foodMenu;
        private readonly List<DrinkItem> _drinkMenu;
        private readonly Table _table;

        public OrderService(List<FoodItem> foodMenu, List<DrinkItem> drinkMenu)
        {
            _foodMenu = foodMenu;
            _drinkMenu = drinkMenu;
        }

        public OrderService(List<FoodItem> foodMenu, List<DrinkItem> drinkMenu, Table table)
        {
            _foodMenu = foodMenu;
            _drinkMenu = drinkMenu;
            _table = table;
            _table.IsAvailable = false;
        }

        public void AddFood(Order order, int foodIndex, int quantity)
        {
            var foodItem = _foodMenu[foodIndex];
            order.FoodOrders.Add(new FoodOrder { FoodItem = foodItem, Quantity = quantity, TimeStamp = DateTime.Now });
        }

        public void AddDrink(Order order, int drinkIndex, int quantity)
        {
            var drinkItem = _drinkMenu[drinkIndex];
            order.DrinkOrders.Add(new DrinkOrder { DrinkItem = drinkItem, Quantity = quantity, TimeStamp = DateTime.Now });
        }

        public double GetCurrentBalance(Order order)
        {
            return order.GetTotalPrice();
        }

        public void CloseOrder(Order order, bool clientWantsCheck, string folderPath)
        {
            if (order.FoodOrders.Count == 0 && order.DrinkOrders.Count == 0)
            {
                Console.WriteLine("No items added. Order will not be saved.");
                order.Table.IsAvailable = true;
                return;
            }

            order.ClientWantsCheck = clientWantsCheck;
            order.IsClosedOut = true;
            string restaurantCheckPath = Path.Combine(folderPath, "RestaurantChecks.json");
            string customerCheckPath = Path.Combine(folderPath, $"{order.Table.TableNumber}_{order.OrderNumber}_{DateTime.Now:yyyyMMdd}.json");

            SaveOrder(order, restaurantCheckPath);
            if (clientWantsCheck)
            {
                SaveOrder(order, customerCheckPath);
            }

            order.Table.IsAvailable = true;
        }

        private void SaveOrder(Order order, string path)
        {
            var json = JsonConvert.SerializeObject(order, Formatting.Indented);
            File.AppendAllText(path, json + Environment.NewLine);
        }

        public int GetFoodMenuCount()
        {
            return _foodMenu.Count;
        }
        public int GetDrinkMenuCount()
        {
            return _drinkMenu.Count;
        }
        public List<FoodItem> GetFoodMenu()
        {
            return _foodMenu;
        }
        public List<DrinkItem> GetDrinkMenu()
        {
            return _drinkMenu;
        }
    }

}
