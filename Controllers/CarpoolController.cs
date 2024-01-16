using CarpooliDotTN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Redirects to the List action.
        /// </summary>
        /// <returns>RedirectToAction("List")</returns>
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays details of a specific carpool.
        /// </summary>
        /// <param name="id">The unique identifier of the carpool.</param>
        /// <returns>View(carpool)</returns>
        public IActionResult Details(Guid id)
        {
            Carpool carpool = _context.carpools.Include(x => x.Owner).Where(x => x.Id == id).FirstOrDefault();
            return View(carpool);
        }

        /// <summary>
        /// Displays a list of all carpools.
        /// </summary>
        /// <returns>View(carpools)</returns>
        public IActionResult List()
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ICollection<Carpool> carpools = _context.carpools.ToList();
            return View(carpools);
        }

        /// <summary>
        /// Displays the form to add a new carpool.
        /// </summary>
        /// <returns>View()</returns>
        public IActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Adds a new carpool to the database.
        /// </summary>
        /// <param name="carpool">The carpool model containing information about the carpool to be added.</param>
        /// <returns>RedirectToAction("List")</returns>
        [HttpPost]
        public IActionResult Add(Carpool carpool)
        {
            carpool.Id = Guid.NewGuid();
            carpool.CreationTime = DateTime.Now;
            carpool.NumberOfPlaces = 4;
            carpool.IsOpen = true;
            carpool.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _context.carpools.Add(carpool);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays the form to edit a specific carpool.
        /// </summary>
        /// <param name="id">The unique identifier of the carpool to be edited.</param>
        /// <returns>View(carpool) if the user is the owner, RedirectToAction("List") if not.</returns>
        public IActionResult Edit(Guid id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Carpool carpool = _context.carpools.Find(id);
            if (carpool.OwnerId == UserId)
            {
                return View(carpool);
            }
            else
            {
                return RedirectToAction("List");
            }
        }

        /// <summary>
        /// Deletes a specific carpool.
        /// </summary>
        /// <param name="id">The unique identifier of the carpool to be deleted.</param>
        /// <returns>RedirectToAction("Index") if the user is the owner, no action if not.</returns>
        public IActionResult Delete(Guid id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Carpool carpool = _context.carpools.Find(id);
            if (UserId == carpool.OwnerId)
            {
                _context.carpools.Remove(carpool);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays a list of carpools owned by the current user.
        /// </summary>
        /// <returns>View(carpools)</returns>
        public IActionResult MyCarpools()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ICollection<Carpool> carpools = _context.carpools.Where(x => x.OwnerId == userId).ToList();
            return View(carpools);
        }
    }
}
