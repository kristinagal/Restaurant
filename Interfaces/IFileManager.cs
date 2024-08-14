using Restaurant.Models;

namespace Restaurant.Interfaces
{
    public interface IFileManager
    {
        void GeneratePdfForCustomer(Order order, string path);
        List<T> ReadCsvFile<T>(string fileName, Func<string[], T> map);
        List<T> ReadJsonFile<T>(string fileName);
        List<Order> ReadOrdersFromJsonFile();
        void WriteCheckToFile(string path, string content);
        void WriteCsvFile<T>(string fileName, IEnumerable<T> records, Func<T, string> toCsvLine, string header = null);
        void WriteJsonFile<T>(List<T> data);
    }
}