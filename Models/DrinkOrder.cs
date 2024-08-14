namespace Restaurant.Models
{
    public class DrinkOrder
    {
        public DrinkItem DrinkItem { get; set; }
        public int Quantity { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
