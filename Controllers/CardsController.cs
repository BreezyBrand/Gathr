using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using CrummyApp.Data;
using CrummyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CrummyApp.Controllers
{
    public class CardsController : Controller
    {
        private readonly ILogger<CardsController> _logger;
        private readonly ApplicationContext _context;

        public CardsController(ApplicationContext context, ILogger<CardsController> logger)
        {
            _logger = logger;
            _context = context;
        }

        //References
        public List<Card> searchCards(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Card> cards = new List<Card>();
            if (!set_code.IsNullOrEmpty() && !card_num.IsNullOrEmpty())
            {
                List<string> setList = set_code.Split(",").ToList();

                cards = _context.Cards.Where(x =>
                        set_code.Contains(x.set)
                        && x.collector_number.Equals(card_num)
                        && x.lang.Equals(lang_code)
                    ).ToList();
            }
            //Search but only set
            else if (!set_code.IsNullOrEmpty() && card_num.IsNullOrEmpty())
            {
                List<string> setList = set_code.Split(",").ToList();

                cards = _context.Cards.Where(x =>
                        set_code.Contains(x.set)
                        && x.lang.Equals(lang_code)
                    ).ToList();
            }
            //Search but only number                            
            else if (set_code.IsNullOrEmpty() && !card_num.IsNullOrEmpty())
            {
                cards = _context.Cards.Where(x =>
                        x.collector_number.Equals(card_num)
                        && x.lang.Equals(lang_code)
                    ).ToList();
            }
            //Search for any and all
            else
            {
                cards = _context.Cards.Where(x =>
                        x.lang.Equals(lang_code)
                    ).ToList();
            }

            return cards;
        }
        public List<CardView> processCards(List<Card> cards)
        {
            List<CardView> processedCards = new List<CardView>();
            foreach (var card in cards.OrderBy(n => n.collector_number))
            {
                CardView thisCard = new CardView(_context);
                thisCard.processCardDetails(card);
                processedCards.Add(thisCard);
            }
            return processedCards;
        }

        //GET Operations
        public PartialViewResult CardDetails(string? set_code, string lang_code = "EN", string card_num = "")
        {
            //Search cards
            List<Card> cards = searchCards(set_code, lang_code, card_num);

            //Process Cards into Card View
            List<CardView> processedCards = processCards(cards);

            return PartialView("_CardDetails", processedCards.ToList());
        }
        public PartialViewResult GetBulk(string? set_code, string lang_code = "EN", string card_num = "")
        {
            //Search cards
            List<Card> cards = searchCards(set_code, lang_code, card_num);

            //Process Cards into Card View
            List<CardView> processedCards = processCards(cards);

            return PartialView("_Bulk", processedCards.ToList());
        }

        public PartialViewResult EZSearch(string raw_cards)
        {
            dynamic raw_cards_list = JsonConvert.DeserializeObject(raw_cards);
            List<EzCard> ezCards = new List<EzCard>();
            foreach (var raw_card_obj in raw_cards_list)
            {
                EzCard ez = new EzCard()
                {
                    CardNum = raw_card_obj.CardNum,
                    SetCode = raw_card_obj.SetCode
                };
                ezCards.Add(ez);
            }

            List<Card> cards = new List<Card>();
            foreach(var ezCard in ezCards)
            {
                if(!ezCard.SetCode.IsNullOrEmpty() && !ezCard.CardNum.IsNullOrEmpty())
                {
                    cards.AddRange(
                            _context.Cards.Where(x=> 
                                x.set.Equals(ezCard.SetCode)
                                && x.collector_number.Equals(ezCard.CardNum)
                            ).ToList()
                        );
                }
            }

            List<CardView> processedCards = processCards(cards);
            return PartialView("_CardDetails", processedCards);
        }
        //POST Operations
        public PartialViewResult AddToInventory(string card_id)
        {
            //Find card
            Card card = _context.Cards.Find(card_id);
            if (card == null)
            {
                throw new InvalidOperationException("No matching Card Id");
            }
            //Add found card to inventory, provide base options
            var new_opts = new InvOptions()
            {
                Card_Id = card.Id,
                Location = "New",
                Language = "EN",
                _confirmed = true,
                confirmed_date = DateTime.Now,
                Mark = ""
            };
            _context.Inventory.Add(new_opts);
            _context.SaveChanges();

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }
        public PartialViewResult UpdateInventory([Bind("Card_Id,confirmed_date,Id,Language,Location,Mark,_confirmed")] InvOptions invUpdate) {

            if (invUpdate.Mark.IsNullOrEmpty())
            {
                invUpdate.Mark = "";
            }
            if (invUpdate.Location.IsNullOrEmpty())
            {
                invUpdate.Location = "";
            }
            invUpdate.confirmed_date = DateTime.Now;

            _context.Inventory.Update(invUpdate);
            _context.SaveChanges();

            Card card = _context.Cards.Find(invUpdate.Card_Id);
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }
    }
}
