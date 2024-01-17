﻿using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("Demand/List")]
    /**
        * this function is used to list all the demands of the current user
        * it's used in the view of the user to see all the demands he made
        */
    public IActionResult List() {
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
    public IActionResult List(Guid CarpoolId) {
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
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (userId == carpool.OwnerId)
            return View(carpool.Demands);
        // not the owner
        return List();
    }
    public IActionResult Apply(Guid id) {
        Demand demand = new Demand();
        demand.PassengerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //demand.Carpool = _context.carpools.Find(carpoolId);
        demand.CarpoolId = id;
        demand.status = Demand.Response.pending;
        demand.SubmissionTime = DateTime.Now;
        Console.WriteLine(demand.Id);
        Console.WriteLine(demand.CarpoolId);
        Console.WriteLine(demand.PassengerId);
        Console.WriteLine(demand.SubmissionTime);
        Console.WriteLine(demand.status);


        // TODO: check if the user already applied to this carpool
        _context.demands.Add(demand);
        //a omar fama error mayhebesh yaamli savechanges lena 
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    public IActionResult Details(Guid id) {
        Demand demand = _context.demands.Include(c => c.Carpool).Include(c => c.Passenger).Where(c => c.Id == id).First();
            if (demand == null)
            return List();
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (userId == demand.PassengerId)
            return View(demand);
        // not the owner
        return List();
    }
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

        return RedirectToAction("List");
    }
    public IActionResult Accept(Guid id)
    {
        Demand demand = _context.demands.Include(c => c.Carpool).Where(c => c.Id == id).First();
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (demand != null && 
            UserId == demand.Carpool.OwnerId &&
            demand.Carpool.NumberOfPlaces > 0)
        {
            demand.status = Demand.Response.accepted;
            demand.Carpool.NumberOfPlaces --;
            _context.SaveChanges();
        }
        return RedirectToAction("Received");
    }
    public IActionResult Decline(Guid id)
    {
        Demand demand = _context.demands.Find(id);
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (demand != null && UserId == demand.Carpool.OwnerId)
        {
            demand.status = Demand.Response.refused;
            _context.SaveChanges(); 
        }
        return RedirectToAction("Received");
    }
    public IActionResult Sent()
    {
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        ICollection<Demand> Applications = _context.demands.Include(d => d.Passenger).Include(d => d.Carpool).Where(x => x.PassengerId == UserId).ToList();
        return View(Applications);
    }
}