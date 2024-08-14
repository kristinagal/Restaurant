using Restaurant.Interfaces;

namespace Restaurant.Models
{
    public class DrinkItem : IMenuItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
