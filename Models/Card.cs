using CrummyApp.Data;
using System.ComponentModel.DataAnnotations;

namespace CrummyApp.Models
{    
    public class Card
    {
        [Key]
        public string Id { get; set; }
        public string set { get; set; }
        public string collector_number { get; set; }
        public string lang { get; set; }
        public string name { get; set; }
    }
    public class CardView
        {
            private readonly ApplicationContext _context;
            public CardView(ApplicationContext context)
            {
                _context = context;
            }
            //Basic Card Details            
            public string Id { get; set; }
            public string set { get; set; }
            public string collector_number { get; set; }
            public string lang { get; set; }
            public string name { get; set; }
            //Inventory Specific
            public int in_inventory { get; set; }
            public List<InvOptions> inventory {get;set;}
            public void processCardDetails(Card card)
            {
                this.set = card.set;
                this.name = card.name;
                this.Id = card.Id;
                this.collector_number = card.collector_number;
                this.lang = card.lang;

                this.inventory = _context.Inventory.Where(x => x.Card_Id.Equals(card.Id)).ToList();
            }
        }
}
