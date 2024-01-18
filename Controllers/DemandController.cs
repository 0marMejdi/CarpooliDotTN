using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace CarpooliDotTN.Models;
[Authorize]
public class DemandController : Controller
{
    private CarpooliDbContext _context;
    public DemandController(CarpooliDbContext context)
    {
        _context = context;
    }
    public IActionResult Index() {
        return RedirectToAction("List");
    }
    /**
            * this function is used to list all the demands of a specific carpool
            * it's used in the view of the owner of the carpool to see all the demands made to his carpool
            */
    public IActionResult List(Guid id) {
        if (id == new Guid()) {
            FlashMessage.SendError("Specified id is invalid");	
            return RedirectToAction("List");
        }
        var carpool = _context.carpools
            .Include(c => c.Demands)
            .FirstOrDefault(c => c.Id == id);
        if (carpool == null)
        {
            FlashMessage.SendError("Carpool is not found. Could have been deleted by owner");
            return RedirectToAction("List");
        }
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (userId != carpool.OwnerId)
        {
            FlashMessage.SendError("You Are not the owner of this carpool to see its demands!");
            return RedirectToAction("List");
        }
        return View(carpool.Demands);
    }
    [HttpGet("Demand/List")]
    /**
        * this function is used to list all the demands of the current user
        * it's used in the view of the user to see all the demands he made
        */
    public IActionResult List() {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = _context.Users.Find(userId);
        if (user == null) {
            FlashMessage.SendError("Error while getting your account infromations");
            RedirectToAction("Index", "Home");
        }
        var demands = _context.demands
            .Include(d => d.Carpool)
            .Include(d=>d.Passenger)
            .Where(d => d.PassengerId == userId)
            .ToList()??new List<Demand>();
        return View(demands);
    }
    public IActionResult Apply(Guid id) {
        Demand? demand = new Demand();
        demand.PassengerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        demand.CarpoolId = id;
        demand.status = Demand.Response.pending;
        demand.SubmissionTime = DateTime.Now;
        
        if (_context.carpools.FirstOrDefault(c => c.Id == id) == null)
        {
            FlashMessage.SendError("The Carpool You are trying to apply doesn't exist. Probably Deleted by owner");
            return RedirectToAction("List","Carpool");
        }
        if (!_context.carpools.FirstOrDefault(c => c.Id == id)!.IsOpen)
        {
            FlashMessage.SendError("Can't join this Carpool. This one has closed it's application");
            return RedirectToAction("List", "Carpool");
        }
        if (_context.demands.FirstOrDefault(d => d.PassengerId == demand.PassengerId && d.CarpoolId == demand.CarpoolId) == null)
        {
            FlashMessage.SendError("You have already applied for this carpool");
            return RedirectToAction("List","Carpool");
        }
        if (_context.carpools.FirstOrDefault(c => c.Id == id)!.OwnerId == demand.PassengerId)
        {
            FlashMessage.SendError("You can't apply to your own carpool!");
            return RedirectToAction("List", "Carpool");
        }
        _context.demands.Add(demand);
        _context.SaveChanges();
        FlashMessage.SendSuccess("You have applied for the carpool");
        return RedirectToAction("List");
    }
    public IActionResult Details(Guid id)
    {
        if (!_context.demands.Any(d => d != null && d.Id == id))
        {
            FlashMessage.SendError("The Specified Demand is not Found! \nPassenger may have deleted it");
            return RedirectToAction("List");
        }
        var demand = _context.demands
            .Include(c => c.Carpool)
            .Include(c => c.Passenger)
            .FirstOrDefault(d => d.Id==id);
         
        string ? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != demand.PassengerId && userId != demand.Carpool.OwnerId)
        {
            FlashMessage.SendError("You are not Allowed to see this demand! Must be Sender or Receiver.");
            return RedirectToAction("List");
        }
        ViewBag.IsEditable = (userId == demand.PassengerId); 
        return View(demand);
    }
    
    //TODO : List Action already working instead of this, so this needs to be deleted
    public IActionResult Received()
    {
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        ICollection<Demand> Applications = _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Where(x => x.Carpool.OwnerId == UserId).ToList();
        return View(Applications);
    }
    public IActionResult Delete(Guid id)
    {
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        Demand demand = _context.demands.Find(id);
        if (UserId == demand.PassengerId)
        {
            _context.Remove(demand);
            _context.SaveChanges();
        }
        else
        {
            FlashMessage.SendError("You are not the author of that demand!");
        }
        return RedirectToAction("List");
    }
    public IActionResult Accept(Guid id)
    {
        var demand = _context.demands
            .Include(c => c.Carpool)
            .FirstOrDefault(c => c.Id == id);
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (demand == null)
        {
            FlashMessage.SendError("Demand is not found! Probably deleted by owner");
            return RedirectToAction("Received");
        }

        if (UserId != demand.Carpool.OwnerId)
        {
            FlashMessage.SendError("You can't respond to this demand. It is not sent to you");
            return RedirectToAction("Received");
        }

        if (demand.Carpool.NumberOfPlaces <= 0 )
        {
            FlashMessage.SendError("There is no enough places to accept more demands");
            return RedirectToAction("Received");
        }
        demand.status = Demand.Response.accepted;
        demand.Carpool.NumberOfPlaces --;
        
        // TODO: needs to be discussed: if the places are full does this mean that the offer needs to be closed?
        if (demand.Carpool.NumberOfPlaces == 0)
        {
            demand.Carpool.IsOpen = false;
            FlashMessage.SendInfo("Places are full now. The Offer is closed ");
        }
        _context.SaveChanges();
        return RedirectToAction("Received");
    }
    public IActionResult Decline(Guid id)
    {
        var demand = _context.demands
            .Include(c => c.Carpool)
            .FirstOrDefault(c => c.Id == id);
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (demand == null)
        {
            FlashMessage.SendError("Demand is not found! Probably deleted by owner");
            return RedirectToAction("Received");
        }
        if (UserId != demand.Carpool.OwnerId)
        {
            FlashMessage.SendError("You can't respond to this demand. It is not sent to you");
            return RedirectToAction("Received");
        }
        demand.status = Demand.Response.refused;
        _context.SaveChanges(); 
        return RedirectToAction("Received");
    }
    public IActionResult Sent()
    {
        return List();
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        ICollection<Demand> Applications = _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Where(x => x.PassengerId == UserId).ToList();
        return View(Applications);
    }
}