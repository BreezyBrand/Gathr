using CrummyApp.Data;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        public string type_line { get; set; }
        public string rarity { get; set; }
        public string flavor_name { get; set; }
        public string prices { get; set; }
        public string image_uris { get; set; }
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
        public int SortOrder { get; set; }
        public string lang { get; set; }
        public string name { get; set; }
        public string type_line { get; set; }
        public string rarity { get; set; }
        public string alternate_name { get; set; }
        //Price Variable
        public PriceOptions prices { get; set; }
        //Art Variables
        public JsonNode art { get; set; }
        public bool hasArt { get; set; }
        //Inventory Specific
        public int in_inventory { get; set; }
        public List<InvOptions> inventory { get; set; }
        public void processCardDetails(Card card)
        {
            int sortCheck;
            Int32.TryParse(string.Concat(card.collector_number.Where(Char.IsDigit)), out sortCheck);

            this.set = card.set;
            this.name = card.name;
            this.Id = card.Id;
            this.collector_number = card.collector_number;
            this.SortOrder = sortCheck;
            this.lang = card.lang;
            this.type_line = card.type_line;
            this.rarity = card.rarity;
            this.alternate_name = card.flavor_name;
            this.hasArt = false;

            this.prices = _context.Pricing.Find(card.Id);

            var image_holder = card.image_uris.Replace('\'', '"').Replace("None", "\"\"");
            if (!card.image_uris.IsNullOrEmpty())
            {
                this.art = JsonNode.Parse(image_holder);
                try
                {
                    var img_to_use = this.art["png"];
                    this.hasArt = true;
                    img_to_use = img_to_use;
                }
                catch (Exception e)
                {
                }
            }

            this.inventory = _context.Inventory.Where(x => x.Card_Id.Equals(card.Id)).ToList();
        }
    }
    public class EzCard
    {
        public string SetCode { get; set; }
        public string CardNum { get; set; }
    }
    [Table("Images")]
    public class CardImages
    {
        public string Id { get; set; }
        public string small {  get; set; }
        public string normal {  get; set; }
        public string large {  get; set; }
        public string png {  get; set; }
        public string art_crop {  get; set; }
        public string border_crop {  get; set; }
    }
}
