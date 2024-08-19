using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Restaurant.Interfaces;
using Restaurant.Models;
using Formatting = Newtonsoft.Json.Formatting;

namespace Restaurant.Services
{
    public class FileManager : IFileManager
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
        //csv failu skaitymas, reikia paduoti ir mappinimo funkcija, tada galima skaityti ivairius failus
        {
            var path = Path.Combine(_folderPath, fileName);
            var lines = File.ReadAllLines(path);
            return lines.Skip(1)
                        .Select(line => line.Split(','))
                        .Select(map)
                        .ToList();
        }

        public void WriteCsvFile<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvLine, string header = null)
        //tas pats, plius jei failu nera - sukuriami nauji su atributu pavadinimais pirmoj eilutej
        {
            var path = Path.Combine(_folderPath, fileName);

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path).ToList();

                if (lines.Count > 0)
                {
                    var existingHeader = lines.First();
                    var existingData = lines.Skip(1).ToList();

                    var csvLines = new List<string> { existingHeader };
                    csvLines.AddRange(records.Select(toCsvLine));

                    File.WriteAllLines(path, csvLines);
                }
                else
                {
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
        //json failu skaitymas/rasymas - orderiu informacija restoranui, ten visi orderiai uzdaryti ir dar ne
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

        public void WriteCheckToFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public List<Order> ReadOrdersFromJsonFile()
        {
            return ReadJsonFile<Order>(restaurantChecksFileName);
        }

        public void GeneratePdfForCustomer(Order order, string path)
        {
            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();
            doc.Add(new Paragraph(order.GetOrderSummaryForCustomer()));
            doc.Close();
        }

    }
}