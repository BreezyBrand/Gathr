using System.ComponentModel.DataAnnotations.Schema;

namespace CrummyApp.Models
{
    [Table("TransactionLog")]
    public class Transaction
    {
        public int Id { get; set; }
        public string Card_Id { get; set; }
        public int InventoryId { get; set; }        
        public string UpdateType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionUser { get; set; }
        public string Description { get; set; }
    }
}
