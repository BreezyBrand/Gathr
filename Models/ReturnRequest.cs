using CrummyApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CrummyApp.Models
{
    public class ReturnRequest
    {
        public List<Card> rawCards { get; set; }
        public List<CardView> Cards { get; set; }
        public SearchOptions sOpt { get; set; }
        public int max_matches { get; set; }            
        public int LastProcessed { get; set; }        
        public int returnLimit { get; set; }        
        public void Initialize(SearchOptions opt,int lim)
        {
            rawCards = new List<Card>();
            Cards = new List<CardView>();
            sOpt = opt ?? new SearchOptions();
            sOpt.limit = false;
            returnLimit = lim;
        }

        public void GetCards(ApplicationContext context)
        {
            searchCards(context);            
            processCards(context);            
        }

        public void SetCards(ApplicationContext context, List<Card> cards)
        {
            rawCards = cards;          
            processCards(context);            
        }

        private void searchCards(ApplicationContext context)
        {            
            rawCards = sOpt.MatchedCards(context.Cards.ToList(), context);
        }

        private void processCards(ApplicationContext context)
        {
            int maxSingleLoad = returnLimit;
            List<CardView> sortedCards = new List<CardView>();
            foreach (var card in rawCards.OrderBy(n => n.collector_number))
            {
                CardView thisCard = new CardView(context);
                thisCard.SetSortOrder(card);
                sortedCards.Add(thisCard);
            }
            sortedCards = sortedCards.OrderBy(n => n.set).ThenBy(n => n.SortOrder).ToList();
            List<CardView> processedCards = new List<CardView>();
            int count = 0;
            foreach (var card in sortedCards.Skip(sOpt.skip))
            {
                if(Cards.Count() < returnLimit)
                {
                    LastProcessed = sortedCards.IndexOf(card);
                    card.processCardDetails(card.CardObj);
                    if (!sOpt.location.IsNullOrEmpty())
                    {
                        if (sOpt.MatchInventory(card))
                        {
                            sOpt.FilterInventory(card);
                            Cards.Add(card);
                        }
                    } else
                    {
                        Cards.Add(card);
                    }
                }
                else { 
                    break; 
                }
            }

        }
    }
}
