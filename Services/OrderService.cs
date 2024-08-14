using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Restaurant.Models;
using Restaurant.Interfaces;

namespace Restaurant.Services
{
    public class OrderService : IOrderService
    {
        private readonly List<FoodItem> _foodMenu;
        private readonly List<DrinkItem> _drinkMenu;
        private readonly FileManager _fileManager;
        private readonly IEmailService _emailService;

        public OrderService(List<FoodItem> foodMenu, List<DrinkItem> drinkMenu, FileManager fileManager, IEmailService emailService)
        {
            _foodMenu = foodMenu;
            _drinkMenu = drinkMenu;
            _fileManager = fileManager;
            _emailService = emailService;
        }

        public void AddFood(Order order, int foodIndex, int quantity)
        {
            var foodItem = _foodMenu[foodIndex];

            var existingFoodOrder = order.FoodOrders.FirstOrDefault(fo => fo.FoodItem == foodItem);

            if (existingFoodOrder != null)
            {
                existingFoodOrder.Quantity += quantity;
                existingFoodOrder.TimeStamp = DateTime.Now;
            }
            else
            {
                order.FoodOrders.Add(new FoodOrder { FoodItem = foodItem, Quantity = quantity, TimeStamp = DateTime.Now });
            }

            SaveOrder(order);
        }

        public void AddDrink(Order order, int drinkIndex, int quantity)
        {
            var drinkItem = _drinkMenu[drinkIndex];

            var existingDrinkOrder = order.DrinkOrders.FirstOrDefault(dr => dr.DrinkItem == drinkItem);

            if (existingDrinkOrder != null)
            {
                existingDrinkOrder.Quantity += quantity;
                existingDrinkOrder.TimeStamp = DateTime.Now;
            }
            else
            {
                order.DrinkOrders.Add(new DrinkOrder { DrinkItem = drinkItem, Quantity = quantity, TimeStamp = DateTime.Now });
            }

            SaveOrder(order);
        }

        public void CloseOrder(Order order, bool clientWantsCheck, string email, string folderPath)
        {
            if (order.FoodOrders.Count == 0 && order.DrinkOrders.Count == 0)
            {
                Console.WriteLine("No items added. Order will not be saved.");
                order.Table.IsAvailable = true;
                return;
            }

            order.ClientWantsCheck = clientWantsCheck;
            order.IsClosedOut = true;
            string customerCheckPath = Path.Combine(folderPath, $"CustomerCheck_{order.OrderNumber}_{DateTime.Now:yyyyMMdd}.pdf");
            string restaurantCheckPath = Path.Combine(folderPath, $"{order.Table.TableNumber}_{order.OrderNumber}_{DateTime.Now:yyyyMMdd}.pdf");

            if (clientWantsCheck)
            {
                _fileManager.GeneratePdfForCustomer(order, customerCheckPath);
            }

            if (!string.IsNullOrEmpty(email))
            {
                _emailService.SendEmailToCustomer(email, "Your Order Receipt", "Thank you for dining with us.", order.GetOrderSummaryForCustomer());
            }

            _emailService.SendEmailToRestaurant(order.OrderNumber.ToString(), order.GetOrderSummaryForCustomer());
            _fileManager.WriteCheckToFile(restaurantCheckPath, order.GetOrderSummary());

            SaveOrder(order);
            order.Table.IsAvailable = true;
        }

        public void SaveOrder(Order order)
        {
            var orders = _fileManager.ReadOrdersFromJsonFile();
            var existingOrderIndex = orders.FindIndex(o => o.OrderNumber == order.OrderNumber && o.Table.TableNumber == order.Table.TableNumber);

            if (existingOrderIndex >= 0)
            {
                orders[existingOrderIndex] = order;
            }
            else
            {
                orders.Add(order);
            }

            _fileManager.WriteJsonFile(orders);
        }

        public List<FoodItem> GetFoodMenu()
        {
            return _foodMenu;
        }

        public List<DrinkItem> GetDrinkMenu()
        {
            return _drinkMenu;
        }

        public void PrintMenu<T>(List<T> items) where T : IMenuItem
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                Console.WriteLine($"{i + 1}. {item.Name} - ${item.Price:F2}");
            }
            Console.WriteLine();
        }
    }
}
