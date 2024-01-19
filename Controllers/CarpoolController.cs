using CarpooliDotTN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CarpooliDotTN.Controllers
{
    [Authorize]
    public class CarpoolController : Controller
    {
        private CarpooliDbContext _context { get; set; }

        public CarpoolController(CarpooliDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        [Route("api/carpool/closeoffer/{id}")]
        public async Task<IActionResult> CloseOffer(Guid id)
        {
            Console.WriteLine("Entered API CLOSE");
            var carpool = await _context.carpools
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carpool == null)
            {
                return NotFound();
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (userId != carpool.OwnerId)
            {
                return Forbid();
            }
            carpool.IsOpen = false;
            _context.Update(carpool);
            await _context.SaveChangesAsync();
            return Ok(new { IsOpen = false });
        }

        [HttpPost]
        [Route("api/carpool/reopenoffer/{id}")]
        public async Task<IActionResult> ReOpenOffer(Guid id)
        {
            Console.WriteLine("Entered API Open");
            var carpool = await _context.carpools
                .FirstOrDefaultAsync(c => c.Id == id);

            if (carpool == null)
            {
                return NotFound();
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (userId != carpool.OwnerId || carpool.NumberOfPlaces <= 0)
            {
                return Forbid();
            }
            carpool.IsOpen = true; // Set the IsClosed property to true (or your appropriate property)
            _context.Update(carpool);
            await _context.SaveChangesAsync();
            return Ok(new { IsOpen = true });
        }
        /// <summary>
        /// Redirects to the List action.
        /// </summary>
        /// <returns>RedirectToAction("List")</returns>
        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays details of a specific carpool.
        /// </summary>
        /// <param name="id">The unique identifier of the carpool.</param>
        /// <returns>View(carpool)</returns>
        [AllowAnonymous]
        public IActionResult Details(Guid id)
        {
            var carpool = _context.carpools.Include(x => x.Owner).Where(x => x.Id == id).FirstOrDefault();
            if (carpool == null)
            {
                FlashMessage.SendError("Carpool Not Found! Probably deleted by owner.");
                return RedirectToAction("List");
            }

            ViewBag.isEditable = carpool.OwnerId == User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(carpool);
        }

        /// <summary>
        /// Displays a list of all carpools.
        /// </summary>
        /// <returns>View(carpools)</returns>
        [AllowAnonymous]
        public IActionResult List(string filterDepartureCity, string filterArrivalCity, decimal? filterPrice, int? filterNumberOfPlaces)
        {
            IQueryable<Carpool> carpoolsQuery = _context.carpools.Include(x => x.Owner);

            if (!string.IsNullOrEmpty(filterDepartureCity))
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.DepartureCity == filterDepartureCity);
            }

            if (!string.IsNullOrEmpty(filterArrivalCity))
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.ArrivalCity == filterArrivalCity);
            }

            if (filterPrice.HasValue)
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.Price <= (double)filterPrice.Value);
            }

            if (filterNumberOfPlaces.HasValue)
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.NumberOfPlaces >= filterNumberOfPlaces.Value);
            }

            ICollection<Carpool> carpools = carpoolsQuery.ToList();

            // Pass the filter values to the view for display or further filtering
            ViewData["FilterDepartureCity"] = filterDepartureCity;
            ViewData["FilterArrivalCity"] = filterArrivalCity;
            ViewData["FilterPrice"] = filterPrice;
            ViewData["FilterNumberOfPlaces"] = filterNumberOfPlaces;

            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
            // cheking carpool price is not negative,
            // not empty city names,
            // and departure time needs to be 10 minutes from now at least
            if (carpool.Price < 0 ||
                carpool.DepartureCity == null ||
                carpool.ArrivalCity == null ||
                (carpool.DepartureTime - DateTime.Now).TotalMinutes < 10
               )
            {
                FlashMessage.SendError("Can't Add Carpool! Invalid input.");
                return Add();
            }
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
        [HttpPost]
        public IActionResult Edit(Guid id, Carpool editedCarpool)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Carpool existingCarpool = _context.carpools.Find(id);

            if (existingCarpool == null || existingCarpool.OwnerId != userId)
            {
                return NotFound(); // Carpool not found or unauthorized
            }

            // Check ModelState.IsValid to ensure the submitted form data is valid

            // Update properties of the existing carpool with values from editedCarpool
            existingCarpool.DepartureTime = editedCarpool.DepartureTime;
            existingCarpool.DepartureCity = editedCarpool.DepartureCity;
            existingCarpool.Price = editedCarpool.Price;
            existingCarpool.NumberOfPlaces = editedCarpool.NumberOfPlaces;
            existingCarpool.IsOpen = editedCarpool.IsOpen;
            existingCarpool.Description = editedCarpool.Description;
            // Update other properties as needed
            _context.SaveChanges(); // Save changes to the database

            return RedirectToAction("List"); // Redirect to the list view or another page


            // If ModelState is not valid, return to the edit view with validation errors
            return View(editedCarpool);
        }

        /// <summary>
        /// Deletes a specific carpool.
        /// </summary>
        /// <param name="id">The unique identifier of the carpool to be deleted.</param>
        /// <returns>RedirectToAction("Index") if the user is the owner, no action if not.</returns>
        public IActionResult Delete(Guid id)
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var carpool = _context.carpools.Include(c => c.Demands).FirstOrDefault(c => c.Id == id);
            if (carpool == null)
            {
                FlashMessage.SendError("Specified Carpool not found. Maybe Deleted by owner");
                return RedirectToAction("List");
            }
            if (UserId != carpool.OwnerId)
            {
                FlashMessage.SendError("You are not the owner of this carpool");
                return RedirectToAction("List");
            }
            foreach (var demand in carpool.Demands)
            {
                _context.demands.Remove(demand);
            }
            _context.carpools.Remove(carpool);
            _context.SaveChanges();
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
