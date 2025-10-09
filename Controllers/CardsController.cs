using System.Diagnostics;
using System.Net;
using CrummyApp.Data;
using CrummyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

        public PartialViewResult CardDetails(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Card> cards = new List<Card>();
            //Search if both set and number are available
            if (!set_code.IsNullOrEmpty() && !card_num.IsNullOrEmpty())
            {
                cards = _context.Cards.Where(x =>
                        x.set.Equals(set_code)
                        && x.collector_number.Equals(card_num)
                        && x.lang.Equals(lang_code)
                    ).ToList();
            }
            //Search but only set
            else if (!set_code.IsNullOrEmpty() && card_num.IsNullOrEmpty())
            {
                cards = _context.Cards.Where(x =>
                        x.set.Equals(set_code)
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

            List<CardView> processedCards = new List<CardView>();
            foreach (var card in cards.Take(20))
            {
                CardView thisCard = new CardView(_context);
                thisCard.processCardDetails(card);
                processedCards.Add(thisCard);
            }

            return PartialView("_CardDetails", processedCards.ToList());
        }

        public PartialViewResult AddToInventory(string card_id)
        {
            //Find card
            Card card = _context.Cards.Find(card_id);
            if(card == null)
            {
                throw new InvalidOperationException("No matching Card Id");
            }
            //Add found card to inventory, provide base options
            var new_opts = new InvOptions()
            {
                Card_Id = card.Id,
                Location = "New",
                _confirmed = 1,
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
    }
}
