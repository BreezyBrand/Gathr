using CrummyApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CrummyApp.Models
{
    public class SearchOptions()
    {
        public string name { get; set; } = "";
        public string set_code { get; set; } = "";
        public string lang_code { get; set; } = "EN";
        public string card_num { get; set; } = "";
        public string color { get; set; } = "";
        public string type { get; set; } = "";
        public string location { get; set; } = "";
        public string tags { get; set; } = "";
        public string oracle { get; set; }
        public int skip { get; set; } = 0;
        public bool limit { get; set; } = true;

        public List<Card> MatchedCards(List<Card> cards, ApplicationContext _context)
        {

            if (!name.IsNullOrEmpty())
            {
                cards = cards.Where(x => x.name.ToLower().Contains(name.ToLower()) || x.flavor_name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (!set_code.IsNullOrEmpty())
            {
                List<string> setList = set_code.ToLower().Split(",").ToList();
                cards = cards.Where(x => setList.Contains(x.set)).ToList();
            }

            if (!card_num.IsNullOrEmpty())
            {
                cards = cards.Where(x => x.collector_number.Equals(card_num)).ToList();
            }

            if (!color.IsNullOrEmpty())
            {
                //['B', 'G', 'R', 'U', 'W']
                List<string> colorList = new List<string>();
                color = color.ToLower();

                if (color.Contains("white"))
                {
                    colorList.Add("W");
                }
                if (color.Contains("blue"))
                {
                    colorList.Add("U");
                }
                if (color.Contains("black"))
                {
                    colorList.Add("B");
                }
                if (color.Contains("red"))
                {
                    colorList.Add("R");
                }
                if (color.Contains("green"))
                {
                    colorList.Add("G");
                }


                foreach (var cl in colorList)
                {
                    cards = cards.Where(x => x.colors.Contains(cl)).ToList();
                }
            }

            if (!type.IsNullOrEmpty())
            {
                List<string> typeList = type.Split(",").ToList();
                foreach (var tp in typeList)
                {
                    cards = cards.Where(x => x.type_line.Contains(tp)).ToList();
                }
            }
            
            if (!lang_code.IsNullOrEmpty())
            {
                if (!lang_code.Equals("ALL"))
                {
                    cards = cards.Where(x => x.lang.Equals(lang_code.ToLower())).ToList();
                }
            }
            else
            {
                cards = cards.Where(x => x.lang.Equals("en")).ToList();
            }

            if (!tags.IsNullOrEmpty())
            {
                List<string> tagList = tags.ToLower().Split(",").ToList();

                //Check Inventory
                List<int> InvIds = _context.InvTags.Where(x => tagList.Contains(x.tagName)).Select(n => n.invId).ToList();
                List<string> CardIds = _context.Inventory.Where(x => InvIds.Contains(x.Id)).Select(n => n.Card_Id).ToList();

                //Check Cards (COMING SOON)
                cards = cards.Where(x => CardIds.Contains(x.Id)).ToList();
            }

            if (!oracle.IsNullOrEmpty())
            {
                List<string> terms = oracle.ToLower().Split(" ").ToList();
                cards = cards.Where(x => x.oracle_text.ToLower().Split(" ").Intersect(terms).Any()).ToList();
            }

            return cards;
        }
        
        public bool MatchInventory(CardView card)
        {
            //Semantic Matching
            List<string> locString = location.ToLower().Split(" ").ToList();
            return card.inventory.Where(n => n.Location.ToLower().Split(" ").Intersect(locString).Any()).Any();                
        }
        public CardView FilterInventory(CardView card)
        {
            List<string> locString = location.ToLower().Split(" ").ToList();            
            card.inventory = card.inventory.Where(n => n.Location.ToLower().Split(" ").Intersect(locString).Any()).ToList();                            
            return card;
        }
        public List<CardView> MatchedInventory(List<CardView> cards)
        {
            if (!location.IsNullOrEmpty())
            {
                List<string> locString = location.ToLower().Split(" ").ToList();
                List<CardView> validCards = new List<CardView>();
                foreach (var card in cards)
                {
                    if (card.in_inventory)
                    {
                        card.inventory = card.inventory.Where(n => n.Location.ToLower().Split(" ").Intersect(locString).Any()).ToList();
                        validCards.Add(card);
                    }
                }
                cards = validCards;
            }



            return cards;
        }
    }
}
