using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace CarpooliDotTN.Models;
[Authorize]
public class DemandController : Controller
{ 
    private CarpooliDbContext _context ; 
    public DemandController(CarpooliDbContext context)
    {
        _context = context; 
    }
    public IActionResult Index(){
        return List();
    }

    [HttpGet("Demand/List")]
    /**
        * this function is used to list all the demands of the current user
        * it's used in the view of the user to see all the demands he made
        */
    public IActionResult List(){
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = _context.Users.Find(userId);
        if (user != null) {
            var demands = _context.demands
                .Include(d => d.Carpool)
                .Where(d => d.PassengerId == userId)
                .ToList();
            return View(demands);
        }
        return View(new List<Demand>());
    }
    /**
        * this function is used to list all the demands of a specific carpool
        * it's used in the view of the owner of the carpool to see all the demands made to his carpool
        */
    public IActionResult List (Guid CarpoolId){
        if (CarpoolId == new Guid()) {
            // empty guid	
            return List();
        }
        var carpool = _context.carpools
            .Include(c => c.Demands)
            .Where(c => c.Id == CarpoolId)
            .FirstOrDefault();
        if (carpool == null) 
            // invalid carpool id
            return List();
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value??"";
        if (userId == carpool.OwnerId)
            return View(carpool.Demands);
        // not the owner
        return List();
    } 
    public IActionResult Apply (Guid carpoolId){
        Demand demand = new Demand();
        demand.PassengerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ; 
        demand.CarpoolId = carpoolId;
        demand.status = Demand.Response.pending;
        demand.SubmissionTime = DateTime.Now;
        // TODO: check if the user already applied to this carpool
        _context.demands.Add(demand);
        _context.SaveChanges();
        return List();
    }

    public IActionResult Details (Guid id){
        var demand = _context.demands
            .Include(d => d.Carpool)
            .Where(d => d.Id == id)
            .FirstOrDefault();
        if (demand == null) 
            return List();
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value??"";
        if (userId == demand.PassengerId)
            return View(demand);
        // not the owner
        return List();
    }   
}