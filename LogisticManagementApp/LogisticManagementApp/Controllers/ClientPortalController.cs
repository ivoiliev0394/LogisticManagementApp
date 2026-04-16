using LogisticManagementApp.Applicationn.Interfaces.ClientPortal;
using LogisticManagementApp.Models.ClientPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientPortalController : Controller
    {
        private readonly IClientPortalService _clientPortalService;
        private readonly IClientAddressService _clientAddressService;

        public ClientPortalController(
            IClientPortalService clientPortalService,
            IClientAddressService clientAddressService)
        {
            _clientPortalService = clientPortalService;
            _clientAddressService = clientAddressService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _clientPortalService.GetDashboardAsync(userId);
            return View("~/Views/ClientPortal/ClientDashboard.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _clientPortalService.GetOrdersAsync(userId);
            return View("~/Views/ClientPortal/MyOrders.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> MyShipments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _clientPortalService.GetShipmentsAsync(userId);
            return View("~/Views/ClientPortal/MyShipments.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> MyAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _clientPortalService.GetAddressesAsync(userId);
            return View("~/Views/ClientPortal/MyAddresses.cshtml", model);
        }

        [HttpGet]
        public IActionResult CreateAddress()
        {
            return View("~/Views/ClientPortal/CreateAddress.cshtml", new ClientAddressFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(ClientAddressFormViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/ClientPortal/CreateAddress.cshtml", model);
            }

            await _clientAddressService.CreateAddressAsync(userId, model);
            return RedirectToAction(nameof(MyAddresses));
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _clientAddressService.GetAddressForEditAsync(userId, id);

            if (model == null)
            {
                return NotFound();
            }

            return View("~/Views/ClientPortal/EditAddress.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(ClientAddressFormViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/ClientPortal/EditAddress.cshtml", model);
            }

            var updated = await _clientAddressService.UpdateAddressAsync(userId, model);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyAddresses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var deleted = await _clientAddressService.DeleteAddressAsync(userId, id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyAddresses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultAddress(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var updated = await _clientAddressService.SetDefaultAddressAsync(userId, id);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyAddresses));
        }

        [HttpGet]
        public async Task<IActionResult> TrackShipment(Guid shipmentId)
        {
            var clientUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(clientUserId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = await _clientPortalService.GetShipmentTrackingAsync(clientUserId, shipmentId);

            if (model == null)
            {
                return NotFound();
            }

            return View("~/Views/ClientPortal/TrackShipment.cshtml", model);
        }
    }
}