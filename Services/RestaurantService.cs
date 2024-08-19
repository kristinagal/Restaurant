using Restaurant.Interfaces;
using Restaurant.Models;

namespace Restaurant.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly string _folderPath;
        private FileManager _fileManager;
        private IUiService _uiService;
        private ITableService _tableService;
        private IOrderService _orderService;
        private readonly IEmailService _emailService;

        public RestaurantService(string folderPath, IEmailService emailService)
        {
            _folderPath = folderPath;
            _emailService = emailService;
        }

        public void Start()
        {
            InitializeFiles();

            while (true)
            {
                do
                {
                    Console.Clear();
                    LogIn();
                }
                while (string.IsNullOrEmpty(_tableService.CurrentEmployee?.ID));


                ShowTableMenu();
            }
        }

        public void InitializeFiles()
        {
            try
            {
                _fileManager = new FileManager(_folderPath);
                _uiService = new UiService();

                var employees = _fileManager.ReadCsvFile("Employees.csv", tokens => new Employee
                {
                    ID = tokens[0]
                }).ToList();

                var tables = _fileManager.ReadCsvFile("Tables.csv", tokens => new Table
                {
                    TableNumber = int.Parse(tokens[0]),
                    Seats = int.Parse(tokens[1]),
                    IsAvailable = bool.Parse(tokens[2])
                }).ToList();

                var foodMenu = _fileManager.ReadCsvFile("FoodMenu.csv", tokens => new FoodItem
                {
                    Name = tokens[0],
                    Price = double.Parse(tokens[1])
                }).ToList();

                var drinksMenu = _fileManager.ReadCsvFile("DrinksMenu.csv", tokens => new DrinkItem
                {
                    Name = tokens[0],
                    Price = double.Parse(tokens[1])
                }).ToList();

                _tableService = new TableService(tables, _fileManager);
                _orderService = new OrderService(foodMenu, drinksMenu, _fileManager, _emailService);

                var existingOrders = _fileManager.ReadOrdersFromJsonFile();
                foreach (var order in existingOrders.Where(o => !o.IsClosedOut))
                {
                    var table = tables.FirstOrDefault(t => t.TableNumber == order.Table.TableNumber);

                    if (table != null)
                    {
                        order.Table = table;
                        _tableService.CreateNewOrder(order);
                    }
                    else
                    {
                        Console.WriteLine($"Table for order {order.OrderNumber} not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while initializing files: " + ex.Message);
                Environment.Exit(1); // uzdaro programa jei kazkas su failais ne ok 
            }
        }

        public void LogIn()
        {
            Console.Clear();
            var employees = _fileManager.ReadCsvFile("Employees.csv", tokens => new Employee
            {
                ID = tokens[0]
            });

            var employeeID = _uiService.GetEmployeeID();
            var employee = employees.FirstOrDefault(e => e.ID == employeeID);

            if (employee == null)
            {
                Console.WriteLine("Invalid Employee ID.");
                return;
            }

            _tableService.LogIn(employee);
        }

        public void LogOut()
        {
            var employees = _fileManager.ReadCsvFile("Employees.csv", tokens => new Employee
            {
                ID = tokens[0]
            });

            var employeeID = _tableService.CurrentEmployee?.ID;
            var employee = employees.FirstOrDefault(e => e.ID == employeeID);

            if (employee != null)
            {
                _tableService.LogOut();
            }
        }

        public void ShowTableMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Current Employee: {_tableService.CurrentEmployee?.ID ?? "None"}");
                Console.WriteLine("Available Tables:");

                var tables = _fileManager.ReadCsvFile("Tables.csv", tokens => new Table
                {
                    TableNumber = int.Parse(tokens[0]),
                    Seats = int.Parse(tokens[1]),
                    IsAvailable = bool.Parse(tokens[2])
                }).ToList();

                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i];
                    string availability = table.IsAvailable ? "Available" : "Occupied";
                    Console.WriteLine($"{i + 1}. Table {table.TableNumber} - Seats: {table.Seats}, Status: {availability}");
                }

                Console.WriteLine($"{tables.Count + 1}. Log Out");

                int selection = _uiService.GetMenuSelection(1, tables.Count + 1);

                if (selection == tables.Count + 1)
                {
                    LogOut();
                    return;
                }

                if (selection < 1 || selection > tables.Count)
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    continue;
                }

                var selectedTable = tables[selection - 1];
                var order = _tableService.GetOrderForTable(selectedTable.TableNumber);

                if (order == null)
                {
                    order = new Order
                    {
                        Table = selectedTable,
                        Employee = _tableService.CurrentEmployee,
                        OrderNumber = new Random().Next(1000, 9999)
                    };
                    _tableService.CreateNewOrder(order);
                }

                ShowOrderMenu(order);
            }
        }

        public void ShowOrderMenu(Order order)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Current Employee: {_tableService.CurrentEmployee?.ID ?? "None"}");
                Console.WriteLine($"Table No. {order.Table.TableNumber}:");
                Console.WriteLine("1. Add Food Item");
                Console.WriteLine("2. Add Drink Item");
                Console.WriteLine("3. Check Current Balance");
                Console.WriteLine("4. Close Order");
                Console.WriteLine("5. Quit to Table Menu");

                var choice = _uiService.GetMenuSelection(1, 5);

                Console.Clear();

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Food Menu:");
                        var foodItems = _orderService.GetFoodMenu();
                        _orderService.PrintMenu(foodItems);

                        int foodIndex = _uiService.GetMenuSelection(1, foodItems.Count);
                        int foodQuantity = _uiService.GetQuantity();
                        _orderService.AddFood(order, foodIndex - 1, foodQuantity);
                        break;

                    case 2:
                        Console.WriteLine("Drink Menu:");
                        var drinkItems = _orderService.GetDrinkMenu();
                        _orderService.PrintMenu(drinkItems);

                        int drinkIndex = _uiService.GetMenuSelection(1, drinkItems.Count);
                        int drinkQuantity = _uiService.GetQuantity();
                        _orderService.AddDrink(order, drinkIndex - 1, drinkQuantity);
                        break;

                    case 3:
                        Console.WriteLine(order.GetOrderSummary());
                        Console.ReadLine();
                        break;

                    case 4:
                        if (_uiService.ConfirmAction("Do you want to close the tab?"))
                        {
                            Console.WriteLine();
                            bool clientWantsCheck = _uiService.ConfirmAction("Does the customer want a check?");
                            bool clientWantsEmail = _uiService.ConfirmAction("Does the customer want an email?");
                            string email = "";

                            if (clientWantsEmail)
                            {
                                email = _uiService.GetEmail();
                            }

                            _orderService.CloseOrder(order, clientWantsCheck, email, _folderPath);
                            _tableService.TableCheckOut(order.Table.TableNumber);
                            return;
                        }
                        break;

                    case 5:
                        Console.Clear();
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

            }
        }

    }

}
