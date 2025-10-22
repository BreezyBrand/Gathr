using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
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

        //Functions
        private List<Card> searchCards(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Card> cards = new List<Card>();
            //Search if both set and number are available
            if (!set_code.IsNullOrEmpty() && !card_num.IsNullOrEmpty())
            {
                List<string> setList = set_code.Split(",").ToList();

                cards = _context.Cards.Where(x =>
                        x.set.Equals(set_code)
                        && set_code.Contains(x.set)
                        && x.collector_number.Equals(card_num)
                        && x.lang.Equals(lang_code)
                    ).ToList();
            }
            //Search but only set
            else if (!set_code.IsNullOrEmpty() && card_num.IsNullOrEmpty())
            {
                List<string> setList = set_code.Split(",").ToList();

                cards = _context.Cards.Where(x =>
                        x.set.Equals(set_code)
                        && set_code.Contains(x.set)
                        && x.lang.Equals(lang_code)
                    ).ToList();
            }

            return cards;
        }
        private List<CardView> processCards(List<Card> cards)
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

        //GET Endpoints
        public PartialViewResult CardDetails(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Card> cards = searchCards(set_code, lang_code, card_num);
            List<CardView> processedCards = processCards(cards);
            return PartialView("_CardDetails", processedCards.ToList());
        }
        public PartialViewResult GetBulk(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Card> cards = searchCards(set_code, lang_code, card_num);
            List<CardView> processedCards = processCards(cards);
            return PartialView("_Bulk", processedCards.ToList());
        }
        public PartialViewResult GetInventory(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Card> cards = searchCards(set_code, lang_code, card_num);
            List<CardView> processedCards = processCards(cards);
            return PartialView("_Inventory", processedCards.ToList());
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
            foreach (var ezCard in ezCards)
            {
                if (!ezCard.SetCode.IsNullOrEmpty() && !ezCard.CardNum.IsNullOrEmpty())
                {
                    cards.AddRange(
                            _context.Cards.Where(x =>
                                x.set.Equals(ezCard.SetCode)
                                && x.collector_number.Equals(ezCard.CardNum)
                            ).ToList()
                        );
                }
            }

            List<CardView> processedCards = processCards(cards);
            return PartialView("_CardDetails", processedCards);
        }
        public PartialViewResult TransactionLog(string? set_code, string lang_code = "EN", string card_num = "")
        {
            List<Transaction> log;
            if (!set_code.IsNullOrEmpty() || !card_num.IsNullOrEmpty())
            {
                List<Card> cards = searchCards(set_code, lang_code, card_num);
                List<string> cardIDs = cards.Select(n => n.Id).ToList();
                log = _context.Transactions.Where(x => cardIDs.Contains(x.Card_Id)).ToList();
            }
            else
            {
                log = _context.Transactions.ToList();
            }

            return PartialView("_Transactions", log.OrderByDescending(n => n.TransactionDate).ToList());
        }


        //POST Endpoints
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

            TransactionUpdate_Inventory(new_opts, "Added new copy of " + card.name + " (" + card.Id + ")", "Create");

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }
        public PartialViewResult UpdateInventory([Bind("Card_Id,confirmed_date,Id,Language,Location,Mark,_confirmed")] tempInv inv)
        {
            //Find card
            Card card = _context.Cards.Find(inv.Card_Id);
            InvOptions old = _context.Inventory.Find(inv.Id);

            if (card == null)
            {
                throw new InvalidOperationException("No matching Card Id");
            }
            //Add found card to inventory, provide base options                        
            string updateNote = CompareInventory(old, inv);

            old.Mark = inv.Mark;
            old.confirmed_date = DateTime.Now;
            old._confirmed = inv._confirmed;
            old.Location = inv.Location;
            old.UpdateUser = Environment.MachineName;

            _context.Inventory.Update(old);
            _context.SaveChanges();

            TransactionUpdate_Inventory(inv, updateNote, "Update");

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }
        public PartialViewResult DeleteFromInventory([Bind("Card_Id,confirmed_date,Id,Language,Location,Mark,_confirmed")] InvOptions inv)
        {
            //Find card
            Card card = _context.Cards.Find(inv.Card_Id);
            if (card == null)
            {
                throw new InvalidOperationException("No matching Card Id");
            }
            //Add found card to inventory, provide base options            
            _context.Inventory.Remove(inv);
            string updateNote = "Deleted Inventory ID# " + inv.Id.ToString() + " from inventory";
            TransactionUpdate_Inventory(inv, updateNote, "Delete");
            _context.SaveChanges();

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }

        //Tracking Details
        private string CompareInventory(InvOptions old, tempInv inv)
        {
            string running_note = "";

            if (old.Equals(inv))
            {
                return "No changes. Updating confirmed date.";
            }

            if (!old.Location.Equals(inv.Location))
            {
                running_note += "Location changed from '" + old.Location + "' to '" + inv.Location + "'. ";
            }

            if (!old.Language.Equals(inv.Language))
            {
                running_note += "Language changed from '" + old.Language + "' to '" + inv.Language + "'. ";
            }

            if (!old.Mark.Equals(inv.Mark))
            {
                running_note += "Mark changed from '" + old.Mark + "' to '" + inv.Mark + "'. ";
            }

            if (!old._confirmed.Equals(inv._confirmed))
            {
                running_note += "Card confirmation changed from '" + old._confirmed + "' to '" + inv._confirmed + "'.";
            }

            return running_note.Trim();
        }
        private void TransactionUpdate_Card(Card card, string updateNote, string uType)
        {
            Transaction action = new Transaction()
            {
                Card_Id = card.Id,
                Description = updateNote,
                TransactionDate = DateTime.Now,
                TransactionUser = Environment.MachineName,
                UpdateType = uType
            };
            _context.Transactions.Add(action);
            _context.SaveChanges();

            return;
        }
        private void TransactionUpdate_Inventory(InvOptions invOptions, string updateNote, string uType)
        {
            Transaction action = new Transaction()
            {
                Card_Id = invOptions.Card_Id,
                InventoryId = invOptions.Id,
                Description = updateNote,
                TransactionDate = DateTime.Now,
                TransactionUser = Environment.MachineName,
                UpdateType = uType
            };
            _context.Transactions.Add(action);
            _context.SaveChanges();

            return;
        }


    }
}
