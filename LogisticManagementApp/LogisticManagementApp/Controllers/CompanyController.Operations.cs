using LogisticManagementApp.Models.CompanyPortal.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Operations()
            => View(
                "~/Views/CompanyPortal/Operations/Operations.cshtml",
                await _companyPortalService.GetOperationsHomeAsync()
            );

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/Notifications.cshtml",
                await _companyPortalService.GetNotificationsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateNotification()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateNotification.cshtml",
                await _companyPortalService.GetCreateNotificationModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNotification(NotificationCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                model.UserOptions = (
                    await _companyPortalService.GetCreateNotificationModelAsync(companyId.Value)
                ).UserOptions;

                return View("~/Views/CompanyPortal/Operations/CreateNotification.cshtml", model);
            }

            var id = await _companyPortalService.CreateNotificationAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на notification.";
                model.UserOptions = (
                    await _companyPortalService.GetCreateNotificationModelAsync(companyId.Value)
                ).UserOptions;

                return View("~/Views/CompanyPortal/Operations/CreateNotification.cshtml", model);
            }

            TempData["SuccessMessage"] = "Notification беше добавен успешно.";
            return RedirectToAction(nameof(Notifications));
        }

        [HttpGet]
        public async Task<IActionResult> EditNotification(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetNotificationForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditNotification.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNotification(NotificationEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetNotificationForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.UserOptions = reload.UserOptions;

                return View("~/Views/CompanyPortal/Operations/EditNotification.cshtml", model);
            }

            var success = await _companyPortalService.UpdateNotificationAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на notification.";
                return View("~/Views/CompanyPortal/Operations/EditNotification.cshtml", model);
            }

            TempData["SuccessMessage"] = "Notification беше обновен успешно.";
            return RedirectToAction(nameof(Notifications));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteNotificationAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Notification беше изтрит успешно."
                    : "Неуспешно изтриване на notification.";

            return RedirectToAction(nameof(Notifications));
        }

        [HttpGet]
        public async Task<IActionResult> NotificationSubscriptions()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/NotificationSubscriptions.cshtml",
                await _companyPortalService.GetNotificationSubscriptionsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateNotificationSubscription()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateNotificationSubscription.cshtml",
                await _companyPortalService.GetCreateNotificationSubscriptionModelAsync(
                    companyId.Value
                )
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNotificationSubscription(
            NotificationSubscriptionCreateViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                model.UserOptions = (
                    await _companyPortalService.GetCreateNotificationSubscriptionModelAsync(
                        companyId.Value
                    )
                ).UserOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateNotificationSubscription.cshtml",
                    model
                );
            }

            var id = await _companyPortalService.CreateNotificationSubscriptionAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на notification subscription.";
                model.UserOptions = (
                    await _companyPortalService.GetCreateNotificationSubscriptionModelAsync(
                        companyId.Value
                    )
                ).UserOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateNotificationSubscription.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Notification subscription беше добавен успешно.";
            return RedirectToAction(nameof(NotificationSubscriptions));
        }

        [HttpGet]
        public async Task<IActionResult> EditNotificationSubscription(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetNotificationSubscriptionForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View(
                "~/Views/CompanyPortal/Operations/EditNotificationSubscription.cshtml",
                model
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNotificationSubscription(
            NotificationSubscriptionEditViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetNotificationSubscriptionForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.UserOptions = reload.UserOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/EditNotificationSubscription.cshtml",
                    model
                );
            }

            var success = await _companyPortalService.UpdateNotificationSubscriptionAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на notification subscription.";
                return View(
                    "~/Views/CompanyPortal/Operations/EditNotificationSubscription.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Notification subscription беше обновен успешно.";
            return RedirectToAction(nameof(NotificationSubscriptions));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotificationSubscription(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteNotificationSubscriptionAsync(
                companyId.Value,
                id
            );

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Notification subscription беше изтрит успешно."
                    : "Неуспешно изтриване на notification subscription.";

            return RedirectToAction(nameof(NotificationSubscriptions));
        }

        [HttpGet]
        public async Task<IActionResult> AuditLogs()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/AuditLogs.cshtml",
                await _companyPortalService.GetAuditLogsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> SavedFilters()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/SavedFilters.cshtml",
                await _companyPortalService.GetSavedFiltersAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateSavedFilter()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateSavedFilter.cshtml",
                await _companyPortalService.GetCreateSavedFilterModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSavedFilter(SavedFilterCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                model.UserOptions = (
                    await _companyPortalService.GetCreateSavedFilterModelAsync(companyId.Value)
                ).UserOptions;

                return View("~/Views/CompanyPortal/Operations/CreateSavedFilter.cshtml", model);
            }

            var id = await _companyPortalService.CreateSavedFilterAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на saved filter.";
                model.UserOptions = (
                    await _companyPortalService.GetCreateSavedFilterModelAsync(companyId.Value)
                ).UserOptions;

                return View("~/Views/CompanyPortal/Operations/CreateSavedFilter.cshtml", model);
            }

            TempData["SuccessMessage"] = "Saved filter беше добавен успешно.";
            return RedirectToAction(nameof(SavedFilters));
        }

        [HttpGet]
        public async Task<IActionResult> EditSavedFilter(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetSavedFilterForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditSavedFilter.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSavedFilter(SavedFilterEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetSavedFilterForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.UserOptions = reload.UserOptions;

                return View("~/Views/CompanyPortal/Operations/EditSavedFilter.cshtml", model);
            }

            var success = await _companyPortalService.UpdateSavedFilterAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на saved filter.";
                return View("~/Views/CompanyPortal/Operations/EditSavedFilter.cshtml", model);
            }

            TempData["SuccessMessage"] = "Saved filter беше обновен успешно.";
            return RedirectToAction(nameof(SavedFilters));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSavedFilter(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteSavedFilterAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Saved filter беше изтрит успешно." : "Неуспешно изтриване на saved filter.";

            return RedirectToAction(nameof(SavedFilters));
        }

        [HttpGet]
        public async Task<IActionResult> CompanyDashboardConfigs()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CompanyDashboardConfigs.cshtml",
                await _companyPortalService.GetCompanyDashboardConfigsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateCompanyDashboardConfig()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateCompanyDashboardConfig.cshtml",
                await _companyPortalService.GetCreateCompanyDashboardConfigModelAsync(
                    companyId.Value
                )
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompanyDashboardConfig(
            CompanyDashboardConfigCreateViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View(
                    "~/Views/CompanyPortal/Operations/CreateCompanyDashboardConfig.cshtml",
                    model
                );

            var id = await _companyPortalService.CreateCompanyDashboardConfigAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на dashboard config.";
                return View(
                    "~/Views/CompanyPortal/Operations/CreateCompanyDashboardConfig.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Dashboard config беше добавен успешно.";
            return RedirectToAction(nameof(CompanyDashboardConfigs));
        }

        [HttpGet]
        public async Task<IActionResult> EditCompanyDashboardConfig(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetCompanyDashboardConfigForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditCompanyDashboardConfig.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompanyDashboardConfig(
            CompanyDashboardConfigEditViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
                return View("~/Views/CompanyPortal/Operations/EditCompanyDashboardConfig.cshtml", model);

            var success = await _companyPortalService.UpdateCompanyDashboardConfigAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на dashboard config.";
                return View(
                    "~/Views/CompanyPortal/Operations/EditCompanyDashboardConfig.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Dashboard config беше обновен успешно.";
            return RedirectToAction(nameof(CompanyDashboardConfigs));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyDashboardConfig(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteCompanyDashboardConfigAsync(
                companyId.Value,
                id
            );

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Dashboard config беше изтрит успешно."
                    : "Неуспешно изтриване на dashboard config.";

            return RedirectToAction(nameof(CompanyDashboardConfigs));
        }

        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/Bookings.cshtml",
                await _companyPortalService.GetBookingsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateBooking()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateBooking.cshtml",
                await _companyPortalService.GetCreateBookingModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(BookingCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateBookingModelAsync(companyId.Value);
                model.CarrierCompanyOptions = reload.CarrierCompanyOptions;
                model.ShipmentOptions = reload.ShipmentOptions;

                return View("~/Views/CompanyPortal/Operations/CreateBooking.cshtml", model);
            }

            var id = await _companyPortalService.CreateBookingAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на booking.";

                var reload = await _companyPortalService.GetCreateBookingModelAsync(companyId.Value);
                model.CarrierCompanyOptions = reload.CarrierCompanyOptions;
                model.ShipmentOptions = reload.ShipmentOptions;

                return View("~/Views/CompanyPortal/Operations/CreateBooking.cshtml", model);
            }

            TempData["SuccessMessage"] = "Booking беше добавен успешно.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpGet]
        public async Task<IActionResult> EditBooking(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetBookingForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditBooking.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBooking(BookingEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetBookingForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.CarrierCompanyOptions = reload.CarrierCompanyOptions;
                    model.ShipmentOptions = reload.ShipmentOptions;
                }

                return View("~/Views/CompanyPortal/Operations/EditBooking.cshtml", model);
            }

            var success = await _companyPortalService.UpdateBookingAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на booking.";
                return View("~/Views/CompanyPortal/Operations/EditBooking.cshtml", model);
            }

            TempData["SuccessMessage"] = "Booking беше обновен успешно.";
            return RedirectToAction(nameof(Bookings));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteBookingAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Booking беше изтрит успешно." : "Неуспешно изтриване на booking.";

            return RedirectToAction(nameof(Bookings));
        }

        [HttpGet]
        public async Task<IActionResult> BookingLegs()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/BookingLegs.cshtml",
                await _companyPortalService.GetBookingLegsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateBookingLeg()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateBookingLeg.cshtml",
                await _companyPortalService.GetCreateBookingLegModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBookingLeg(BookingLegCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateBookingLegModelAsync(
                    companyId.Value
                );
                model.BookingOptions = reload.BookingOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Operations/CreateBookingLeg.cshtml", model);
            }

            var id = await _companyPortalService.CreateBookingLegAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на booking leg.";

                var reload = await _companyPortalService.GetCreateBookingLegModelAsync(
                    companyId.Value
                );
                model.BookingOptions = reload.BookingOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;
                model.LocationOptions = reload.LocationOptions;

                return View("~/Views/CompanyPortal/Operations/CreateBookingLeg.cshtml", model);
            }

            TempData["SuccessMessage"] = "Booking leg беше добавен успешно.";
            return RedirectToAction(nameof(BookingLegs));
        }

        [HttpGet]
        public async Task<IActionResult> EditBookingLeg(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetBookingLegForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditBookingLeg.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBookingLeg(BookingLegEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetBookingLegForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.BookingOptions = reload.BookingOptions;
                    model.ShipmentLegOptions = reload.ShipmentLegOptions;
                    model.LocationOptions = reload.LocationOptions;
                }

                return View("~/Views/CompanyPortal/Operations/EditBookingLeg.cshtml", model);
            }

            var success = await _companyPortalService.UpdateBookingLegAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на booking leg.";
                return View("~/Views/CompanyPortal/Operations/EditBookingLeg.cshtml", model);
            }

            TempData["SuccessMessage"] = "Booking leg беше обновен успешно.";
            return RedirectToAction(nameof(BookingLegs));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBookingLeg(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteBookingLegAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Booking leg беше изтрит успешно." : "Неуспешно изтриване на booking leg.";

            return RedirectToAction(nameof(BookingLegs));
        }

        [HttpGet]
        public async Task<IActionResult> Consolidations()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/Consolidations.cshtml",
                await _companyPortalService.GetConsolidationsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> ConsolidationShipments()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/ConsolidationShipments.cshtml",
                await _companyPortalService.GetConsolidationShipmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateConsolidationShipment()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateConsolidationShipment.cshtml",
                await _companyPortalService.GetCreateConsolidationShipmentModelAsync(
                    companyId.Value
                )
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConsolidationShipment(
            ConsolidationShipmentCreateViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateConsolidationShipmentModelAsync(
                    companyId.Value
                );
                model.ConsolidationOptions = reload.ConsolidationOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateConsolidationShipment.cshtml",
                    model
                );
            }

            var id = await _companyPortalService.CreateConsolidationShipmentAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на consolidation shipment.";

                var reload = await _companyPortalService.GetCreateConsolidationShipmentModelAsync(
                    companyId.Value
                );
                model.ConsolidationOptions = reload.ConsolidationOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateConsolidationShipment.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Consolidation shipment беше добавен успешно.";
            return RedirectToAction(nameof(ConsolidationShipments));
        }

        [HttpGet]
        public async Task<IActionResult> EditConsolidationShipment(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetConsolidationShipmentForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditConsolidationShipment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsolidationShipment(
            ConsolidationShipmentEditViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetConsolidationShipmentForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.ConsolidationOptions = reload.ConsolidationOptions;
                    model.ShipmentOptions = reload.ShipmentOptions;
                    model.ShipmentLegOptions = reload.ShipmentLegOptions;
                }

                return View("~/Views/CompanyPortal/Operations/EditConsolidationShipment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateConsolidationShipmentAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на consolidation shipment.";
                return View(
                    "~/Views/CompanyPortal/Operations/EditConsolidationShipment.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Consolidation shipment беше обновен успешно.";
            return RedirectToAction(nameof(ConsolidationShipments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConsolidationShipment(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteConsolidationShipmentAsync(
                companyId.Value,
                id
            );

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Consolidation shipment беше изтрит успешно."
                    : "Неуспешно изтриване на consolidation shipment.";

            return RedirectToAction(nameof(ConsolidationShipments));
        }

        [HttpGet]
        public async Task<IActionResult> ResourceCalendars()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/ResourceCalendars.cshtml",
                await _companyPortalService.GetResourceCalendarsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateResourceCalendar()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateResourceCalendar.cshtml",
                await _companyPortalService.GetCreateResourceCalendarModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResourceCalendar(ResourceCalendarCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                model.ResourceOptions = (
                    await _companyPortalService.GetCreateResourceCalendarModelAsync(companyId.Value)
                ).ResourceOptions;

                return View("~/Views/CompanyPortal/Operations/CreateResourceCalendar.cshtml", model);
            }

            var id = await _companyPortalService.CreateResourceCalendarAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на resource calendar.";
                model.ResourceOptions = (
                    await _companyPortalService.GetCreateResourceCalendarModelAsync(companyId.Value)
                ).ResourceOptions;

                return View("~/Views/CompanyPortal/Operations/CreateResourceCalendar.cshtml", model);
            }

            TempData["SuccessMessage"] = "Resource calendar беше добавен успешно.";
            return RedirectToAction(nameof(ResourceCalendars));
        }

        [HttpGet]
        public async Task<IActionResult> EditResourceCalendar(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetResourceCalendarForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditResourceCalendar.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResourceCalendar(ResourceCalendarEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetResourceCalendarForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.ResourceOptions = reload.ResourceOptions;

                return View("~/Views/CompanyPortal/Operations/EditResourceCalendar.cshtml", model);
            }

            var success = await _companyPortalService.UpdateResourceCalendarAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на resource calendar.";
                return View("~/Views/CompanyPortal/Operations/EditResourceCalendar.cshtml", model);
            }

            TempData["SuccessMessage"] = "Resource calendar беше обновен успешно.";
            return RedirectToAction(nameof(ResourceCalendars));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResourceCalendar(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteResourceCalendarAsync(
                companyId.Value,
                id
            );

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Resource calendar беше изтрит успешно."
                    : "Неуспешно изтриване на resource calendar.";

            return RedirectToAction(nameof(ResourceCalendars));
        }

        [HttpGet]
        public async Task<IActionResult> ResourceAvailabilities()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/ResourceAvailabilities.cshtml",
                await _companyPortalService.GetResourceAvailabilitiesAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateResourceAvailability()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateResourceAvailability.cshtml",
                await _companyPortalService.GetCreateResourceAvailabilityModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResourceAvailability(
            ResourceAvailabilityCreateViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                model.ResourceOptions = (
                    await _companyPortalService.GetCreateResourceAvailabilityModelAsync(
                        companyId.Value
                    )
                ).ResourceOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateResourceAvailability.cshtml",
                    model
                );
            }

            var id = await _companyPortalService.CreateResourceAvailabilityAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на resource availability.";
                model.ResourceOptions = (
                    await _companyPortalService.GetCreateResourceAvailabilityModelAsync(
                        companyId.Value
                    )
                ).ResourceOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateResourceAvailability.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Resource availability беше добавен успешно.";
            return RedirectToAction(nameof(ResourceAvailabilities));
        }

        [HttpGet]
        public async Task<IActionResult> EditResourceAvailability(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetResourceAvailabilityForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditResourceAvailability.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResourceAvailability(ResourceAvailabilityEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetResourceAvailabilityForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null) model.ResourceOptions = reload.ResourceOptions;

                return View("~/Views/CompanyPortal/Operations/EditResourceAvailability.cshtml", model);
            }

            var success = await _companyPortalService.UpdateResourceAvailabilityAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на resource availability.";
                return View(
                    "~/Views/CompanyPortal/Operations/EditResourceAvailability.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Resource availability беше обновен успешно.";
            return RedirectToAction(nameof(ResourceAvailabilities));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResourceAvailability(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteResourceAvailabilityAsync(
                companyId.Value,
                id
            );

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Resource availability беше изтрит успешно."
                    : "Неуспешно изтриване на resource availability.";

            return RedirectToAction(nameof(ResourceAvailabilities));
        }

        [HttpGet]
        public async Task<IActionResult> CapacityReservations()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CapacityReservations.cshtml",
                await _companyPortalService.GetCapacityReservationsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateCapacityReservation()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateCapacityReservation.cshtml",
                await _companyPortalService.GetCreateCapacityReservationModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCapacityReservation(
            CapacityReservationCreateViewModel model
        )
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateCapacityReservationModelAsync(
                    companyId.Value
                );
                model.ResourceOptions = reload.ResourceOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateCapacityReservation.cshtml",
                    model
                );
            }

            var id = await _companyPortalService.CreateCapacityReservationAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на capacity reservation.";

                var reload = await _companyPortalService.GetCreateCapacityReservationModelAsync(
                    companyId.Value
                );
                model.ResourceOptions = reload.ResourceOptions;
                model.ShipmentOptions = reload.ShipmentOptions;
                model.ShipmentLegOptions = reload.ShipmentLegOptions;

                return View(
                    "~/Views/CompanyPortal/Operations/CreateCapacityReservation.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Capacity reservation беше добавен успешно.";
            return RedirectToAction(nameof(CapacityReservations));
        }

        [HttpGet]
        public async Task<IActionResult> EditCapacityReservation(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetCapacityReservationForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditCapacityReservation.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCapacityReservation(CapacityReservationEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCapacityReservationForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.ResourceOptions = reload.ResourceOptions;
                    model.ShipmentOptions = reload.ShipmentOptions;
                    model.ShipmentLegOptions = reload.ShipmentLegOptions;
                }

                return View("~/Views/CompanyPortal/Operations/EditCapacityReservation.cshtml", model);
            }

            var success = await _companyPortalService.UpdateCapacityReservationAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на capacity reservation.";
                return View(
                    "~/Views/CompanyPortal/Operations/EditCapacityReservation.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "Capacity reservation беше обновен успешно.";
            return RedirectToAction(nameof(CapacityReservations));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCapacityReservation(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteCapacityReservationAsync(
                companyId.Value,
                id
            );

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Capacity reservation беше изтрит успешно."
                    : "Неуспешно изтриване на capacity reservation.";

            return RedirectToAction(nameof(CapacityReservations));
        }

        [HttpGet]
        public async Task<IActionResult> Assignments()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/Assignments.cshtml",
                await _companyPortalService.GetAssignmentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateAssignment()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/CreateAssignment.cshtml",
                await _companyPortalService.GetCreateAssignmentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(AssignmentCreateViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateAssignmentModelAsync(
                    companyId.Value
                );
                model.ShipmentLegOptions = reload.ShipmentLegOptions;
                model.ResourceOptions = reload.ResourceOptions;

                return View("~/Views/CompanyPortal/Operations/CreateAssignment.cshtml", model);
            }

            var id = await _companyPortalService.CreateAssignmentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на assignment.";

                var reload = await _companyPortalService.GetCreateAssignmentModelAsync(
                    companyId.Value
                );
                model.ShipmentLegOptions = reload.ShipmentLegOptions;
                model.ResourceOptions = reload.ResourceOptions;

                return View("~/Views/CompanyPortal/Operations/CreateAssignment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Assignment беше добавен успешно.";
            return RedirectToAction(nameof(Assignments));
        }

        [HttpGet]
        public async Task<IActionResult> EditAssignment(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetAssignmentForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Operations/EditAssignment.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignment(AssignmentEditViewModel model)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetAssignmentForEditAsync(
                    companyId.Value,
                    model.Id
                );
                if (reload != null)
                {
                    model.ShipmentLegOptions = reload.ShipmentLegOptions;
                    model.ResourceOptions = reload.ResourceOptions;
                }

                return View("~/Views/CompanyPortal/Operations/EditAssignment.cshtml", model);
            }

            var success = await _companyPortalService.UpdateAssignmentAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на assignment.";
                return View("~/Views/CompanyPortal/Operations/EditAssignment.cshtml", model);
            }

            TempData["SuccessMessage"] = "Assignment беше обновен успешно.";
            return RedirectToAction(nameof(Assignments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteAssignmentAsync(companyId.Value, id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Assignment беше изтрит успешно." : "Неуспешно изтриване на assignment.";

            return RedirectToAction(nameof(Assignments));
        }

        [HttpGet]
        public async Task<IActionResult> UtilizationSnapshots()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Operations/UtilizationSnapshots.cshtml",
                await _companyPortalService.GetUtilizationSnapshotsAsync(companyId.Value)
            );
        }
    }
}