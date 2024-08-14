namespace Restaurant.Interfaces
{
    public interface IUiService
    {
        bool ConfirmAction(string message);
        string GetEmail();
        string GetEmployeeID();
        int GetMenuSelection(int min, int max);
        int GetQuantity();
    }
}