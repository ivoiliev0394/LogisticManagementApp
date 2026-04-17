using LogisticManagementApp.Models.CompanyPortal.Assets.Road;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> RoadAssets()
            => View(
                "~/Views/CompanyPortal/Assets/Road/RoadAssets.cshtml",
                await _companyPortalService.GetRoadAssetsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Vehicles()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/Vehicles.cshtml",
                await _companyPortalService.GetVehiclesAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateVehicle()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/CreateVehicle.cshtml",
                await _companyPortalService.GetCreateVehicleModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVehicle(VehicleCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Road/CreateVehicle.cshtml", model);

            var id = await _companyPortalService.CreateVehicleAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на vehicle.";
                return View("~/Views/CompanyPortal/Assets/Road/CreateVehicle.cshtml", model);
            }

            TempData["SuccessMessage"] = "Vehicle беше добавен успешно.";
            return RedirectToAction(nameof(Vehicles));
        }

        [HttpGet]
        public async Task<IActionResult> EditVehicle(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetVehicleForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Road/EditVehicle.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(VehicleEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Road/EditVehicle.cshtml", model);

            var success = await _companyPortalService.UpdateVehicleAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на vehicle.";
                return View("~/Views/CompanyPortal/Assets/Road/EditVehicle.cshtml", model);
            }

            TempData["SuccessMessage"] = "Vehicle беше обновен успешно.";
            return RedirectToAction(nameof(Vehicles));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteVehicleAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Vehicle беше изтрит успешно." : "Неуспешно изтриване на vehicle.";

            return RedirectToAction(nameof(Vehicles));
        }

        [HttpGet]
        public async Task<IActionResult> Drivers()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/Drivers.cshtml",
                await _companyPortalService.GetDriversAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateDriver()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/CreateDriver.cshtml",
                await _companyPortalService.GetCreateDriverModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDriver(DriverCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Road/CreateDriver.cshtml", model);

            var id = await _companyPortalService.CreateDriverAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на driver.";
                return View("~/Views/CompanyPortal/Assets/Road/CreateDriver.cshtml", model);
            }

            TempData["SuccessMessage"] = "Driver беше добавен успешно.";
            return RedirectToAction(nameof(Drivers));
        }

        [HttpGet]
        public async Task<IActionResult> EditDriver(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDriverForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Road/EditDriver.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDriver(DriverEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Road/EditDriver.cshtml", model);

            var success = await _companyPortalService.UpdateDriverAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на driver.";
                return View("~/Views/CompanyPortal/Assets/Road/EditDriver.cshtml", model);
            }

            TempData["SuccessMessage"] = "Driver беше обновен успешно.";
            return RedirectToAction(nameof(Drivers));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDriver(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteDriverAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Driver беше изтрит успешно." : "Неуспешно изтриване на driver.";

            return RedirectToAction(nameof(Drivers));
        }

        [HttpGet]
        public async Task<IActionResult> Trips()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/Trips.cshtml",
                await _companyPortalService.GetTripsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateTrip()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/CreateTrip.cshtml",
                await _companyPortalService.GetCreateTripModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrip(TripCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateTripModelAsync(companyId.Value);
                model.VehicleOptions = reload.VehicleOptions;
                model.DriverOptions = reload.DriverOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Road/CreateTrip.cshtml", model);
            }

            var id = await _companyPortalService.CreateTripAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на trip.";

                var reload = await _companyPortalService.GetCreateTripModelAsync(companyId.Value);
                model.VehicleOptions = reload.VehicleOptions;
                model.DriverOptions = reload.DriverOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Road/CreateTrip.cshtml", model);
            }

            TempData["SuccessMessage"] = "Trip беше добавен успешно.";
            return RedirectToAction(nameof(Trips));
        }

        [HttpGet]
        public async Task<IActionResult> EditTrip(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetTripForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Road/EditTrip.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrip(TripEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetTripForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.VehicleOptions = reload.VehicleOptions;
                    model.DriverOptions = reload.DriverOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Road/EditTrip.cshtml", model);
            }

            var success = await _companyPortalService.UpdateTripAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на trip.";
                return View("~/Views/CompanyPortal/Assets/Road/EditTrip.cshtml", model);
            }

            TempData["SuccessMessage"] = "Trip беше обновен успешно.";
            return RedirectToAction(nameof(Trips));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteTripAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Trip беше изтрит успешно." : "Неуспешно изтриване на trip.";

            return RedirectToAction(nameof(Trips));
        }

        [HttpGet]
        public async Task<IActionResult> TripStops()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/TripStops.cshtml",
                await _companyPortalService.GetTripStopsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateTripStop()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/CreateTripStop.cshtml",
                await _companyPortalService.GetCreateTripStopModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTripStop(TripStopCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateTripStopModelAsync(
                    companyId.Value
                );
                model.TripOptions = reload.TripOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Road/CreateTripStop.cshtml", model);
            }

            var id = await _companyPortalService.CreateTripStopAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на trip stop.";

                var reload = await _companyPortalService.GetCreateTripStopModelAsync(
                    companyId.Value
                );
                model.TripOptions = reload.TripOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Road/CreateTripStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Trip stop беше добавен успешно.";
            return RedirectToAction(nameof(TripStops));
        }

        [HttpGet]
        public async Task<IActionResult> EditTripStop(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetTripStopForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Road/EditTripStop.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTripStop(TripStopEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetTripStopForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.TripOptions = reload.TripOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Road/EditTripStop.cshtml", model);
            }

            var success = await _companyPortalService.UpdateTripStopAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на trip stop.";
                return View("~/Views/CompanyPortal/Assets/Road/EditTripStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Trip stop беше обновен успешно.";
            return RedirectToAction(nameof(TripStops));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTripStop(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteTripStopAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Trip stop беше изтрит успешно." : "Неуспешно изтриване на trip stop.";

            return RedirectToAction(nameof(TripStops));
        }

        [HttpGet]
        public async Task<IActionResult> TripShipments()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/TripShipments.cshtml",
                await _companyPortalService.GetTripShipmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateTripShipment()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Road/CreateTripShipment.cshtml",
                await _companyPortalService.GetCreateTripShipmentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTripShipment(TripShipmentCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateTripShipmentModelAsync(
                    companyId.Value
                );
                model.TripOptions = reload.TripOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;
                model.TripStopOptions = reload.TripStopOptions;

                return View("~/Views/CompanyPortal/Assets/Road/CreateTripShipment.cshtml", model);
            }

            var id = await _companyPortalService.CreateTripShipmentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на trip shipment.";

                var reload = await _companyPortalService.GetCreateTripShipmentModelAsync(
                    companyId.Value
                );
                model.TripOptions = reload.TripOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;
                model.TripStopOptions = reload.TripStopOptions;

                return View("~/Views/CompanyPortal/Assets/Road/CreateTripShipment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Trip shipment беше добавен успешно.";
            return RedirectToAction(nameof(TripShipments));
        }

        [HttpGet]
        public async Task<IActionResult> EditTripShipment(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetTripShipmentForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Road/EditTripShipment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTripShipment(TripShipmentEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetTripShipmentForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.TripOptions = reload.TripOptions;
                    model.ShipmentOptions = reload.ShipmentOptions;
                    model.ShipmentLegOptions = reload.ShipmentLegOptions;
                    model.TripStopOptions = reload.TripStopOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Road/EditTripShipment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateTripShipmentAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на trip shipment.";
                return View("~/Views/CompanyPortal/Assets/Road/EditTripShipment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Trip shipment беше обновен успешно.";
            return RedirectToAction(nameof(TripShipments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTripShipment(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteTripShipmentAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Trip shipment беше изтрит успешно."
                    : "Неуспешно изтриване на trip shipment.";

            return RedirectToAction(nameof(TripShipments));
        }
    }
}