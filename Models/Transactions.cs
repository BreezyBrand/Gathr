namespace CrummyApp.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public int Card_Id { get; set; }
        public int InventoryId { get; set; }        
        public string UpdateType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
    }
}
