using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrummyApp.Models
{
    [Table("InventoryV2")]
    public class InvOptions
    {
        [Key]
        public int Id { get; set; }
        public string Card_Id { get; set; }
        public string Mark { get; set; }
        public string Location { get; set; }
        [Column("Confirmed")]
        public int _confirmed { get; set; }
        public DateTime confirmed_date { get; set; }        
    }
    [Table("PriceHistory")]
    public class PriceOptions
    {
        [Key]
        public string CardId { get; set; }
        public string usd { get; set; }
        public string usd_foil { get; set; }
        public string usd_etched { get; set; }
        public string eur { get; set; }
        public string eur_foil { get; set; }
        public string tix { get; set; }

        public bool IsExpensive(string mark)
        {
            double val;
            if (mark.IsNullOrEmpty())
            {
                double.TryParse(this.usd, out val);
            }
            else if(mark.Equals("f")) {
                double.TryParse(this.usd_foil, out val);
            }
            else
            {
                double.TryParse(this.usd_etched, out val);
            }

            return val > 4.5;
        }

        public string GetDisplay(string? askType)
        {
            if (askType.IsNullOrEmpty())
            {
                if (usd.IsNullOrEmpty())
                {
                    return usd_foil;
                } else
                {
                    return usd;
                }
            }
            else
            {
                switch (askType)
                {
                    case "usd":
                        return usd;
                    case "usd_foil":
                        return usd_foil;
                    case "usd_etched":
                        return usd_etched;
                    case "eur":
                        return eur;
                    case "eur_foil":
                        return eur_foil;
                    case "tix":
                        return tix;
                }
            }

            return "";
        }
    }
}
