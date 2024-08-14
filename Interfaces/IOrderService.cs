using Restaurant.Models;

namespace Restaurant.Interfaces
{
    public interface IOrderService
    {
        void AddDrink(Order order, int drinkIndex, int quantity);
        void AddFood(Order order, int foodIndex, int quantity);
        void CloseOrder(Order order, bool clientWantsCheck, string email, string folderPath);
        List<DrinkItem> GetDrinkMenu();
        List<FoodItem> GetFoodMenu();
        void PrintMenu<T>(List<T> items) where T : IMenuItem;
        void SaveOrder(Order order);
    }
}