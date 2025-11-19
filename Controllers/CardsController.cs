using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using CrummyApp.Data;
using CrummyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
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
        private List<Card> searchCards(SearchOptions sOpts)
        {
            List<Card> cards = _context.Cards.ToList();
            return sOpts.MatchedCards(cards, _context);
        }
        //REVISIT - Need to change how values are passed between search blocks, searching 0-50 may return no matches, which
        //is currently tricking the displays to act as if there are no other matches to load
        //however, if searching for all cards in books, the total number of matches would exceed the maxSingleLoad
        //So we say 50 is the max, but then we've checked 200 cards to get to 50 that match - the current skip logic would say
        //we need to start at 50 - which would then match everything between 50-200 again and that could end at any value above 200
        private List<CardView> processCards(List<Card> cards, SearchOptions sOpt)
        {
            int maxSingleLoad = _config.GetSection("MaxSingleLoad").Get<int>();
            List<CardView> sortedCards = new List<CardView>();
            foreach (var card in cards.OrderBy(n => n.collector_number))
            {
                CardView thisCard = new CardView(_context);
                thisCard.SetSortOrder(card);
                sortedCards.Add(thisCard);
            }
            sortedCards = sortedCards.OrderBy(n => n.set).ThenBy(n => n.SortOrder).Skip(sOpt.skip).ToList();
            List<CardView> processedCards = new List<CardView>();
            int count = 0;
            foreach (var card in sortedCards)
            {
                card.processCardDetails(card.CardObj);
                if (card.in_inventory && !sOpt.location.IsNullOrEmpty())
                {
                    if (sOpt.MatchInventory(card))
                    {
                        processedCards.Add(card);
                    };
                }
                else
                {
                    processedCards.Add(card);
                }
                if (processedCards.Count().Equals(maxSingleLoad) && sOpt.limit)
                {
                    break;
                }
            }
            return sOpt.MatchedInventory(processedCards).Take(maxSingleLoad).ToList();
        }
        private List<Card> getCardsByName(string name)
        {
            string normalized_name = name.ToUpper();
            List<Card> cards = _context.Cards.ToList();
            cards = cards.Where(x => x.name.ToUpper().Equals(normalized_name)).ToList();

            return cards;
        }
        //GET Endpoints        
        public PartialViewResult CardDetails([Bind("set_code,lang_code,card_num,tags,skip,oracle,type,color,location,name")] SearchOptions sOpts)
        {
            ReturnRequest rReq = new ReturnRequest();
            rReq.Initialize(sOpts, _config.GetSection("MaxSingleLoad").Get<int>());
            rReq.GetCards(_context);
            return PartialView("Page/_CardDetails", rReq);
        }
        public PartialViewResult GetLocations([Bind("set_code,lang_code,card_num,tags,skip,oracle,type,color,location,name")] SearchOptions sOpts)
        {
            ReturnRequest rReq = new ReturnRequest();
            rReq.Initialize(sOpts, _config.GetSection("MaxSingleLoad").Get<int>());
            rReq.GetLocations(_context);
            //rReq.GetCards(_context);            

            return PartialView("Page/_Location", rReq);
        }
        public PartialViewResult GetBulk([Bind("set_code,lang_code,card_num,tags,skip,oracle,type,color,location,name")] SearchOptions sOpts)
        {
            ReturnRequest rReq = new ReturnRequest();
            rReq.Initialize(sOpts, _config.GetSection("MaxSingleLoad").Get<int>());
            rReq.sOpt.limit = false;
            rReq.returnLimit = 2000;
            rReq.GetCards(_context);
            return PartialView("Page/_Bulk", rReq);
        }
        public PartialViewResult GetInventory([Bind("set_code,lang_code,card_num,tags,skip,oracle,type,color,location,name")] SearchOptions sOpts)
        {
            ReturnRequest rReq = new ReturnRequest();
            rReq.Initialize(sOpts, _config.GetSection("MaxSingleLoad").Get<int>());
            rReq.GetCards(_context);
            if (sOpts.skip > 0)
            {
                return PartialView("Component/_InventoryRows", rReq);
            }
            else
            {
                return PartialView("Page/_Inventory", rReq);
            }
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
            List<string> quickSets = ezCards.Select(n => n.SetCode.ToUpper()).ToList();
            List<Card> cards = new List<Card>();
            List<Card> allCards = _context.Cards.Where(x => quickSets.Contains(x.set.ToUpper())).ToList();
            foreach (var ezCard in ezCards)
            {
                if (!ezCard.SetCode.IsNullOrEmpty() && !ezCard.CardNum.IsNullOrEmpty())
                {
                    List<Card> matchedCards = allCards.Where(x =>
                                x.set.Equals(ezCard.SetCode)
                                && x.collector_number.Equals(ezCard.CardNum)
                            ).ToList();

                    if (matchedCards.Where(x => x.lang.ToUpper().Equals("EN")).Any())
                    {
                        cards.Add(
                            matchedCards.Where(x => x.lang.ToUpper().Equals("EN")).First()
                        );
                    }
                    else
                    {
                        cards.Add(
                            matchedCards.First()
                        );
                    }


                }
            }
            SearchOptions sOpt = new SearchOptions()
            {
                limit = false,
                skip = 0
            };

            ReturnRequest rReq = new ReturnRequest();
            rReq.Initialize(sOpt, _config.GetSection("MaxSingleLoad").Get<int>());
            rReq.SetCards(_context, cards.Distinct().ToList());
            //List<CardView> processedCards = processCards(cards,sOpt);
            return PartialView("_CardDetails", rReq);
        }
        public PartialViewResult TransactionLog([Bind("set_code,lang_code,card_num,tags,skip,oracle,type,color,location,name")] SearchOptions sOpts)
        {
            List<Transaction> log;
            if (!sOpts.set_code.IsNullOrEmpty() || !sOpts.card_num.IsNullOrEmpty())
            {
                List<Card> cards = searchCards(sOpts);
                List<string> cardIDs = cards.Select(n => n.Id).ToList();
                log = _context.Transactions.Where(x => cardIDs.Contains(x.Card_Id)).ToList();
            }
            else
            {
                log = _context.Transactions.ToList();
            }

            return PartialView("Page/_Transactions", log.OrderByDescending(n => n.TransactionDate).ToList());
        }
        public PartialViewResult AllCardsByName(string name)
        {
            List<Card> cards = getCardsByName(name);
            SearchOptions sOpt = new SearchOptions()
            {
                limit = false,
                skip = 0
            };
            List<CardView> processedCards = processCards(cards, sOpt);
            return PartialView("_Inventory", processedCards.ToList());
        }
        public PartialViewResult GetCardsByLocation([Bind("set_code,lang_code,card_num,tags,skip,oracle,type,color,location,name,toggleType")] SearchOptions sOpts, int LocationId)
        {
            CardLocation Location = _context.Locations.Find(LocationId);
            var InvByLoc = _context.Inventory.Where(x => x.Location.Contains(Location.Name));
            var matchedCards = InvByLoc.Select(x => x.Card_Id).ToList();
            var rawCards = _context.Cards.Where(x => matchedCards.Contains(x.Id)).ToList();
            ReturnRequest rReq = new ReturnRequest();
            rReq.OnlyMatches = true;

            rReq.Initialize(sOpts, InvByLoc.Count());
            rReq.SetCards(_context, rawCards);
            
            if (sOpts.toggleType.Equals("database"))
            {
                return PartialView("Page/_CardDetails", rReq);
            }
            else if (sOpts.toggleType.Equals("inventory"))
            {
                return PartialView("Page/_Inventory", rReq);
            }
            else if (sOpts.toggleType.Equals("bulk"))
            {
                return PartialView("Page/_Bulk", rReq);
            }
            else
            {
                return PartialView("Page/_CardDetails", rReq);
            }
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
            return PartialView("Component/_InventoryDetails", thisCard.inventory);
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
            return PartialView("Component/_InventoryDetails", thisCard.inventory);
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
            return PartialView("Component/_InventoryDetails", thisCard.inventory);
        }
        public string UpdateCardArt(string id, [Bind("small,normal,large,png,art_crop,border_crop")] CardImages imgs)
        {
            if (_context.Images.Where(x => x.Id.Equals(id)).Any())
            {
                CardImages oldImg = _context.Images.Where(x => x.Id.Equals(id)).First();

                oldImg.small = imgs.small;
                oldImg.normal = imgs.small;
                oldImg.large = imgs.small;
                oldImg.png = imgs.small;
                oldImg.art_crop = imgs.small;
                oldImg.border_crop = imgs.small;
                _context.Images.Update(oldImg);

            }
            else
            {
                imgs.Id = id;
                _context.Images.Add(imgs);
            }
            _context.SaveChanges();

            return "Updated";
        }
        public PartialViewResult AddNewLocation([Bind("Name,Type,Count,Tier")] CardLocation newLocation)
        {
            _context.Locations.Add(newLocation);
            _context.SaveChanges();

            ReturnRequest rReq = new ReturnRequest();
            rReq.Initialize(new SearchOptions(), _config.GetSection("MaxSingleLoad").Get<int>());
            rReq.GetLocations(_context);
            return PartialView("Page/_Location", rReq);
        }
        public int UpdateBulk(string CardId, string mark, int newCount, string loc = "Bulk Entry", string lang = "EN")
        {
            List<InvOptions> invCards = _context.Inventory.Where(x => x.Card_Id.Equals(CardId)).ToList();
            //For debugging, leave above
            invCards = invCards.Where(x => x.Mark.Contains(mark)).ToList();

            if (newCount < invCards.Count())
            {
                List<InvOptions> bulkEntryCards = invCards.Where(x => x.Location.Equals("Bulk Entry")).ToList();
                if (bulkEntryCards.Any())
                {
                    _context.Inventory.Remove(bulkEntryCards.Last());
                }
                else
                {
                    return invCards.Count();
                }
            }
            else
            {
                InvOptions newCard = new InvOptions()
                {
                    Card_Id = CardId,
                    Language = lang,
                    Location = loc,
                    Mark = mark,
                    UpdateUser = Environment.MachineName,
                    confirmed_date = DateTime.Now,
                    _confirmed = true
                };
                _context.Inventory.Add(newCard);
            }
            _context.SaveChanges();
            return 0;
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
