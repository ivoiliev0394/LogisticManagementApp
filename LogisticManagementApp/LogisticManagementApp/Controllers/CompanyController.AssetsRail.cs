using LogisticManagementApp.Models.CompanyPortal.Assets.Rail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> RailAssets()
            => View(
                "~/Views/CompanyPortal/Assets/Rail/RailAssets.cshtml",
                await _companyPortalService.GetRailAssetsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Trains()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/Trains.cshtml",
                await _companyPortalService.GetTrainsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateTrain()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/CreateTrain.cshtml",
                await _companyPortalService.GetCreateTrainModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrain(TrainCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Rail/CreateTrain.cshtml", model);

            var id = await _companyPortalService.CreateTrainAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на train.";
                return View("~/Views/CompanyPortal/Assets/Rail/CreateTrain.cshtml", model);
            }

            TempData["SuccessMessage"] = "Train беше добавен успешно.";
            return RedirectToAction(nameof(Trains));
        }

        [HttpGet]
        public async Task<IActionResult> EditTrain(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetTrainForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Rail/EditTrain.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrain(TrainEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Rail/EditTrain.cshtml", model);

            var ok = await _companyPortalService.UpdateTrainAsync(companyId.Value, model);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на train.";
                return View("~/Views/CompanyPortal/Assets/Rail/EditTrain.cshtml", model);
            }

            TempData["SuccessMessage"] = "Train беше обновен успешно.";
            return RedirectToAction(nameof(Trains));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrain(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var ok = await _companyPortalService.DeleteTrainAsync(companyId.Value, id);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Train беше изтрит успешно." : "Неуспешно изтриване на train.";

            return RedirectToAction(nameof(Trains));
        }

        [HttpGet]
        public async Task<IActionResult> RailCars()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/RailCars.cshtml",
                await _companyPortalService.GetRailCarsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRailCar()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/CreateRailCar.cshtml",
                await _companyPortalService.GetCreateRailCarModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRailCar(RailCarCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Rail/CreateRailCar.cshtml", model);

            var id = await _companyPortalService.CreateRailCarAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на rail car.";
                return View("~/Views/CompanyPortal/Assets/Rail/CreateRailCar.cshtml", model);
            }

            TempData["SuccessMessage"] = "Rail car беше добавен успешно.";
            return RedirectToAction(nameof(RailCars));
        }

        [HttpGet]
        public async Task<IActionResult> EditRailCar(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRailCarForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Rail/EditRailCar.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRailCar(RailCarEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Rail/EditRailCar.cshtml", model);

            var ok = await _companyPortalService.UpdateRailCarAsync(companyId.Value, model);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на rail car.";
                return View("~/Views/CompanyPortal/Assets/Rail/EditRailCar.cshtml", model);
            }

            TempData["SuccessMessage"] = "Rail car беше обновен успешно.";
            return RedirectToAction(nameof(RailCars));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRailCar(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var ok = await _companyPortalService.DeleteRailCarAsync(companyId.Value, id);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Rail car беше изтрит успешно." : "Неуспешно изтриване на rail car.";

            return RedirectToAction(nameof(RailCars));
        }

        [HttpGet]
        public async Task<IActionResult> RailServices()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/RailServices.cshtml",
                await _companyPortalService.GetRailServicesAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRailService()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/CreateRailService.cshtml",
                await _companyPortalService.GetCreateRailServiceModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRailService(RailServiceCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateRailServiceModelAsync(
                    companyId.Value
                );
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Rail/CreateRailService.cshtml", model);
            }

            var id = await _companyPortalService.CreateRailServiceAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на rail service.";

                var reload = await _companyPortalService.GetCreateRailServiceModelAsync(
                    companyId.Value
                );
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Rail/CreateRailService.cshtml", model);
            }

            TempData["SuccessMessage"] = "Rail service беше добавен успешно.";
            return RedirectToAction(nameof(RailServices));
        }

        [HttpGet]
        public async Task<IActionResult> EditRailService(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRailServiceForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Rail/EditRailService.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRailService(RailServiceEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetRailServiceForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Rail/EditRailService.cshtml", model);
            }

            var ok = await _companyPortalService.UpdateRailServiceAsync(companyId.Value, model);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на rail service.";
                return View("~/Views/CompanyPortal/Assets/Rail/EditRailService.cshtml", model);
            }

            TempData["SuccessMessage"] = "Rail service беше обновен успешно.";
            return RedirectToAction(nameof(RailServices));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRailService(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var ok = await _companyPortalService.DeleteRailServiceAsync(companyId.Value, id);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Rail service беше изтрит успешно." : "Неуспешно изтриване на rail service.";

            return RedirectToAction(nameof(RailServices));
        }

        [HttpGet]
        public async Task<IActionResult> RailMovements()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/RailMovements.cshtml",
                await _companyPortalService.GetRailMovementsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateRailMovement()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Rail/CreateRailMovement.cshtml",
                await _companyPortalService.GetCreateRailMovementModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRailMovement(RailMovementCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateRailMovementModelAsync(
                    companyId.Value
                );
                model.TrainOptions = reload.TrainOptions;
                model.RailServiceOptions = reload.RailServiceOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Rail/CreateRailMovement.cshtml", model);
            }

            var id = await _companyPortalService.CreateRailMovementAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на rail movement.";

                var reload = await _companyPortalService.GetCreateRailMovementModelAsync(
                    companyId.Value
                );
                model.TrainOptions = reload.TrainOptions;
                model.RailServiceOptions = reload.RailServiceOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Rail/CreateRailMovement.cshtml", model);
            }

            TempData["SuccessMessage"] = "Rail movement беше добавен успешно.";
            return RedirectToAction(nameof(RailMovements));
        }

        [HttpGet]
        public async Task<IActionResult> EditRailMovement(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetRailMovementForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Rail/EditRailMovement.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRailMovement(RailMovementEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetRailMovementForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.TrainOptions = reload.TrainOptions;
                    model.RailServiceOptions = reload.RailServiceOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Rail/EditRailMovement.cshtml", model);
            }

            var ok = await _companyPortalService.UpdateRailMovementAsync(companyId.Value, model);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на rail movement.";
                return View("~/Views/CompanyPortal/Assets/Rail/EditRailMovement.cshtml", model);
            }

            TempData["SuccessMessage"] = "Rail movement беше обновен успешно.";
            return RedirectToAction(nameof(RailMovements));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRailMovement(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var ok = await _companyPortalService.DeleteRailMovementAsync(companyId.Value, id);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Rail movement беше изтрит успешно." : "Неуспешно изтриване на rail movement.";

            return RedirectToAction(nameof(RailMovements));
        }
    }
}