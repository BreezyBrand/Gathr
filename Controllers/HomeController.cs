using System.Diagnostics;
using CrummyApp.Data;
using CrummyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CrummyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        private readonly IConfiguration _config;

        public HomeController(ApplicationContext context, ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        public IActionResult Index()
        {
            ReturnRequest rReq = new ReturnRequest();                      
            rReq.Initialize(new SearchOptions(),_config.GetSection("MaxSingleLoad").Get<int>());
            rReq.GetLocations(_context);                          
            return View(rReq);
        }

        public IActionResult RestoreImages()
        {
            List<string> ImgIds = _context.Images.Select(x => x.Id).ToList();
            List<Card> cards = _context.Cards.Where(x => !x.card_faces.Equals("")).ToList();

            cards = cards.Where(x => !ImgIds.Contains(x.Id)).ToList();
            
            List<CardView> preCard = new List<CardView>();
            foreach(var card in cards)
            {
                List<object> cardFaces = JsonConvert.DeserializeObject<List<object>>(card.card_faces);

                dynamic data = JArray.Parse(card.card_faces);
                
                var front = data[0];
                var back = data[1];

                var frontImages = front["image_uris"];
                if (frontImages != null)
                {
                    

                    CardImages fImage = new CardImages()
                    {
                        Id = card.Id,
                        small = front["image_uris"]["small"],
                        normal = front["image_uris"]["normal"],
                        large = front["image_uris"]["large"],
                        png = front["image_uris"]["png"],
                        art_crop = front["image_uris"]["art_crop"],
                        border_crop = front["image_uris"]["border_crop"]
                    };

                    CardImages bImage = new CardImages()
                    {
                        Id = card.Id+"|back",
                        small = back["image_uris"]["small"],
                        normal = back["image_uris"]["normal"],
                        large = back["image_uris"]["large"],
                        png = back["image_uris"]["png"],
                        art_crop = back["image_uris"]["art_crop"],
                        border_crop = back["image_uris"]["border_crop"]
                    };

                    CardView newCarda = new CardView(_context)
                    {
                        Id = card.Id,
                        name = card.name,
                        art = fImage,
                        set = card.set,
                        collector_number = card.collector_number
                    };
                    preCard.Add(newCarda);

                    CardView newCardb = new CardView(_context)
                    {
                        Id = card.Id+"|back",
                        name = card.name,
                        art = bImage,
                        set = card.set,
                        collector_number = card.collector_number
                    };
                    preCard.Add(newCardb);
                    
                }                


                
            }
            
            foreach(var c in preCard)
            {
                if (_context.Images.Where(x => x.Id.Equals(c.Id)).Any())
                {
                    _context.Images.Update(c.art);
                }
                else
                {
                    _context.Images.Add(c.art);
                }
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
            //return View(preCard);
        }

        public IActionResult RecountInv()
        {
            var locations = _context.Locations.ToList();            
            foreach(var loc in locations)
            {
                loc.Count = _context.Inventory.Where(x => x.Location.Contains(loc.Name)).Count();
                _context.Locations.Update(loc); 
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
