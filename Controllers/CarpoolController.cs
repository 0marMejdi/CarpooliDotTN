using CarpooliDotTN.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarpooliDotTN.Controllers
{
    public class CarpoolController : Controller
    {
        private CarpooliDbContext _context { get; set; }
        public CarpoolController(CarpooliDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return List();
        }
        public IActionResult Details(Guid id)
        {
            Carpool carpool = _context.carpools.Where(x => x.Id == id).FirstOrDefault();
            return View(carpool);
        }
        public IActionResult List()
        {
            ICollection<Carpool> carpools = _context.carpools.ToList();
            return View(carpools);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Carpool carpool)
        {
            if (ModelState.IsValid)
            {
                _context.carpools.Add(carpool);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            { return View(); }
        }
       public IActionResult Edit(Guid id)
        {
            Carpool carpool = _context.carpools.Find(id);
            return View(carpool);
        }
        public IActionResult Delete(Guid id)
        {
            Carpool carpool = _context.carpools.Find(id);
            _context.carpools.Remove(carpool);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult MyCarpools()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ICollection<Carpool> carpools = _context.carpools.Where(x => x.OwnerId == userId).ToList();
            return View(carpools);
        }

    }
}
