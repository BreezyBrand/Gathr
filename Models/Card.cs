using CrummyApp.Data;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
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
        public string colors { get; set; }
        public string rarity { get; set; }
        public string flavor_name { get; set; }
        public string card_faces { get; set; }
        public string prices { get; set; }
        public string image_uris { get; set; }
        public string oracle_text { get; set; }

        public string DisplayName()
        {
            if (!flavor_name.IsNullOrEmpty())
            {
                return flavor_name + " (" + name + ")";
            }
            return name;
        }
        public string DecodeLanguage()
        {
            switch (this.lang)
            {
                case "EN":
                    return "English";
                case "ES":
                    return "Spanish";
                case "JA":
                    return "Japanese";
                case "DE":
                    return "German";
                case "PH":
                    return "Phyrexian";
                case "GRC":
                    return "Ancient Greek";
                case "AR":
                    return "Arabic";
                case "ZHS":
                    return "Chinese (Simplified)";
                case "ZHT":
                    return "Chinese (Traditional)";
                case "FR":
                    return "French";
                case "HE":
                    return "Hebrew";
                case "IT":
                    return "Italian";
                case "KO":
                    return "Korean";
                case "LA":
                    return "Latin";
                case "PT":
                    return "Portuguese";
                case "QYA":
                    return "Quenya";
                case "RU":
                    return "Russian";
                case "SA":
                    return "Sanskirt";
                default:
                    return "English";
            }
        }
    }
    public class CardView(ApplicationContext context)
    {
        //Object Creation
        private readonly ApplicationContext _context = context;

        public void SetSortOrder(Card card)
        {
            int sortCheck;
            Int32.TryParse(string.Concat(card.collector_number.Where(Char.IsDigit)), out sortCheck);
            this.SortOrder = sortCheck;
            this.CardObj = card;
            this.Id = card.Id;
            this.set = card.set;
            this.collector_number = card.collector_number;
            this.lang = card.lang;            
            this.name = card.name;                                    
            this.type_line = card.type_line;
            this.rarity = card.rarity;            
            this.alternate_name = card.flavor_name;
            this.oracle_text = card.oracle_text;
        }
        public void processCardDetails(Card card)
        {
            var imgs = _context.Images.Where(x => x.Id.Equals(card.Id)).ToList();
            this.card_faces = card.card_faces;
            this.prices = _context.Pricing.Find(card.Id);
            this.art = imgs.Any() ? imgs.First() : new CardImages();            
            this.hasArt = imgs.Any();

            this.inventory = _context.Inventory.Where(x => x.Card_Id.Equals(card.Id)).ToList();
            this.in_inventory = this.inventory.Any();

            this.colors = new List<string>();
            if (card.colors.Contains("W"))
            {
                this.colors.Add("White");
            }
            if (card.colors.Contains("U"))
            {
                this.colors.Add("Blue");
            }
            if (card.colors.Contains("B"))
            {
                this.colors.Add("Black");
            }
            if (card.colors.Contains("R"))
            {
                this.colors.Add("Red");
            }
            if (card.colors.Contains("G"))
            {
                this.colors.Add("Green");
            }

        }

        //Basic Card Details
        public Card CardObj { get; set; }
        public string Id { get; set; }
        public string set { get; set; }
        public string collector_number { get; set; }
        public int SortOrder { get; set; }
        public string lang { get; set; }
        public string name { get; set; }
        public string type_line { get; set; }
        public List<string> colors { get; set; }
        public string rarity { get; set; }
        public string alternate_name { get; set; }
        public string card_faces { get; set; }
        public string oracle_text { get; set; }
        //Price Variable
        public PriceOptions prices { get; set; }
        //Art Variables
        public CardImages art { get; set; }
        public bool hasArt { get; set; }
        //Inventory Specific
        public bool in_inventory { get; set; }
        public List<InvOptions> inventory { get; set; }
        //Functions
        public string DisplayNameHTML()
        {
            if (!alternate_name.IsNullOrEmpty())
            {
                return alternate_name + "<br/><span class='small'>(" + name + ")</span>";
            }
            return name;
        }
        public string DecodeLanguage()
        {
            switch (this.lang.ToUpper())
            {
                case "EN":
                    return "English";
                case "ES":
                    return "Spanish";
                case "JA":
                    return "Japanese";
                case "DE":
                    return "German";
                case "PH":
                    return "Phyrexian";
                case "GRC":
                    return "Ancient Greek";
                case "AR":
                    return "Arabic";
                case "ZHS":
                    return "Chinese (Simplified)";
                case "ZHT":
                    return "Chinese (Traditional)";
                case "FR":
                    return "French";
                case "HE":
                    return "Hebrew";
                case "IT":
                    return "Italian";
                case "KO":
                    return "Korean";
                case "LA":
                    return "Latin";
                case "PT":
                    return "Portuguese";
                case "QYA":
                    return "Quenya";
                case "RU":
                    return "Russian";
                case "SA":
                    return "Sanskirt";
                default:
                    return "English";
            }
        }
        public string GetPriceRange()
        {
            string priceString = "";
            double out_val;

            double min = 10000000;
            double max = -10000000;

            //usd
            double.TryParse(prices.usd, out out_val);
            if (out_val > 0)
            {
                min = Math.Min(min, out_val);
                max = Math.Max(max, out_val);
            }

            //usd_foil
            double.TryParse(prices.usd_foil, out out_val);
            if (out_val > 0)
            {
                
                min = Math.Min(min, out_val);
                max = Math.Max(max, out_val);
            }
            
            //usd_etch
            double.TryParse(prices.usd_etched, out out_val);
            if (out_val > 0)
            {                
                min = Math.Min(min, out_val);
                max = Math.Max(max, out_val);
            }

            if(min.Equals(10000000) || max.Equals(-10000000))
            {
                return "";
            }

            if (min.Equals(max))
            {
                priceString = String.Format(CultureInfo.CurrentCulture, "{0:C}", min);
            }
            else
            {
                priceString = String.Format(CultureInfo.CurrentCulture, "{0:C}", min) + " - " + String.Format(CultureInfo.CurrentCulture, "{0:C}", max);
            }
            return priceString;
        }
    }
    public class EzCard
    {
        [DefaultValue("")]
        public string Qty { get; set; }
        public string CardName { get; set; }
        public string SetCode { get; set; }
        public string CardNum { get; set; }        
        public string mark { get; set; }
    }
    [Table("Images")]
    public class CardImages
    {
        [Key]
        public string Id { get; set; }
        public string small { get; set; }
        public string normal { get; set; }
        public string large { get; set; }
        public string png { get; set; }
        public string art_crop { get; set; }
        public string border_crop { get; set; }
    }

    public class CardInv
    {
        public string cardInvString { get; set; }
    }
}
