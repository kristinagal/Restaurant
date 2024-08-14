namespace Restaurant.Models
{
    public class FoodOrder
    {
        public FoodItem FoodItem { get; set; }
        public int Quantity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
