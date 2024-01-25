using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CarpooliDotTN.DAL.IServices;
using CarpooliDotTN.Models;
using CarpooliDotTN.Services;
using CarpooliDotTN.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DemandController : Controller
{
    private readonly IDemandService _demandService;
    private readonly ICarpoolService _carpoolService;

    public string GetCurrentUser()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    }

    public DemandController(IDemandService demandService, ICarpoolService carpoolService)
    {
        _demandService = demandService;
        _carpoolService = carpoolService;
    }

    public IActionResult Index()
    {
        return RedirectToAction("List");
    }

    public async Task<IActionResult> ReceivedByCarpool(Guid id)
    {
        try
        {
            var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

            if (carpool == null)
            {
                FlashMessage.SendError("Carpool not found. Could have been deleted by owner.");
                return RedirectToAction("List");
            }

            string userId = GetCurrentUser();
            if (userId != carpool.OwnerId)
            {
                FlashMessage.SendError("You are not the owner of this carpool to see its demands!");
                return RedirectToAction("List");
            }

            var demands = await _demandService.GetDemandsByCarpoolAsync(id);

            return View("List", DemandCard.GetCardsForReceiverByCarpool(demands));
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("List");
        }
    }

    [HttpGet("Demand/List")]
    public async Task<IActionResult> List()
    {
        try
        {
            var userId = GetCurrentUser();
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (user == null)
            {
                FlashMessage.SendError("Error while getting your account information.");
                return RedirectToAction("Index", "Home");
            }

            var demands = await _demandService.GetDemandsBySenderAsync(userId) ?? new List<Demand>();

            return View(DemandCard.GetCardsForSender(demands));
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Apply(Guid id)
    {
        try
        {
            string userId = GetCurrentUser();

            var carpool = await _carpoolService.GetCarpoolByIdAsync(id);

            if (carpool == null)
            {
                FlashMessage.SendError("The Carpool You are trying to apply doesn't exist. Probably Deleted by owner");
                return RedirectToAction("List", "Carpool");
            }

            if (!carpool.IsOpen)
            {
                FlashMessage.SendError("Can't join this Carpool. This one has closed its application");
                return RedirectToAction("List", "Carpool");
            }

            if (_demandService.IsApplied(userId, id))
            {
                FlashMessage.SendError("You have already applied for this carpool");
                return RedirectToAction("List", "Carpool");
            }

            if (carpool.OwnerId == userId)
            {
                FlashMessage.SendError("You can't apply to your own carpool!");
                return RedirectToAction("List", "Carpool");
            }

            var demand = new Demand
            {
                PassengerId = userId,
                CarpoolId = id,
                status = Demand.Response.pending,
                SubmissionTime = DateTime.Now
            };

            await _demandService.AddDemandAsync(demand);
            FlashMessage.SendSuccess("You have applied for the carpool");
            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("List");
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var demand = await _demandService.GetDemandByIdAsync(id);

            if (demand == null)
            {
                FlashMessage.SendError("The Specified Demand is not Found! Passenger may have deleted it");
                return RedirectToAction("List");
            }

            string userId = GetCurrentUser();
            if (userId != demand.PassengerId && userId != demand.Carpool.OwnerId)
            {
                FlashMessage.SendError("You are not Allowed to see this demand! Must be Sender or Receiver.");
                return RedirectToAction("List");
            }

            ViewBag.IsEditable = (userId == demand.PassengerId);
            bool receiver = userId == demand.Carpool.OwnerId;
            ViewBag.CarpoolCard = new CarpoolCard(demand.Carpool)
            {
                ByOwner = !receiver,
                Apply = !receiver && !(_demandService.IsApplied(demand.PassengerId, demand.CarpoolId)),
                SeeApplication = false,
                SeeCarpool = true,
                DemandId = demand.Id
            };

            return View(demand);
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("List");
        }
    }

    public async Task<IActionResult> Received()
    {
        try
        {
            string userId = GetCurrentUser();
            var applications = await _demandService.GetDemandsByOwnerIdAsync(userId);

            return View("List", DemandCard.GetCardsForReceiver(applications));
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("List");
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            string userId = GetCurrentUser();
            var demand = await _demandService.GetDemandByIdAsync(id);

            if (demand != null && userId == demand.PassengerId)
            {
                await _demandService.DeleteDemandAsync(id);
            }
            else
            {
                FlashMessage.SendError("You are not the author of that demand!");
            }

            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("List");
        }
    }

    public async Task<IActionResult> Accept(Guid id)
    {
        try
        {
            string userId = GetCurrentUser();
            var demand = await _demandService.GetDemandByIdAsync(id);

            if (demand == null)
            {
                FlashMessage.SendError("Demand is not found! Probably deleted by owner");
                return RedirectToAction("Received");
            }

            if (userId != demand.Carpool.OwnerId)
            {
                FlashMessage.SendError("You can't respond to this demand. It is not sent to you");
                return RedirectToAction("Received");
            }

            if (demand.status != Demand.Response.pending)
            {
                FlashMessage.SendError("You can't respond to this demand. It is already responded or cancelled");
                return RedirectToAction("Received");
            }

            if (demand.Carpool.NumberOfPlaces <= 0)
            {
                FlashMessage.SendError("There are no enough places to accept more demands");
                return RedirectToAction("Received");
            }

            demand.status = Demand.Response.accepted;
            demand.Carpool.NumberOfPlaces--;

            // TODO: needs to be discussed: if the places are full does this mean that the offer needs to be closed?
            if (demand.Carpool.NumberOfPlaces == 0)
            {
                demand.Carpool.IsOpen = false;
                FlashMessage.SendInfo("Places are full now. The Offer is closed ");
            }

            await _demandService.UpdateDemandAsync(demand);
            return RedirectToAction("Received");
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("Received");
        }
    }

    public async Task<IActionResult> Decline(Guid id)
    {
        try
        {
            string userId = GetCurrentUser();
            var demand = await _demandService.GetDemandByIdAsync(id);

            if (demand == null)
            {
                FlashMessage.SendError("Demand is not found! Probably deleted by owner");
                return RedirectToAction("Received");
            }

            if (userId != demand.Carpool.OwnerId)
            {
                FlashMessage.SendError("You can't respond to this demand. It is not sent to you");
                return RedirectToAction("Received");
            }

            demand.status = Demand.Response.refused;
            await _demandService.UpdateDemandAsync(demand);

            return RedirectToAction("Received");
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("Received");
        }
    }

    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            string userId = GetCurrentUser();
            var demand = await _demandService.GetDemandByIdAsync(id);

            if (demand == null)
            {
                FlashMessage.SendError("Demand is not found! Probably deleted by owner");
                return RedirectToAction("Received");
            }

            if (userId != demand.PassengerId)
            {
                FlashMessage.SendError("You can't cancel to this demand. It is not you who sent it");
                return RedirectToAction("Received");
            }

            if (demand.status == Demand.Response.refused)
            {
                FlashMessage.SendError("Can't cancel already refused demand!");
                return RedirectToAction("Sent");
            }

            if (demand.status == Demand.Response.cancelled)
            {
                FlashMessage.SendError("Can't Cancel Already Cancelled Demand!");
                return RedirectToAction("Sent");
            }

            if (demand.status != Demand.Response.accepted)
            {
                demand.Carpool.NumberOfPlaces++;
            }

            demand.status = Demand.Response.cancelled;
            await _demandService.UpdateDemandAsync(demand);

            FlashMessage.SendSuccess("You have cancelled this demand!");
            return RedirectToAction("Sent");
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("Received");
        }
    }

    public async Task<IActionResult> Sent()
    {
        try
        {
            string userId = GetCurrentUser();
            var applications = await _demandService.GetDemandsBySenderAsync(userId);

            return View(applications);
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately (log, display an error page, etc.)
            FlashMessage.SendError("An error occurred while processing your request.");
            return RedirectToAction("List");
        }
    }

    [NonAction]
    public async Task<bool> IsApplied(string userId, Guid carpoolId)
    {
        return  _demandService.IsApplied(userId, carpoolId);
    }

    // Other methods as needed...
}
