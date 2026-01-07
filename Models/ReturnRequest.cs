using Gathr.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace Gathr.Models
{
    public class ReturnRequest
    {
        public List<Card> rawCards { get; set; }
        public List<CardView> Cards { get; set; }
        public List<CardLocation> Locations { get; set; }
        public SearchOptions sOpt { get; set; }
        public int max_matches { get; set; }
        public int LastProcessed { get; set; }
        public int returnLimit { get; set; }
        public bool OnlyMatches { get; set; }
        public void Initialize(SearchOptions opt, int lim)
        {
            rawCards = new List<Card>();
            Cards = new List<CardView>();
            Locations = new List<CardLocation>();
            sOpt = opt ?? new SearchOptions();
            sOpt.limit = false;
            returnLimit = lim;
        }

        public void GetCards(ApplicationContext context)
        {
            searchCards(context);
            GetLocations(context);
            processCards(context);
            return;
        }

        public void SetCards(ApplicationContext context, List<Card> cards)
        {
            rawCards = cards;
            max_matches = cards.Count();
            processCards(context);
            return;
        }

        private void searchCards(ApplicationContext context)
        {
            if (sOpt.toggleType.Equals("inventory"))
            {
                List<string> cardsInInv = context.Inventory.Select(x => x.Card_Id).Distinct().ToList();
                List<Card> cards = context.Cards.Where(x => cardsInInv.Contains(x.Id)).ToList();
                rawCards = sOpt.MatchedCards(cards, context);
            } else
            {
                rawCards = sOpt.MatchedCards(context.Cards.ToList(), context);
            }
            return;
        }

        private void processCards(ApplicationContext context)
        {
            //Prefilter if any of the results are in the selected locations
            if (!sOpt.location.Equals("Any"))
            {
                var markedLocations = sOpt.location.Split(',');
                List<string> matchedCardIds = new List<string>();
                List<string> cleanLocation = new List<string>();
                foreach (var loc in markedLocations)
                {
                    if (loc.Contains("Other"))
                    {
                        cleanLocation.Add(loc.Replace("Other - ", ""));
                    } else
                    {
                        cleanLocation.Add(loc);
                    }
                }
                var matches = context.Inventory.Where(x => cleanLocation.Contains(x.Location)).Distinct().Select(n => n.Card_Id).ToList();
                matchedCardIds.AddRange(matches);
                matchedCardIds = matchedCardIds.Distinct().ToList();
                rawCards = rawCards.Where(x => matchedCardIds.Contains(x.Id)).ToList();
            }

            int maxSingleLoad = returnLimit;
            List<CardView> sortedCards = new List<CardView>();
            //Get the ordering of the results
            foreach (var card in rawCards.OrderBy(n => n.collector_number))
            {
                CardView thisCard = new CardView(context);
                thisCard.SetSortOrder(card);
                sortedCards.Add(thisCard);
            }
            sortedCards = sortedCards.OrderBy(n => n.set).ThenBy(n => n.SortOrder).ToList();
            //Process the remaining cards
            List<CardView> processedCards = new List<CardView>();
            int count = 0;
            max_matches = sortedCards.Count();
            foreach (var card in sortedCards.Skip(sOpt.skip))
            {
                if (Cards.Count() < returnLimit)
                {
                    LastProcessed = sortedCards.IndexOf(card)+1;
                    Debug.Write("Processing card " + LastProcessed.ToString());
                    card.processCardDetails(card.CardObj);
                    if (!sOpt.location.Equals("Any"))
                    {
                        if (sOpt.MatchInventory(card))
                        {
                            sOpt.FilterInventory(card);
                            Cards.Add(card);
                        }
                    }
                    else
                    {
                        Cards.Add(card);
                    }
                }
                else
                {
                    break;
                }
            }

            
        }

        public void GetLocations(ApplicationContext context)
        {
            Locations = context.Locations.OrderBy(n => n.Name).ToList();
        }

        public List<CardView> FindInLocation(string location)
        {
            List<CardView> filteredCards = new List<CardView>();

            filteredCards = this.Cards.Where(x => x.inventory.Where(n => n.Location.ToLower().Equals(location)).Count() > 0).ToList();

            foreach (CardView fCard in filteredCards)
            {
                List<InvOptions> MatchInv = new List<InvOptions>();
                MatchInv = fCard.inventory.Where(x => x.Location.ToLower().Equals(location.ToLower())).ToList();
                if (MatchInv.Any())
                {
                    fCard.inventory.Clear();
                    fCard.inventory = MatchInv;
                }
            }
            return filteredCards;
        }
    }
}
