using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CarpooliDotTN.Models;
using CarpooliDotTN.Services;

namespace CarpooliDotTN.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmailSender _emailsender;

    public HomeController(ILogger<HomeController> logger , IEmailSender emailSender)
    {
        _logger = logger;
        _emailsender = emailSender;

    }

    public IActionResult Index()
    {
        _emailsender.SendEmail("janick.conn26@ethereal.email", "Hello", "Ok it works");
        return View();
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
