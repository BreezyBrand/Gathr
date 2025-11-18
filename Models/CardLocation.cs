using System.ComponentModel.DataAnnotations.Schema;

namespace CrummyApp.Models
{
    [Table("Locations")]
    public class CardLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        [Column("LastCount")]
        public int? Count { get;set; }
        public int? tier { get;set; }
        public string DisplayName()
        {
            if (Type.Equals("Other"))
            {
                return Name;
            } else
            {
                return Type + " - " + Name;
            }
        }
    }
}
