using LogisticManagementApp.Models.CompanyPortal.Assets.CargoUnits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> CargoUnitAssets()
            => View(
                "~/Views/CompanyPortal/Assets/CargoUnits/CargoUnitAssets.cshtml",
                await _companyPortalService.GetCargoUnitAssetsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Containers()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/CargoUnits/Containers.cshtml",
                await _companyPortalService.GetContainersAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateContainer()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/CargoUnits/CreateContainer.cshtml",
                await _companyPortalService.GetCreateContainerModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContainer(ContainerCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/CargoUnits/CreateContainer.cshtml", model);

            var id = await _companyPortalService.CreateContainerAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на container.";
                return View("~/Views/CompanyPortal/Assets/CargoUnits/CreateContainer.cshtml", model);
            }

            TempData["SuccessMessage"] = "Container беше добавен успешно.";
            return RedirectToAction(nameof(Containers));
        }

        [HttpGet]
        public async Task<IActionResult> EditContainer(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetContainerForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/CargoUnits/EditContainer.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContainer(ContainerEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/CargoUnits/EditContainer.cshtml", model);

            var ok = await _companyPortalService.UpdateContainerAsync(companyId.Value, model);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на container.";
                return View("~/Views/CompanyPortal/Assets/CargoUnits/EditContainer.cshtml", model);
            }

            TempData["SuccessMessage"] = "Container беше обновен успешно.";
            return RedirectToAction(nameof(Containers));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContainer(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var ok = await _companyPortalService.DeleteContainerAsync(companyId.Value, id);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok ? "Container беше изтрит успешно." : "Неуспешно изтриване на container.";

            return RedirectToAction(nameof(Containers));
        }

        [HttpGet]
        public async Task<IActionResult> ContainerSeals()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/CargoUnits/ContainerSeals.cshtml",
                await _companyPortalService.GetContainerSealsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateContainerSeal()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/CargoUnits/CreateContainerSeal.cshtml",
                await _companyPortalService.GetCreateContainerSealModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContainerSeal(ContainerSealCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateContainerSealModelAsync(
                    companyId.Value
                );
                model.ContainerOptions = reload.ContainerOptions;

                return View(
                    "~/Views/CompanyPortal/Assets/CargoUnits/CreateContainerSeal.cshtml",
                    model
                );
            }

            var id = await _companyPortalService.CreateContainerSealAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на container seal.";

                var reload = await _companyPortalService.GetCreateContainerSealModelAsync(
                    companyId.Value
                );
                model.ContainerOptions = reload.ContainerOptions;

                return View(
                    "~/Views/CompanyPortal/Assets/CargoUnits/CreateContainerSeal.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Container seal беше добавен успешно.";
            return RedirectToAction(nameof(ContainerSeals));
        }

        [HttpGet]
        public async Task<IActionResult> EditContainerSeal(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetContainerSealForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/CargoUnits/EditContainerSeal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContainerSeal(ContainerSealEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetContainerSealForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.ContainerOptions = reload.ContainerOptions;

                return View("~/Views/CompanyPortal/Assets/CargoUnits/EditContainerSeal.cshtml", model);
            }

            var ok = await _companyPortalService.UpdateContainerSealAsync(companyId.Value, model);

            if (!ok)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на container seal.";
                return View("~/Views/CompanyPortal/Assets/CargoUnits/EditContainerSeal.cshtml", model);
            }

            TempData["SuccessMessage"] = "Container seal беше обновен успешно.";
            return RedirectToAction(nameof(ContainerSeals));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContainerSeal(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var ok = await _companyPortalService.DeleteContainerSealAsync(companyId.Value, id);
            TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
                ok
                    ? "Container seal беше изтрит успешно."
                    : "Неуспешно изтриване на container seal.";

            return RedirectToAction(nameof(ContainerSeals));
        }
    }
}