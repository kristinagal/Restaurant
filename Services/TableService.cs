using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Restaurant.Interfaces;
using Restaurant.Models;

namespace Restaurant.Services
{
    public class TableService : ITableService
    {
        private readonly List<Table> _tables;
        private readonly Dictionary<int, Order> _currentOrders;
        private readonly IFileManager _fileManager;

        public Employee CurrentEmployee { get; private set; }

        public TableService(List<Table> tables, IFileManager fileManager)
        {
            _tables = tables ?? throw new ArgumentNullException(nameof(tables));
            _currentOrders = new Dictionary<int, Order>();
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public void TableCheckOut(int tableNumber)
        {
            if (_currentOrders.ContainsKey(tableNumber))
            {
                _currentOrders.Remove(tableNumber);
            }
            var table = _tables.FirstOrDefault(t => t.TableNumber == tableNumber);
            UpdateTableAvailability(table.TableNumber, true);
        }

        public Order GetOrderForTable(int tableNumber)
        {
            if (_currentOrders.TryGetValue(tableNumber, out var order))
            {
                return order;
            }

            var table = _tables.FirstOrDefault(t => t.TableNumber == tableNumber);
            if (table != null)
            {
                UpdateTableAvailability(table.TableNumber, false);
            }

            return null;
        }

        public void CreateNewOrder(Order order)
        {
            if (order == null || order.Table == null)
            {
                Console.WriteLine("Order or Table is null.");
                return;
            }

            _currentOrders[order.Table.TableNumber] = order;
            UpdateTableAvailability(order.Table.TableNumber, false);
        }

        public void LogIn(Employee employee)
        {
            CurrentEmployee = employee;
        }

        public void LogOut()
        {
            if (CurrentEmployee != null)
            {
                CurrentEmployee = null;
            }
        }

        public void UpdateTableAvailability(int tableNumber, bool isAvailable)
        {
            var tables = _fileManager.ReadCsvFile("Tables.csv", tokens => new Table
            {
                TableNumber = int.Parse(tokens[0]),
                Seats = int.Parse(tokens[1]),
                IsAvailable = bool.Parse(tokens[2])
            });

            var table = tables.FirstOrDefault(t => t.TableNumber == tableNumber);
            if (table != null)
            {
                table.IsAvailable = isAvailable;

                var header = "TableNumber,Seats,IsAvailable";
                _fileManager.WriteCsvFile("Tables.csv", tables, t => $"{t.TableNumber},{t.Seats},{t.IsAvailable}", header);
            }
            else
            {
                Console.WriteLine("Table not found.");
            }
        }
    }

}
