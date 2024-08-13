namespace Restaurant
{
    public class Restaurant
    {
        private readonly string _folderPath;
        private FileManager _fileManager;
        private UiService _uiService;
        private TableService _tableService;
        private OrderService _orderService;

        public Restaurant(string folderPath)
        {
            _folderPath = folderPath;
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

        private void InitializeFiles()
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

                // Initialize services
                _tableService = new TableService(tables, _uiService, _fileManager);
                _orderService = new OrderService(foodMenu, drinksMenu, _fileManager);

                // Load unclosed orders from the file
                var existingOrders = _fileManager.ReadOrdersFromJsonFile();
                foreach (var order in existingOrders.Where(o => !o.IsClosedOut))
                {
                    // Find the table for the order
                    var table = tables.FirstOrDefault(t => t.TableNumber == order.Table.TableNumber);

                    if (table != null)
                    {
                        // Assign the table to the order
                        order.Table = table;

                        // Create or update the order in the TableService
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
                Environment.Exit(1); // Exit the application if file initialization fails
            }
        }

        private void LogIn()
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

        private void LogOut()
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

        private void ShowTableMenu()
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

        private void ShowOrderMenu(Order order)
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
                Console.WriteLine("Q. Quit to Table Menu");

                var choice = Console.ReadLine();
                if (choice.ToUpper() == "Q")
                {
                    Console.Clear();
                    return;
                }

                Console.Clear();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Food Menu:");
                        var foodItems = _orderService.GetFoodMenu();
                        for (int i = 0; i < foodItems.Count; i++)
                        {
                            var item = foodItems[i];
                            Console.WriteLine($"{i + 1}. {item.Name} - ${item.Price:F2}");
                        }
                        Console.WriteLine();
                        int foodIndex = _uiService.GetMenuSelection(1, foodItems.Count);
                        int foodQuantity = _uiService.GetQuantity();
                        _orderService.AddFood(order, foodIndex - 1, foodQuantity);
                        break;

                    case "2":
                        Console.WriteLine("Drink Menu:");
                        var drinkItems = _orderService.GetDrinkMenu();
                        for (int i = 0; i < drinkItems.Count; i++)
                        {
                            var item = drinkItems[i];
                            Console.WriteLine($"{i + 1}. {item.Name} - ${item.Price:F2}");
                        }
                        Console.WriteLine();
                        int drinkIndex = _uiService.GetMenuSelection(1, drinkItems.Count);
                        int drinkQuantity = _uiService.GetQuantity();
                        _orderService.AddDrink(order, drinkIndex - 1, drinkQuantity);
                        break;

                    case "3":
                        Console.WriteLine(order.GetOrderSummary());
                        Console.ReadLine();
                        break;

                    case "4":
                        if (_uiService.ConfirmAction("Do you want to close the tab?"))
                        {
                            bool clientWantsCheck = _uiService.ConfirmAction("Does the customer need a check?");
                            _orderService.CloseOrder(order, clientWantsCheck, _folderPath);
                            _tableService.TableCheckOut(order.Table.TableNumber);
                            return;
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }

}
