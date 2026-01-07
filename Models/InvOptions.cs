using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gathr.Models
{
    [Table("InventoryV2")]
    public class InvOptions
    {
        [Key]
        public int Id { get; set; }
        public string Card_Id { get; set; }        
        public string Mark { get; set; }
        public string Location { get; set; }
        public string Language { get; set; }
        public string UpdateUser { get; set; }
        [Column("Confirmed")]
        public bool _confirmed { get; set; }
        public DateTime confirmed_date { get; set; }
    }

    public class tempInv : InvOptions
    {

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
            else if (mark.Equals("f"))
            {
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
                }
                else
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

    public class Marks
    {
        //f-etch, -, f-pre, f-pp, pp, f, f list, *pp*, list
        public bool foil { get; set; }
        public bool etched { get; set; }
        public bool promo { get; set; }
        public bool list { get; set; }
        override public string ToString()
        {
            string markString = "";
            markString += foil ? "f" : "";
            markString += etched ? "-etch" : "";
            markString += promo ? "-pp" : "";
            markString += list ? " list" : "";

            return markString.Trim();
        }

        public void ParseMarks(string mark)
        {
            etched = false;
            foil = false;
            promo = false;
            list = false;

            if (mark.Contains("etch"))
            {
                etched = true;
            }

            if (mark.Contains("pp") || mark.Contains("pre"))
            {
                promo = true;
            }

            if (mark.Contains("list"))
            {
                list = true;
            }

            if (mark.Contains("f"))
            {
                foil = true;
            }
        }
    }

    [Table("InvTags")]
    public class InvTag
    {
        public int ID { get; set; }
        public int invId { get; set; }
        public string tagName { get; set; }

    }
}
