using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Restaurant
{
    public class FileManager
    {
        private readonly string _folderPath;
        private const string restaurantChecksFileName = "RestaurantChecks.json";
        private string restaurantChecksPath;

        public FileManager(string folderPath)
        {
            _folderPath = folderPath;
            restaurantChecksPath = Path.Combine(_folderPath, restaurantChecksFileName);
        }

        public List<T> ReadCsvFile<T>(string fileName, Func<string[], T> map)
        {
            var path = Path.Combine(_folderPath, fileName);
            var lines = File.ReadAllLines(path);
            return lines.Skip(1)
                        .Select(line => line.Split(','))
                        .Select(map)
                        .ToList();
        }

        public void WriteCsvFile<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvLine, string header = null)
        {
            var path = Path.Combine(_folderPath, fileName);

            // Check if the file already exists and has content
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path).ToList();

                // If the file has lines, assume the first line is the header
                if (lines.Count > 0)
                {
                    // Extract the existing header and data
                    var existingHeader = lines.First();
                    var existingData = lines.Skip(1).ToList();

                    // Create the new content with updated records
                    var csvLines = new List<string> { existingHeader };
                    csvLines.AddRange(records.Select(toCsvLine));

                    // Write updated content to the file
                    File.WriteAllLines(path, csvLines);
                }
                else
                {
                    // If the file is empty, write the header and data
                    var csvLines = new List<string>();
                    if (!string.IsNullOrEmpty(header))
                    {
                        csvLines.Add(header);
                    }
                    csvLines.AddRange(records.Select(toCsvLine));
                    File.WriteAllLines(path, csvLines);
                }
            }
            else
            {
                // If the file does not exist, create it with header and data
                var csvLines = new List<string>();
                if (!string.IsNullOrEmpty(header))
                {
                    csvLines.Add(header);
                }
                csvLines.AddRange(records.Select(toCsvLine));
                File.WriteAllLines(path, csvLines);
            }
        }

        public List<T> ReadJsonFile<T>(string fileName)
        {
            var path = Path.Combine(_folderPath, fileName);
            if (!File.Exists(path))
            {
                return new List<T>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void WriteJsonFile<T>(List<T> data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(restaurantChecksPath, json);
        }

        public List<Order> ReadOrdersFromJsonFile()
        {
            return ReadJsonFile<Order>(restaurantChecksFileName);
        }

    }
}