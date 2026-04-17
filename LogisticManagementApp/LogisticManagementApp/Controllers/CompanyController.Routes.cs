using LogisticManagementApp.Models.CompanyPortal.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> RouteManagement()
            => View(
                "~/Views/CompanyPortal/Routes/RouteManagement.cshtml",
                await _companyPortalService.GetRoutesHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Routes()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/Routes.cshtml",
                await _companyPortalService.GetRoutesAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoute()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/CreateRoute.cshtml",
                await _companyPortalService.GetCreateRouteModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoute(RouteCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Routes/CreateRoute.cshtml", model);

            var id = await _companyPortalService.CreateRouteAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на route.";
                return View("~/Views/CompanyPortal/Routes/CreateRoute.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route беше добавен успешно.";
            return RedirectToAction(nameof(Routes));
        }

        [HttpGet]
        public async Task<IActionResult> EditRoute(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRouteForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Routes/EditRoute.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoute(RouteEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Routes/EditRoute.cshtml", model);

            var success = await _companyPortalService.UpdateRouteAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на route.";
                return View("~/Views/CompanyPortal/Routes/EditRoute.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route беше обновен успешно.";
            return RedirectToAction(nameof(Routes));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteRouteAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Route беше изтрит успешно."
                    : "Неуспешно изтриване на route.";

            return RedirectToAction(nameof(Routes));
        }

        [HttpGet]
        public async Task<IActionResult> RouteStops()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/RouteStops.cshtml",
                await _companyPortalService.GetRouteStopsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRouteStop()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/CreateRouteStop.cshtml",
                await _companyPortalService.GetCreateRouteStopModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRouteStop(RouteStopCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateRouteStopModelAsync(companyId.Value);
                model.RouteOptions = reload.RouteOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRouteStop.cshtml", model);
            }

            var id = await _companyPortalService.CreateRouteStopAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на route stop.";

                var reload = await _companyPortalService.GetCreateRouteStopModelAsync(companyId.Value);
                model.RouteOptions = reload.RouteOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRouteStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route stop беше добавен успешно.";
            return RedirectToAction(nameof(RouteStops));
        }

        [HttpGet]
        public async Task<IActionResult> EditRouteStop(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRouteStopForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Routes/EditRouteStop.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRouteStop(RouteStopEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetRouteStopForEditAsync(companyId.Value, model.Id);
                if (reload != null)
                {
                    model.RouteOptions = reload.RouteOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Routes/EditRouteStop.cshtml", model);
            }

            var success = await _companyPortalService.UpdateRouteStopAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на route stop.";
                return View("~/Views/CompanyPortal/Routes/EditRouteStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route stop беше обновен успешно.";
            return RedirectToAction(nameof(RouteStops));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRouteStop(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteRouteStopAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Route stop беше изтрит успешно."
                    : "Неуспешно изтриване на route stop.";

            return RedirectToAction(nameof(RouteStops));
        }

        [HttpGet]
        public async Task<IActionResult> RoutePlans()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/RoutePlans.cshtml",
                await _companyPortalService.GetRoutePlansAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoutePlan()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/CreateRoutePlan.cshtml",
                await _companyPortalService.GetCreateRoutePlanModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoutePlan(RoutePlanCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateRoutePlanModelAsync(companyId.Value);
                model.RouteOptions = reload.RouteOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRoutePlan.cshtml", model);
            }

            var id = await _companyPortalService.CreateRoutePlanAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на route plan.";

                var reload = await _companyPortalService.GetCreateRoutePlanModelAsync(companyId.Value);
                model.RouteOptions = reload.RouteOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRoutePlan.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route plan беше добавен успешно.";
            return RedirectToAction(nameof(RoutePlans));
        }

        [HttpGet]
        public async Task<IActionResult> EditRoutePlan(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRoutePlanForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Routes/EditRoutePlan.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoutePlan(RoutePlanEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetRoutePlanForEditAsync(companyId.Value, model.Id);
                if (reload != null) model.RouteOptions = reload.RouteOptions;

                return View("~/Views/CompanyPortal/Routes/EditRoutePlan.cshtml", model);
            }

            var success = await _companyPortalService.UpdateRoutePlanAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на route plan.";
                return View("~/Views/CompanyPortal/Routes/EditRoutePlan.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route plan беше обновен успешно.";
            return RedirectToAction(nameof(RoutePlans));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoutePlan(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteRoutePlanAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Route plan беше изтрит успешно."
                    : "Неуспешно изтриване на route plan.";

            return RedirectToAction(nameof(RoutePlans));
        }

        [HttpGet]
        public async Task<IActionResult> RoutePlanStops()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/RoutePlanStops.cshtml",
                await _companyPortalService.GetRoutePlanStopsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoutePlanStop()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/CreateRoutePlanStop.cshtml",
                await _companyPortalService.GetCreateRoutePlanStopModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoutePlanStop(RoutePlanStopCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateRoutePlanStopModelAsync(companyId.Value);
                model.RoutePlanOptions = reload.RoutePlanOptions;
                model.RouteStopOptions = reload.RouteStopOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRoutePlanStop.cshtml", model);
            }

            var id = await _companyPortalService.CreateRoutePlanStopAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на route plan stop.";

                var reload = await _companyPortalService.GetCreateRoutePlanStopModelAsync(companyId.Value);
                model.RoutePlanOptions = reload.RoutePlanOptions;
                model.RouteStopOptions = reload.RouteStopOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRoutePlanStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route plan stop беше добавен успешно.";
            return RedirectToAction(nameof(RoutePlanStops));
        }

        [HttpGet]
        public async Task<IActionResult> EditRoutePlanStop(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRoutePlanStopForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Routes/EditRoutePlanStop.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoutePlanStop(RoutePlanStopEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetRoutePlanStopForEditAsync(companyId.Value, model.Id);
                if (reload != null)
                {
                    model.RoutePlanOptions = reload.RoutePlanOptions;
                    model.RouteStopOptions = reload.RouteStopOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Routes/EditRoutePlanStop.cshtml", model);
            }

            var success = await _companyPortalService.UpdateRoutePlanStopAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на route plan stop.";
                return View("~/Views/CompanyPortal/Routes/EditRoutePlanStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route plan stop беше обновен успешно.";
            return RedirectToAction(nameof(RoutePlanStops));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoutePlanStop(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteRoutePlanStopAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Route plan stop беше изтрит успешно."
                    : "Неуспешно изтриване на route plan stop.";

            return RedirectToAction(nameof(RoutePlanStops));
        }

        [HttpGet]
        public async Task<IActionResult> RoutePlanShipments()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/RoutePlanShipments.cshtml",
                await _companyPortalService.GetRoutePlanShipmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoutePlanShipment()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Routes/CreateRoutePlanShipment.cshtml",
                await _companyPortalService.GetCreateRoutePlanShipmentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoutePlanShipment(RoutePlanShipmentCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateRoutePlanShipmentModelAsync(companyId.Value);
                model.RoutePlanOptions = reload.RoutePlanOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.RoutePlanStopOptions = reload.RoutePlanStopOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRoutePlanShipment.cshtml", model);
            }

            var id = await _companyPortalService.CreateRoutePlanShipmentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на route plan shipment.";

                var reload = await _companyPortalService.GetCreateRoutePlanShipmentModelAsync(companyId.Value);
                model.RoutePlanOptions = reload.RoutePlanOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.RoutePlanStopOptions = reload.RoutePlanStopOptions;

                return View("~/Views/CompanyPortal/Routes/CreateRoutePlanShipment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route plan shipment беше добавен успешно.";
            return RedirectToAction(nameof(RoutePlanShipments));
        }

        [HttpGet]
        public async Task<IActionResult> EditRoutePlanShipment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRoutePlanShipmentForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Routes/EditRoutePlanShipment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoutePlanShipment(RoutePlanShipmentEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetRoutePlanShipmentForEditAsync(companyId.Value, model.Id);
                if (reload != null)
                {
                    model.RoutePlanOptions = reload.RoutePlanOptions;
                    model.ShipmentOptions = reload.ShipmentOptions;
                    model.RoutePlanStopOptions = reload.RoutePlanStopOptions;
                }

                return View("~/Views/CompanyPortal/Routes/EditRoutePlanShipment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateRoutePlanShipmentAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на route plan shipment.";
                return View("~/Views/CompanyPortal/Routes/EditRoutePlanShipment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Route plan shipment беше обновен успешно.";
            return RedirectToAction(nameof(RoutePlanShipments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoutePlanShipment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteRoutePlanShipmentAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Route plan shipment беше изтрит успешно."
                    : "Неуспешно изтриване на route plan shipment.";

            return RedirectToAction(nameof(RoutePlanShipments));
        }
    }
}