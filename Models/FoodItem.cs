using Restaurant.Interfaces;

namespace Restaurant.Models
{
    public class FoodItem : IMenuItem
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
