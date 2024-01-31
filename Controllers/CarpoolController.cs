using CarpooliDotTN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CarpooliDotTN.ViewModels;
using CarpooliDotTN.DAL.IServices;

[Authorize]
public class CarpoolController : Controller
{
    private readonly ICarpoolService _carpoolService;
    private readonly IDemandService _demandService;

    public string GetCurrentUser()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    }

    public CarpoolController(ICarpoolService carpoolService, IDemandService demandService)
    {
        _carpoolService = carpoolService;
        _demandService = demandService;
    }

    [HttpPost]
    [Route("api/carpool/closeoffer/{id}")]
    public async Task<IActionResult> CloseOffer(Guid id)
    {
        Console.WriteLine("Entered API CLOSE");
        var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

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
        await _carpoolService.UpdateCarpoolAsync(carpool);

        return Ok(new { IsOpen = false });
    }

    [HttpPost]
    [Route("api/carpool/reopenoffer/{id}")]
    public async Task<IActionResult> ReOpenOffer(Guid id)
    {
        Console.WriteLine("Entered API Open");
        var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

        if (carpool == null)
        {
            return NotFound();
        }

        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (userId != carpool.OwnerId || carpool.NumberOfPlaces <= 0)
        {
            return Forbid();
        }

        carpool.IsOpen = true;
        await _carpoolService.UpdateCarpoolAsync(carpool);

        return Ok(new { IsOpen = true });
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return RedirectToAction("List");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

        if (carpool == null)
        {
            FlashMessage.SendError("Carpool Not Found! Probably deleted by owner.");
            return RedirectToAction("List");
        }

        ViewBag.isEditable = carpool.OwnerId == User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return View(carpool);
    }

    [AllowAnonymous]
    public async Task<IActionResult> List()
    {
        return RedirectToAction("ListAll");
    }

    public async Task<IActionResult> ListAll(string filterDepartureCity, string filterArrivalCity, string ownerFilter, string availabiltyFilter, decimal? filterPrice, int? filterNumberOfPlaces)
    {
        IQueryable<Carpool> carpoolsQuery = await _carpoolService.GetQueryableCarpoolsWithDemandsAsync();

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

        if (!string.IsNullOrEmpty(ownerFilter))
        {
            if (ownerFilter == "me")
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.OwnerId == GetCurrentUser());
            }
            else if (ownerFilter == "others")
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.OwnerId != GetCurrentUser());
            }
        }

        if (!string.IsNullOrEmpty(availabiltyFilter))
        {
            if (availabiltyFilter == "open")
            {
                carpoolsQuery = carpoolsQuery.Where(c => c.IsOpen);
            }
            else if (availabiltyFilter == "closed")
            {
                carpoolsQuery = carpoolsQuery.Where(c => !c.IsOpen);
            }
        }

        var carpools = await carpoolsQuery.ToListAsync();

        ViewData["FilterDepartureCity"] = filterDepartureCity;
        ViewData["FilterArrivalCity"] = filterArrivalCity;
        ViewData["FilterPrice"] = filterPrice;
        ViewData["FilterNumberOfPlaces"] = filterNumberOfPlaces;
        ViewData["OwnerFilter"] = ownerFilter;
        ViewData["AvailabiltyFilter"] = availabiltyFilter;
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return View(CarpoolCard.GetCardsForAll(carpools, UserId));

    }

    public async Task<IActionResult> Add()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Carpool carpool)
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
            return RedirectToAction("Add");
        }
        carpool.CreationTime = DateTime.Now;
        carpool.NumberOfPlaces = 4;
        carpool.IsOpen = true;
        carpool.OwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _carpoolService.AddCarpoolAsync(carpool);
        return RedirectToAction("ListAll");
    }
    public async Task<IActionResult> Edit(Guid id)
    {
        Carpool carpool = _carpoolService.GetCarpoolByIdAsync(id).Result;

        if (carpool == null || carpool.OwnerId != GetCurrentUser())
        {
            FlashMessage.SendError("You do not have permission to edit this carpool.");
            return RedirectToAction("List");
        }
        return View(carpool);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Carpool editedCarpool)
    {
        string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Carpool existingCarpool = _carpoolService.GetCarpoolByIdAsync(id).Result;

        if (existingCarpool == null || existingCarpool.OwnerId != userId)
        {
            return NotFound(); // Carpool not found or unauthorized
        }
        await _carpoolService.UpdateCarpoolAsync(editedCarpool);
        return RedirectToAction("List"); // Redirect to the list view or another page


        // If ModelState is not valid, return to the edit view with validation errors
        return View(editedCarpool);
    }
    
    public  IActionResult Delete(Guid id)
    {
        string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        Carpool carpool = _carpoolService.GetCarpoolByIdAsync(id).Result;
        if (carpool == null)
        {
            FlashMessage.SendError("Specified Carpool not found. Maybe Deleted by owner");
            return RedirectToAction("ListAll");
        }
        if (UserId != carpool.OwnerId)
        {
            FlashMessage.SendError("You are not the owner of this carpool");
            return RedirectToAction("ListAll");
        }
        foreach (var demand in carpool.Demands)
        {
            _demandService.DeleteDemandAsync(demand.Id);
        }
        _carpoolService.DeleteCarpoolAsync(carpool.Id);
        return RedirectToAction("ListAll");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Join(Guid id)
    {
        var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

        if (carpool == null || !carpool.IsOpen || carpool.OwnerId == GetCurrentUser())
        {
            FlashMessage.SendError("You cannot join this carpool.");
            return RedirectToAction("List");
        }

        ViewBag.CarpoolCard = new CarpoolCard(carpool) { ByOwner = false, Apply = !IsApplied(GetCurrentUser(), id), SeeApplication = false, SeeCarpool = true };
        return View(carpool);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> JoinConfirmation(Guid id)
    {
        var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

        if (carpool == null || !carpool.IsOpen || carpool.OwnerId == GetCurrentUser())
        {
            FlashMessage.SendError("You cannot join this carpool.");
            return RedirectToAction("List");
        }

        var demand = new Demand
        {
            PassengerId = GetCurrentUser(),
            CarpoolId = id,
            status = Demand.Response.pending,
            SubmissionTime = DateTime.Now
        };

        await _demandService.AddDemandAsync(demand);

        FlashMessage.SendSuccess("You have applied to join the carpool");
        return RedirectToAction("List");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Leave(Guid id)
    {
        var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

        if (carpool == null || carpool.OwnerId == GetCurrentUser())
        {
            FlashMessage.SendError("You cannot leave this carpool.");
            return RedirectToAction("List");
        }

        ViewBag.CarpoolCard = new CarpoolCard(carpool) { ByOwner = false, Apply = false, SeeApplication = false, SeeCarpool = true };
        return View(carpool);
    }



    [NonAction]
    [AllowAnonymous]
    public bool IsApplied(string userId, Guid carpoolId)
    {
        return _demandService.IsApplied(userId, carpoolId);
    }
}
