using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Restaurant
{
    public class OrderService
    {
        private readonly List<FoodItem> _foodMenu;
        private readonly List<DrinkItem> _drinkMenu;
        private readonly FileManager _fileManager;

        public OrderService(List<FoodItem> foodMenu, List<DrinkItem> drinkMenu, FileManager fileManager)
        {
            _foodMenu = foodMenu;
            _drinkMenu = drinkMenu;
            _fileManager = fileManager;
        }

        public void AddFood(Order order, int foodIndex, int quantity)
        {
            var foodItem = _foodMenu[foodIndex];
            order.FoodOrders.Add(new FoodOrder { FoodItem = foodItem, Quantity = quantity, TimeStamp = DateTime.Now });
            SaveOrder(order);  // Save the updated order to the JSON file
        }

        public void AddDrink(Order order, int drinkIndex, int quantity)
        {
            var drinkItem = _drinkMenu[drinkIndex];
            order.DrinkOrders.Add(new DrinkOrder { DrinkItem = drinkItem, Quantity = quantity, TimeStamp = DateTime.Now });
            SaveOrder(order);  // Save the updated order to the JSON file
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
            string customerCheckPath = Path.Combine(folderPath, $"{order.Table.TableNumber}_{order.OrderNumber}_{DateTime.Now:yyyyMMdd}.pdf");

            if (clientWantsCheck)
            {
                GeneratePdf(order, customerCheckPath);
            }

            SaveOrder(order);  // Save the closed order to the JSON file
            order.Table.IsAvailable = true;
        }

        private void GeneratePdf(Order order, string path)
        {
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();
            doc.Add(new Paragraph(order.GetOrderSummary()));
            doc.Close();
        }

        private void SaveOrder(Order order)
        {
            var orders = _fileManager.ReadOrdersFromJsonFile();
            var existingOrderIndex = orders.FindIndex(o => o.OrderNumber == order.OrderNumber && o.Table.TableNumber == order.Table.TableNumber);

            if (existingOrderIndex >= 0)
            {
                // Update existing order
                orders[existingOrderIndex] = order;
            }
            else
            {
                // Add new order
                orders.Add(order);
            }

            _fileManager.WriteJsonFile(orders); // Save updated orders list to JSON
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
