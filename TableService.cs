using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Restaurant
{
    public class TableService
    {
        private readonly List<Table> _tables;
        private readonly Dictionary<int, Order> _currentOrders;
        private UiService _uiService;
        public Employee CurrentEmployee { get; private set; }

        public TableService(List<Table> tables, UiService uiService)
        {
            _tables = tables;
            _currentOrders = new Dictionary<int, Order>();
            _uiService = uiService;
        }

        //public Table ShowAllTablesMenu()
        //{
        //    Console.Clear();
        //    Console.WriteLine("Available Tables:");

        //    for (int i = 0; i < _tables.Count; i++)
        //    {
        //        var table = _tables[i];
        //        Console.WriteLine($"{i + 1}. Table {table.TableNumber} - Seats: {table.Seats}, Available: {table.IsAvailable}");
        //    }

        //    int selection = _uiService.GetMenuSelection(1, _tables.Count);
        //    return _tables[selection - 1];
        //}

        public void TableCheckOut(int tableNumber)
        {
            if (_currentOrders.ContainsKey(tableNumber))
            {
                _currentOrders.Remove(tableNumber);
            }
            var table = _tables.FirstOrDefault(t => t.TableNumber == tableNumber);
            if (table != null)
            {
                table.IsAvailable = true;
            }
        }

        public Order GetOrderForTable(int tableNumber)
        {
            if (_currentOrders.TryGetValue(tableNumber, out var order))
            {
                return order;
            }
            return null;
        }

        public void CreateNewOrder(Order order)
        {
            _currentOrders[order.Table.TableNumber] = order;
            order.Table.IsAvailable = false;
        }

        public void LogIn(Employee employee)
        {
            CurrentEmployee = employee;
        }

        public void LogOut()
        {
            if (CurrentEmployee != null)
            {
                CurrentEmployee.IsCurrentlyLoggedIn = false;
                CurrentEmployee = null;
            }
        }
    }

}
