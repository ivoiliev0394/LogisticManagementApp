using LogisticManagementApp.Models.CompanyPortal.Assets.Sea;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Assets()
            => View(
                "~/Views/CompanyPortal/Assets/Assets.cshtml",
                await _companyPortalService.GetAssetsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> SeaAssets()
            => View(
                "~/Views/CompanyPortal/Assets/Sea/SeaAssets.cshtml",
                await _companyPortalService.GetSeaAssetsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Vessels()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/Vessels.cshtml",
                await _companyPortalService.GetVesselsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateVessel()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CreateVessel.cshtml",
                await _companyPortalService.GetCreateVesselModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVessel(VesselCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Sea/CreateVessel.cshtml", model);

            var id = await _companyPortalService.CreateVesselAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на vessel.";
                return View("~/Views/CompanyPortal/Assets/Sea/CreateVessel.cshtml", model);
            }

            TempData["SuccessMessage"] = "Vessel беше добавен успешно.";
            return RedirectToAction(nameof(Vessels));
        }

        [HttpGet]
        public async Task<IActionResult> EditVessel(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetVesselForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Sea/EditVessel.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVessel(VesselEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Sea/EditVessel.cshtml", model);

            var success = await _companyPortalService.UpdateVesselAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на vessel.";
                return View("~/Views/CompanyPortal/Assets/Sea/EditVessel.cshtml", model);
            }

            TempData["SuccessMessage"] = "Vessel беше обновен успешно.";
            return RedirectToAction(nameof(Vessels));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVessel(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteVesselAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Vessel беше изтрит успешно." : "Неуспешно изтриване на vessel.";

            return RedirectToAction(nameof(Vessels));
        }

        [HttpGet]
        public async Task<IActionResult> VesselPositions()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/VesselPositions.cshtml",
                await _companyPortalService.GetVesselPositionsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateVesselPosition()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CreateVesselPosition.cshtml",
                await _companyPortalService.GetCreateVesselPositionModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVesselPosition(VesselPositionCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateVesselPositionModelAsync(
                    companyId.Value
                );
                model.VesselOptions = reload.VesselOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateVesselPosition.cshtml", model);
            }

            var id = await _companyPortalService.CreateVesselPositionAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на vessel position.";

                var reload = await _companyPortalService.GetCreateVesselPositionModelAsync(
                    companyId.Value
                );
                model.VesselOptions = reload.VesselOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateVesselPosition.cshtml", model);
            }

            TempData["SuccessMessage"] = "Vessel position беше добавена успешно.";
            return RedirectToAction(nameof(VesselPositions));
        }

        [HttpGet]
        public async Task<IActionResult> EditVesselPosition(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetVesselPositionForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Sea/EditVesselPosition.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVesselPosition(VesselPositionEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetVesselPositionForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.VesselOptions = reload.VesselOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/EditVesselPosition.cshtml", model);
            }

            var success = await _companyPortalService.UpdateVesselPositionAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на vessel position.";
                return View("~/Views/CompanyPortal/Assets/Sea/EditVesselPosition.cshtml", model);
            }

            TempData["SuccessMessage"] = "Vessel position беше обновена успешно.";
            return RedirectToAction(nameof(VesselPositions));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVesselPosition(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteVesselPositionAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Vessel position беше изтрита успешно."
                    : "Неуспешно изтриване на vessel position.";

            return RedirectToAction(nameof(VesselPositions));
        }

        [HttpGet]
        public async Task<IActionResult> Voyages()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/Voyages.cshtml",
                await _companyPortalService.GetVoyagesAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateVoyage()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CreateVoyage.cshtml",
                await _companyPortalService.GetCreateVoyageModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoyage(VoyageCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateVoyageModelAsync(companyId.Value);
                model.VesselOptions = reload.VesselOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateVoyage.cshtml", model);
            }

            var id = await _companyPortalService.CreateVoyageAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на voyage.";

                var reload = await _companyPortalService.GetCreateVoyageModelAsync(companyId.Value);
                model.VesselOptions = reload.VesselOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateVoyage.cshtml", model);
            }

            TempData["SuccessMessage"] = "Voyage беше добавен успешно.";
            return RedirectToAction(nameof(Voyages));
        }

        [HttpGet]
        public async Task<IActionResult> EditVoyage(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetVoyageForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Sea/EditVoyage.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoyage(VoyageEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetVoyageForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.VesselOptions = reload.VesselOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/EditVoyage.cshtml", model);
            }

            var success = await _companyPortalService.UpdateVoyageAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на voyage.";
                return View("~/Views/CompanyPortal/Assets/Sea/EditVoyage.cshtml", model);
            }

            TempData["SuccessMessage"] = "Voyage беше обновен успешно.";
            return RedirectToAction(nameof(Voyages));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoyage(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteVoyageAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Voyage беше изтрит успешно." : "Неуспешно изтриване на voyage.";

            return RedirectToAction(nameof(Voyages));
        }

        [HttpGet]
        public async Task<IActionResult> VoyageStops()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/VoyageStops.cshtml",
                await _companyPortalService.GetVoyageStopsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateVoyageStop()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CreateVoyageStop.cshtml",
                await _companyPortalService.GetCreateVoyageStopModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoyageStop(VoyageStopCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateVoyageStopModelAsync(
                    companyId.Value
                );
                model.VoyageOptions = reload.VoyageOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateVoyageStop.cshtml", model);
            }

            var id = await _companyPortalService.CreateVoyageStopAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на voyage stop.";

                var reload = await _companyPortalService.GetCreateVoyageStopModelAsync(
                    companyId.Value
                );
                model.VoyageOptions = reload.VoyageOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateVoyageStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Voyage stop беше добавен успешно.";
            return RedirectToAction(nameof(VoyageStops));
        }

        [HttpGet]
        public async Task<IActionResult> EditVoyageStop(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetVoyageStopForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Sea/EditVoyageStop.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoyageStop(VoyageStopEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetVoyageStopForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.VoyageOptions = reload.VoyageOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Sea/EditVoyageStop.cshtml", model);
            }

            var success = await _companyPortalService.UpdateVoyageStopAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на voyage stop.";
                return View("~/Views/CompanyPortal/Assets/Sea/EditVoyageStop.cshtml", model);
            }

            TempData["SuccessMessage"] = "Voyage stop беше обновен успешно.";
            return RedirectToAction(nameof(VoyageStops));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoyageStop(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteVoyageStopAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Voyage stop беше изтрит успешно." : "Неуспешно изтриване на voyage stop.";

            return RedirectToAction(nameof(VoyageStops));
        }

        [HttpGet]
        public async Task<IActionResult> VesselCrewMembers()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/VesselCrewMembers.cshtml",
                await _companyPortalService.GetVesselCrewMembersAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateVesselCrewMember()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CreateVesselCrewMember.cshtml",
                await _companyPortalService.GetCreateVesselCrewMemberModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVesselCrewMember(VesselCrewMemberCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Sea/CreateVesselCrewMember.cshtml", model);

            var id = await _companyPortalService.CreateVesselCrewMemberAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на crew member.";
                return View("~/Views/CompanyPortal/Assets/Sea/CreateVesselCrewMember.cshtml", model);
            }

            TempData["SuccessMessage"] = "Crew member беше добавен успешно.";
            return RedirectToAction(nameof(VesselCrewMembers));
        }

        [HttpGet]
        public async Task<IActionResult> EditVesselCrewMember(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetVesselCrewMemberForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Sea/EditVesselCrewMember.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVesselCrewMember(VesselCrewMemberEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Sea/EditVesselCrewMember.cshtml", model);

            var success = await _companyPortalService.UpdateVesselCrewMemberAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на crew member.";
                return View("~/Views/CompanyPortal/Assets/Sea/EditVesselCrewMember.cshtml", model);
            }

            TempData["SuccessMessage"] = "Crew member беше обновен успешно.";
            return RedirectToAction(nameof(VesselCrewMembers));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVesselCrewMember(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteVesselCrewMemberAsync(
                companyId.Value,
                id
            );
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Crew member беше изтрит успешно." : "Неуспешно изтриване на crew member.";

            return RedirectToAction(nameof(VesselCrewMembers));
        }

        [HttpGet]
        public async Task<IActionResult> CrewAssignments()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CrewAssignments.cshtml",
                await _companyPortalService.GetCrewAssignmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateCrewAssignment()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Sea/CreateCrewAssignment.cshtml",
                await _companyPortalService.GetCreateCrewAssignmentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCrewAssignment(CrewAssignmentCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateCrewAssignmentModelAsync(
                    companyId.Value
                );
                model.VoyageOptions = reload.VoyageOptions;
                model.CrewMemberOptions = reload.CrewMemberOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateCrewAssignment.cshtml", model);
            }

            var id = await _companyPortalService.CreateCrewAssignmentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на crew assignment.";

                var reload = await _companyPortalService.GetCreateCrewAssignmentModelAsync(
                    companyId.Value
                );
                model.VoyageOptions = reload.VoyageOptions;
                model.CrewMemberOptions = reload.CrewMemberOptions;

                return View("~/Views/CompanyPortal/Assets/Sea/CreateCrewAssignment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Crew assignment беше добавен успешно.";
            return RedirectToAction(nameof(CrewAssignments));
        }

        [HttpGet]
        public async Task<IActionResult> EditCrewAssignment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetCrewAssignmentForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Sea/EditCrewAssignment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCrewAssignment(CrewAssignmentEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCrewAssignmentForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.VoyageOptions = reload.VoyageOptions;
                    model.CrewMemberOptions = reload.CrewMemberOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Sea/EditCrewAssignment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateCrewAssignmentAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на crew assignment.";
                return View("~/Views/CompanyPortal/Assets/Sea/EditCrewAssignment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Crew assignment беше обновен успешно.";
            return RedirectToAction(nameof(CrewAssignments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCrewAssignment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteCrewAssignmentAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Crew assignment беше изтрит успешно."
                    : "Неуспешно изтриване на crew assignment.";

            return RedirectToAction(nameof(CrewAssignments));
        }
    }
}