using Restaurant.Models;

namespace Restaurant.Interfaces
{
    public interface ITableService
    {
        Employee CurrentEmployee { get; }

        void CreateNewOrder(Order order);
        Order GetOrderForTable(int tableNumber);
        void LogIn(Employee employee);
        void LogOut();
        void TableCheckOut(int tableNumber);
        void UpdateTableAvailability(int tableNumber, bool isAvailable);
    }
}