using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Restaurant
{
    public class FileManager
    {
        private readonly string _folderPath;

        public FileManager(string folderPath)
        {
            _folderPath = folderPath;
            EnsureFilesExist();
        }

        private void EnsureFilesExist()
        {
            EnsureCsvFileExists("Employees.csv", "ID,IsLoggedIn");
            EnsureCsvFileExists("FoodMenu.csv", "Name,Price");
            EnsureCsvFileExists("DrinksMenu.csv", "Name,Price");
            EnsureCsvFileExists("Tables.csv", "TableNumber,Seats,IsAvailable");
            EnsureJsonFileExists("RestaurantChecks.json");
        }

        private void EnsureCsvFileExists(string fileName, string header)
        {
            string filePath = Path.Combine(_folderPath, fileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, header + Environment.NewLine);
            }
        }

        private void EnsureJsonFileExists(string fileName)
        {
            string filePath = Path.Combine(_folderPath, fileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }
        }

        public List<T> ReadCsvFile<T>(string fileName, Func<string[], T> mapFunction)
        {
            string filePath = Path.Combine(_folderPath, fileName);
            var lines = File.ReadAllLines(filePath);
            return lines.Skip(1).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => mapFunction(line.Split(','))).ToList();
        }

        public void WriteCsvFile<T>(string fileName, List<T> data, Func<T, string> mapFunction)
        {
            string filePath = Path.Combine(_folderPath, fileName);
            var lines = new List<string> { data.FirstOrDefault()?.GetType().Name };
            lines.AddRange(data.Select(mapFunction));
            File.WriteAllLines(filePath, lines);
        }

        public void AppendToJsonFile<T>(string fileName, T data)
        {
            string filePath = Path.Combine(_folderPath, fileName);
            var existingData = File.ReadAllText(filePath);
            var jsonData = JsonConvert.DeserializeObject<List<T>>(existingData) ?? new List<T>();
            jsonData.Add(data);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(jsonData, Formatting.Indented));
        }
    }

}
