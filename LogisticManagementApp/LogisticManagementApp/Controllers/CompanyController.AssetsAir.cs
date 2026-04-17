using LogisticManagementApp.Models.CompanyPortal.Assets.Air;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> AirAssets()
            => View(
                "~/Views/CompanyPortal/Assets/Air/AirAssets.cshtml",
                await _companyPortalService.GetAirAssetsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Aircraft()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/Aircraft.cshtml",
                await _companyPortalService.GetAircraftAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateAircraft()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/CreateAircraft.cshtml",
                await _companyPortalService.GetCreateAircraftModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAircraft(AircraftCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Air/CreateAircraft.cshtml", model);

            var id = await _companyPortalService.CreateAircraftAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на aircraft.";
                return View("~/Views/CompanyPortal/Assets/Air/CreateAircraft.cshtml", model);
            }

            TempData["SuccessMessage"] = "Aircraft беше добавен успешно.";
            return RedirectToAction(nameof(Aircraft));
        }

        [HttpGet]
        public async Task<IActionResult> EditAircraft(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetAircraftForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Air/EditAircraft.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAircraft(AircraftEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Air/EditAircraft.cshtml", model);

            var success = await _companyPortalService.UpdateAircraftAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на aircraft.";
                return View("~/Views/CompanyPortal/Assets/Air/EditAircraft.cshtml", model);
            }

            TempData["SuccessMessage"] = "Aircraft беше обновен успешно.";
            return RedirectToAction(nameof(Aircraft));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAircraft(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteAircraftAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Aircraft беше изтрит успешно." : "Неуспешно изтриване на aircraft.";

            return RedirectToAction(nameof(Aircraft));
        }

        [HttpGet]
        public async Task<IActionResult> Flights()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/Flights.cshtml",
                await _companyPortalService.GetFlightsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateFlight()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/CreateFlight.cshtml",
                await _companyPortalService.GetCreateFlightModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFlight(FlightCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateFlightModelAsync(companyId.Value);
                model.AircraftOptions = reload.AircraftOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Air/CreateFlight.cshtml", model);
            }

            var id = await _companyPortalService.CreateFlightAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на flight.";

                var reload = await _companyPortalService.GetCreateFlightModelAsync(companyId.Value);
                model.AircraftOptions = reload.AircraftOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Air/CreateFlight.cshtml", model);
            }

            TempData["SuccessMessage"] = "Flight беше добавен успешно.";
            return RedirectToAction(nameof(Flights));
        }

        [HttpGet]
        public async Task<IActionResult> EditFlight(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetFlightForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Air/EditFlight.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFlight(FlightEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetFlightForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.AircraftOptions = reload.AircraftOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Air/EditFlight.cshtml", model);
            }

            var success = await _companyPortalService.UpdateFlightAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на flight.";
                return View("~/Views/CompanyPortal/Assets/Air/EditFlight.cshtml", model);
            }

            TempData["SuccessMessage"] = "Flight беше обновен успешно.";
            return RedirectToAction(nameof(Flights));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFlight(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteFlightAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Flight беше изтрит успешно." : "Неуспешно изтриване на flight.";

            return RedirectToAction(nameof(Flights));
        }

        [HttpGet]
        public async Task<IActionResult> FlightSegments()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/FlightSegments.cshtml",
                await _companyPortalService.GetFlightSegmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateFlightSegment()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/CreateFlightSegment.cshtml",
                await _companyPortalService.GetCreateFlightSegmentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFlightSegment(FlightSegmentCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateFlightSegmentModelAsync(
                    companyId.Value
                );
                model.FlightOptions = reload.FlightOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Air/CreateFlightSegment.cshtml", model);
            }

            var id = await _companyPortalService.CreateFlightSegmentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на flight segment.";

                var reload = await _companyPortalService.GetCreateFlightSegmentModelAsync(
                    companyId.Value
                );
                model.FlightOptions = reload.FlightOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Assets/Air/CreateFlightSegment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Flight segment беше добавен успешно.";
            return RedirectToAction(nameof(FlightSegments));
        }

        [HttpGet]
        public async Task<IActionResult> EditFlightSegment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetFlightSegmentForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Air/EditFlightSegment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFlightSegment(FlightSegmentEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetFlightSegmentForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.FlightOptions = reload.FlightOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Air/EditFlightSegment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateFlightSegmentAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на flight segment.";
                return View("~/Views/CompanyPortal/Assets/Air/EditFlightSegment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Flight segment беше обновен успешно.";
            return RedirectToAction(nameof(FlightSegments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFlightSegment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteFlightSegmentAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Flight segment беше изтрит успешно."
                    : "Неуспешно изтриване на flight segment.";

            return RedirectToAction(nameof(FlightSegments));
        }

        [HttpGet]
        public async Task<IActionResult> AirCrewMembers()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/AirCrewMembers.cshtml",
                await _companyPortalService.GetAirCrewMembersAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateAirCrewMember()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/CreateAirCrewMember.cshtml",
                await _companyPortalService.GetCreateAirCrewMemberModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAirCrewMember(AirCrewMemberCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Air/CreateAirCrewMember.cshtml", model);

            var id = await _companyPortalService.CreateAirCrewMemberAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на air crew member.";
                return View("~/Views/CompanyPortal/Assets/Air/CreateAirCrewMember.cshtml", model);
            }

            TempData["SuccessMessage"] = "Air crew member беше добавен успешно.";
            return RedirectToAction(nameof(AirCrewMembers));
        }

        [HttpGet]
        public async Task<IActionResult> EditAirCrewMember(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetAirCrewMemberForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Air/EditAirCrewMember.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAirCrewMember(AirCrewMemberEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Air/EditAirCrewMember.cshtml", model);

            var success = await _companyPortalService.UpdateAirCrewMemberAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на air crew member.";
                return View("~/Views/CompanyPortal/Assets/Air/EditAirCrewMember.cshtml", model);
            }

            TempData["SuccessMessage"] = "Air crew member беше обновен успешно.";
            return RedirectToAction(nameof(AirCrewMembers));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAirCrewMember(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteAirCrewMemberAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Air crew member беше изтрит успешно."
                    : "Неуспешно изтриване на air crew member.";

            return RedirectToAction(nameof(AirCrewMembers));
        }

        [HttpGet]
        public async Task<IActionResult> AirCrewAssignments()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/AirCrewAssignments.cshtml",
                await _companyPortalService.GetAirCrewAssignmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateAirCrewAssignment()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/CreateAirCrewAssignment.cshtml",
                await _companyPortalService.GetCreateAirCrewAssignmentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAirCrewAssignment(
            AirCrewAssignmentCreateViewModel model
        )
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateAirCrewAssignmentModelAsync(
                    companyId.Value
                );
                model.FlightOptions = reload.FlightOptions;
                model.AirCrewMemberOptions = reload.AirCrewMemberOptions;

                return View("~/Views/CompanyPortal/Assets/Air/CreateAirCrewAssignment.cshtml", model);
            }

            var id = await _companyPortalService.CreateAirCrewAssignmentAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на air crew assignment.";

                var reload = await _companyPortalService.GetCreateAirCrewAssignmentModelAsync(
                    companyId.Value
                );
                model.FlightOptions = reload.FlightOptions;
                model.AirCrewMemberOptions = reload.AirCrewMemberOptions;

                return View("~/Views/CompanyPortal/Assets/Air/CreateAirCrewAssignment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Air crew assignment беше добавен успешно.";
            return RedirectToAction(nameof(AirCrewAssignments));
        }

        [HttpGet]
        public async Task<IActionResult> EditAirCrewAssignment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetAirCrewAssignmentForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Air/EditAirCrewAssignment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAirCrewAssignment(AirCrewAssignmentEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetAirCrewAssignmentForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.FlightOptions = reload.FlightOptions;
                    model.AirCrewMemberOptions = reload.AirCrewMemberOptions;
                }

                return View("~/Views/CompanyPortal/Assets/Air/EditAirCrewAssignment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateAirCrewAssignmentAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на air crew assignment.";
                return View("~/Views/CompanyPortal/Assets/Air/EditAirCrewAssignment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Air crew assignment беше обновен успешно.";
            return RedirectToAction(nameof(AirCrewAssignments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAirCrewAssignment(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteAirCrewAssignmentAsync(
                companyId.Value,
                id
            );
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Air crew assignment беше изтрит успешно."
                    : "Неуспешно изтриване на air crew assignment.";

            return RedirectToAction(nameof(AirCrewAssignments));
        }

        [HttpGet]
        public async Task<IActionResult> ULDs()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/ULDs.cshtml",
                await _companyPortalService.GetUldsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateULD()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Assets/Air/CreateULD.cshtml",
                await _companyPortalService.GetCreateUldModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateULD(UldCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Air/CreateULD.cshtml", model);

            var id = await _companyPortalService.CreateUldAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на ULD.";
                return View("~/Views/CompanyPortal/Assets/Air/CreateULD.cshtml", model);
            }

            TempData["SuccessMessage"] = "ULD беше добавен успешно.";
            return RedirectToAction(nameof(ULDs));
        }

        [HttpGet]
        public async Task<IActionResult> EditULD(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetUldForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Assets/Air/EditULD.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditULD(UldEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Assets/Air/EditULD.cshtml", model);

            var success = await _companyPortalService.UpdateUldAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на ULD.";
                return View("~/Views/CompanyPortal/Assets/Air/EditULD.cshtml", model);
            }

            TempData["SuccessMessage"] = "ULD беше обновен успешно.";
            return RedirectToAction(nameof(ULDs));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteULD(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteUldAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "ULD беше изтрит успешно." : "Неуспешно изтриване на ULD.";

            return RedirectToAction(nameof(ULDs));
        }
    }
}