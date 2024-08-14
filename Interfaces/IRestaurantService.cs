using Restaurant.Models;

namespace Restaurant.Interfaces
{
    public interface IRestaurantService
    {
        void InitializeFiles();
        void LogIn();
        void LogOut();
        void ShowOrderMenu(Order order);
        void ShowTableMenu();
        void Start();
    }
}