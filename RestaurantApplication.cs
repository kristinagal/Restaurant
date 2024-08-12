namespace Restaurant
{
    public class RestaurantApplication
    {
        private readonly string _folderPath;
        private FileManager _fileManager;
        private UiService _uiService;
        private TableService _tableService;
        private OrderService _orderService;

        public RestaurantApplication(string folderPath)
        {
            _folderPath = folderPath;
        }

        public void Start()
        {
            InitializeFiles();

            while (true)
            {
                LogIn();
                if (string.IsNullOrEmpty(_tableService.CurrentEmployee?.ID))
                {
                    Console.Clear();
                    Console.WriteLine("No valid employee logged in. Exiting...");
                    return; // Exit the application if no employee is logged in
                }

                ShowTableMenu();
            }
        }

        private void InitializeFiles()
        {
            try
            {
                // Initialize FileManager and UiService
                _fileManager = new FileManager(_folderPath);
                _uiService = new UiService();

                // Load data from files
                var employees = _fileManager.ReadCsvFile("Employees.csv", tokens => new Employee
                {
                    ID = tokens[0],
                    IsCurrentlyLoggedIn = bool.Parse(tokens[1])
                });

                var tables = _fileManager.ReadCsvFile("Tables.csv", tokens => new Table
                {
                    TableNumber = int.Parse(tokens[0]),
                    Seats = int.Parse(tokens[1]),
                    IsAvailable = bool.Parse(tokens[2])
                });

                var foodMenu = _fileManager.ReadCsvFile("FoodMenu.csv", tokens => new FoodItem
                {
                    Name = tokens[0],
                    Price = double.Parse(tokens[1])
                });

                var drinksMenu = _fileManager.ReadCsvFile("DrinksMenu.csv", tokens => new DrinkItem
                {
                    Name = tokens[0],
                    Price = double.Parse(tokens[1])
                });

                // Initialize services
                _tableService = new TableService(tables, _uiService);
                _orderService = new OrderService(foodMenu, drinksMenu);

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
                ID = tokens[0],
                IsCurrentlyLoggedIn = bool.Parse(tokens[1])
            });

            var employeeID = _uiService.GetEmployeeID();
            var employee = employees.FirstOrDefault(e => e.ID == employeeID);

            if (employee == null)
            {
                Console.WriteLine("Invalid Employee ID.");
                return;
            }

            if (employee.IsCurrentlyLoggedIn)
            {
                Console.WriteLine("Employee is already logged in.");
                return;
            }

            employee.IsCurrentlyLoggedIn = true;
            _fileManager.WriteCsvFile("Employees.csv", employees, e => $"{e.ID},{e.IsCurrentlyLoggedIn}");
            _tableService.LogIn(employee);
        }

        private void LogOut()
        {
            var employees = _fileManager.ReadCsvFile("Employees.csv", tokens => new Employee
            {
                ID = tokens[0],
                IsCurrentlyLoggedIn = bool.Parse(tokens[1])
            });

            var employeeID = _tableService.CurrentEmployee?.ID;
            var employee = employees.FirstOrDefault(e => e.ID == employeeID);

            if (employee != null)
            {
                employee.IsCurrentlyLoggedIn = false;
                _fileManager.WriteCsvFile("Employees.csv", employees, e => $"{e.ID},{e.IsCurrentlyLoggedIn}");
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
                        Console.WriteLine($"Current Balance: {_orderService.GetCurrentBalance(order):C}");
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
