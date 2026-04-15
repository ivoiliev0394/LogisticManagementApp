using LogisticManagementApp.Applicationn.Interfaces;
using LogisticManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LogisticManagementApp.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Client"))
                {
                    return RedirectToAction("Dashboard", "ClientPortal");
                }

                if (User.IsInRole("Company"))
                {
                    return RedirectToAction("Profile", "Company");
                }

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminDashboard", "Home");
                }
            }

            var model = await _homeService.GetHomeDataAsync();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string trackingNumber)
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Client"))
            {
                return RedirectToAction("Dashboard", "ClientPortal");
            }
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard", "Home");
            }

            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Company"))
            {
                return RedirectToAction("Profile", "Company");
            }

            var model = await _homeService.TrackShipmentAsync(trackingNumber);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminDashboard()
        {
            return View("~/Views/Home/Dashboards/AdminDashboard.cshtml");
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}