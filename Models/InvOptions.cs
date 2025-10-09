using System.ComponentModel.DataAnnotations.Schema;

namespace CrummyApp.Models
{
    [Table("InventoryV2")]
    public class InvOptions
    {
        public int Id { get; set; }
        public string Card_Id {  get; set; }
        public string Mark { get; set; }
        public string Location { get; set; }
        [Column("Confirmed")]
        public int _confirmed { get; set; }
        
        public DateTime confirmed_date { get; set; }
            
    }
}
