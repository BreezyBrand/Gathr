using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gathr.Models
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

    [Table("SpreadsheetRow")]
    public class SpreadsheetRow
    {
        public int Id { get; set; }
        public string Qty { get; set; }
        public string _Set { get; set; }
        public string _SetNumber { get; set; }
        public string Mark { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public string Confirmed { get; set; }
        public string Location { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Type3 { get; set; }
        public string Note { get; set; }

    }
}
