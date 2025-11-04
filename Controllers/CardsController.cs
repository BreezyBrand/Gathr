using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
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
        private readonly IConfiguration _config;

        public CardsController(ApplicationContext context, ILogger<CardsController> logger, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        //Functions
        private List<Card> searchCards(string? set_code, string lang_code = "EN", string card_num = "", string tags = "")
        {
            List<Card> cards = _context.Cards.ToList();
            //Search if both set and number are available
            if (!set_code.IsNullOrEmpty())
            {
                List<string> setList = set_code.ToLower().Split(",").ToList();
                cards = cards.Where(x => setList.Contains(x.set)).ToList();
            }

            if (!card_num.IsNullOrEmpty())
            {
                cards = cards.Where(x => x.collector_number.Equals(card_num)).ToList();
            }

            if (!lang_code.Equals("ALL"))
            {
                cards = cards.Where(x => x.lang.Equals(lang_code.ToLower())).ToList();
            }

            if (!tags.IsNullOrEmpty())
            {
                List<string> tagList = tags.ToLower().Split(",").ToList();
                
                //Check Inventory
                List<int> InvIds = _context.InvTags.Where(x => tagList.Contains(x.tagName)).Select(n=> n.invId).ToList();
                List<string> CardIds = _context.Inventory.Where(x => InvIds.Contains(x.Id)).Select(n=>n.Card_Id).ToList();
                
                //Check Cards (COMING SOON)
                cards = cards.Where(x => CardIds.Contains(x.Id)).ToList();
            }

            return cards;            
        }
        private List<CardView> processCards(List<Card> cards, int skip = 0)
        {
            int maxSingleLoad = _config.GetSection("MaxSingleLoad").Get<int>();
            
            List<CardView> sortedCards = new List<CardView>();            
            foreach (var card in cards.OrderBy(n => n.collector_number))
            {
                CardView thisCard = new CardView(_context);
                thisCard.SetSortOrder(card);
                sortedCards.Add(thisCard);
            }
            sortedCards = sortedCards.OrderBy(n => n.set).ThenBy(n => n.SortOrder).Skip(skip).Take(maxSingleLoad).ToList();
            ViewData["MaxResults"] = sortedCards.Count();
            
            List<CardView> processedCards = new List<CardView>();
            foreach (var card in sortedCards)
            {                
                card.processCardDetails(card.CardObj);
                processedCards.Add(card);
            }

            return processedCards.ToList();
        }
        private List<Card> getCardsByName(string name)
        {
            string normalized_name = name.ToUpper();
            List<Card> cards = _context.Cards.ToList();
            cards = cards.Where(x => x.name.ToUpper().Equals(normalized_name)).ToList();

            return cards;
        }
        //GET Endpoints
        public PartialViewResult CardDetails(string? set_code, string lang_code = "EN", string card_num = "", string tags = "",int skip = 0)
        {
            List<Card> cards = searchCards(set_code, lang_code, card_num, tags);
            List<CardView> processedCards = processCards(cards,skip);
            return PartialView("_CardDetails", processedCards.ToList());
        }
        public PartialViewResult GetBulk(string? set_code, string lang_code = "EN", string card_num = "", string tags = "",int skip = 0)
        {
            List<Card> cards = searchCards(set_code, lang_code, card_num, tags);
            List<CardView> processedCards = processCards(cards, skip);
            return PartialView("_Bulk", processedCards.ToList());
        }
        public PartialViewResult GetInventory(string? set_code, string lang_code = "EN", string card_num = "", string tags = "",int skip = 0)
        {
            List<Card> cards = searchCards(set_code, lang_code, card_num, tags);
            List<CardView> processedCards = processCards(cards,skip);
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
                    List<Card> matchedCards = _context.Cards.Where(x =>
                                x.set.Equals(ezCard.SetCode)
                                && x.collector_number.Equals(ezCard.CardNum)
                            ).ToList();

                    if(matchedCards.Where(x => x.lang.ToUpper().Equals("EN")).Any())
                    {
                        cards.Add(
                            matchedCards.Where(x => x.lang.ToUpper().Equals("EN")).First()
                        );
                    } else
                    {
                        cards.Add(
                            matchedCards.First()
                        );
                    }

                    
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

        public PartialViewResult AllCardsByName(string name)
        {
            List<Card> cards = getCardsByName(name);
            List<CardView> processedCards = processCards(cards);
            return PartialView("_Inventory", processedCards.ToList());
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
                UpdateUser = Environment.MachineName,
                Mark = ""
            };
            _context.Inventory.Add(new_opts);
            _context.SaveChanges();

            TransactionUpdate_Inventory(new_opts, "Added new copy of " + card.DisplayName() + " (" + card.Id + ")", "Create");

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
            updateNote = "Updated " + card.DisplayName() + " (Inventory ID# " + inv.Id.ToString() + "). " + updateNote;

            old.Mark = inv.Mark.IsNullOrEmpty() ? "" : inv.Mark;
            old.confirmed_date = DateTime.Now;
            old._confirmed = inv._confirmed;
            old.Location = inv.Location.IsNullOrEmpty() ? "" : inv.Location;
            old.UpdateUser = Environment.MachineName;

            _context.Inventory.Update(old);
            _context.SaveChanges();

            TransactionUpdate_Inventory(inv, updateNote, "Update");

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }
        public CardView CloneInventory([Bind("Card_Id,confirmed_date,Id,Language,Location,Mark,_confirmed")] InvOptions inv)
        {
            //Find card
            Card card = _context.Cards.Find(inv.Card_Id);

            if (card == null)
            {
                throw new InvalidOperationException("No matching Card Id");
            }
            InvOptions newInv = new InvOptions()
            {
                Card_Id = card.Id,
                confirmed_date = DateTime.Now,
                Location = inv.Location,
                UpdateUser = Environment.MachineName,
                Language = inv.Language,
                Mark = inv.Mark,
                _confirmed = inv._confirmed
            };

            //Add found card to inventory, provide base options                        
            TransactionUpdate_Inventory(newInv, "Added new copy of " + card.DisplayName() + " (" + card.Id + ")", "Create");
            _context.Inventory.Add(newInv);
            _context.SaveChanges();

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return thisCard;
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
            string updateNote = "Deleted " + card.DisplayName() + " (Inventory ID# " + inv.Id.ToString() + ") from inventory";
            TransactionUpdate_Inventory(inv, updateNote, "Delete");
            _context.SaveChanges();

            //Reset view
            CardView thisCard = new CardView(_context);
            thisCard.processCardDetails(card);
            return PartialView("_InventoryDetails", thisCard.inventory);
        }
        public string UpdateCardArt(string id, [Bind("small,normal,large,png,art_crop,border_crop")] CardImages imgs)
        {
            if(_context.Images.Where(x => x.Id.Equals(id)).Any())
            {
                CardImages oldImg = _context.Images.Where(x => x.Id.Equals(id)).First();

                oldImg.small = imgs.small;
                oldImg.normal = imgs.small;
                oldImg.large = imgs.small;
                oldImg.png = imgs.small;
                oldImg.art_crop = imgs.small;
                oldImg.border_crop = imgs.small;
                _context.Images.Update(oldImg);

            } else
            {
                imgs.Id = id;
                _context.Images.Add(imgs);
            }
            _context.SaveChanges();

            return "Updated";
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
