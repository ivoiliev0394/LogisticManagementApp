using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> SharedLocations()
            => View("~/Views/CompanyPortal/SharedLocations/SharedLocations.cshtml", await _companyPortalService.GetSharedLocationsHomeAsync());

        [HttpGet]
        public async Task<IActionResult> Addresses()
            => View("~/Views/CompanyPortal/SharedLocations/Addresses.cshtml", await _companyPortalService.GetAddressesAsync());

        [HttpGet]
        public async Task<IActionResult> Locations()
            => View("~/Views/CompanyPortal/SharedLocations/Locations.cshtml", await _companyPortalService.GetLocationsAsync());

        [HttpGet]
        public async Task<IActionResult> Warehouses()
            => View("~/Views/CompanyPortal/SharedLocations/Warehouses.cshtml", await _companyPortalService.GetWarehousesAsync());

        [HttpGet]
        public async Task<IActionResult> Terminals()
            => View("~/Views/CompanyPortal/SharedLocations/Terminals.cshtml", await _companyPortalService.GetTerminalsAsync());

        [HttpGet]
        public async Task<IActionResult> Docks()
            => View("~/Views/CompanyPortal/SharedLocations/Docks.cshtml", await _companyPortalService.GetDocksAsync());
    }
}
