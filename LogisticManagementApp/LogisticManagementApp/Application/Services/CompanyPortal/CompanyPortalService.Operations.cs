using LogisticManagementApp.Domain.Assets.CargoUnits;
using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Domain.Assets.Sea;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations;
using LogisticManagementApp.Domain.Operations.Audit;
using LogisticManagementApp.Domain.Operations.Notifications;
using LogisticManagementApp.Domain.Operations.Planning;
using LogisticManagementApp.Domain.Operations.Preferences;
using LogisticManagementApp.Models.CompanyPortal.Operations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyOperationsHomeViewModel> GetOperationsHomeAsync() =>
            Task.FromResult(
                new CompanyOperationsHomeViewModel
                {
                    Cards = new[]
                    {
                        new OperationCardViewModel
                        {
                            Title = "Notifications",
                            Description = "Известия към фирмата и потребителите.",
                            ActionName = "Notifications",
                            CreateActionName = "CreateNotification"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Notification Subscriptions",
                            Description = "Абонаменти за събития и канали.",
                            ActionName = "NotificationSubscriptions",
                            CreateActionName = "CreateNotificationSubscription"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Audit Logs",
                            Description = "Read-only следа на действията.",
                            ActionName = "AuditLogs"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Saved Filters",
                            Description = "Запазени филтри за списъци и модули.",
                            ActionName = "SavedFilters",
                            CreateActionName = "CreateSavedFilter"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Dashboard Configs",
                            Description = "Конфигурации на dashboard изгледите.",
                            ActionName = "CompanyDashboardConfigs",
                            CreateActionName = "CreateCompanyDashboardConfig"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Bookings",
                            Description = "Оперативни booking записи към carrier/shipment.",
                            ActionName = "Bookings",
                            CreateActionName = "CreateBooking"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Booking Legs",
                            Description = "Leg записи към booking.",
                            ActionName = "BookingLegs",
                            CreateActionName = "CreateBookingLeg"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Consolidations",
                            Description = "Read-only консолидирани групирания по shipment ownership.",
                            ActionName = "Consolidations"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Consolidation Shipments",
                            Description = "Shipment участия в consolidations.",
                            ActionName = "ConsolidationShipments",
                            CreateActionName = "CreateConsolidationShipment"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Resource Calendars",
                            Description = "Капацитет и заетост по ресурси.",
                            ActionName = "ResourceCalendars",
                            CreateActionName = "CreateResourceCalendar"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Resource Availabilities",
                            Description = "Наличност на фирмени ресурси.",
                            ActionName = "ResourceAvailabilities",
                            CreateActionName = "CreateResourceAvailability"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Capacity Reservations",
                            Description = "Резервиране на capacity към shipment/leg.",
                            ActionName = "CapacityReservations",
                            CreateActionName = "CreateCapacityReservation"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Assignments",
                            Description = "Назначаване на ресурси към shipment legs.",
                            ActionName = "Assignments",
                            CreateActionName = "CreateAssignment"
                        },
                        new OperationCardViewModel
                        {
                            Title = "Utilization Snapshots",
                            Description = "Read-only snapshots за utilization.",
                            ActionName = "UtilizationSnapshots"
                        }
                    }
                });

        public async Task<IEnumerable<NotificationListItemViewModel>> GetNotificationsAsync(Guid companyId) =>
            await _dbContext.Notifications
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    (x.RecipientCompanyId == companyId ||
                     (x.RecipientUser != null && x.RecipientUser.CompanyId == companyId)))
                .OrderByDescending(x => x.SentAtUtc ?? x.CreatedAtUtc)
                .Select(x => new NotificationListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    NotificationType = x.NotificationType.ToString(),
                    Channel = x.Channel.ToString(),
                    RecipientUserDisplay = x.RecipientUser != null
                        ? (x.RecipientUser.UserName ?? x.RecipientUser.Email)
                        : null,
                    IsRead = x.IsRead,
                    SentAtUtc = x.SentAtUtc,
                    RelatedEntityType = x.RelatedEntityType,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<NotificationCreateViewModel> GetCreateNotificationModelAsync(Guid companyId) =>
            new NotificationCreateViewModel
            {
                UserOptions = await GetCompanyUserOptionsAsync(companyId)
            };

        public async Task<NotificationEditViewModel?> GetNotificationForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.Notifications
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    (x.RecipientCompanyId == companyId ||
                     (x.RecipientUser != null && x.RecipientUser.CompanyId == companyId)))
                .Select(x => new NotificationEditViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    NotificationType = x.NotificationType,
                    Channel = x.Channel,
                    RecipientUserId = x.RecipientUserId,
                    IsRead = x.IsRead,
                    ReadAtUtc = x.ReadAtUtc,
                    SentAtUtc = x.SentAtUtc,
                    RelatedEntityType = x.RelatedEntityType,
                    RelatedEntityId = x.RelatedEntityId,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.UserOptions = await GetCompanyUserOptionsAsync(companyId);
            }

            return model;
        }

        public async Task<Guid?> CreateNotificationAsync(Guid companyId, NotificationCreateViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.RecipientUserId) &&
                !await CompanyOwnsUserAsync(companyId, model.RecipientUserId))
            {
                return null;
            }

            var entity = new Notification
            {
                Title = model.Title,
                Message = model.Message,
                NotificationType = model.NotificationType,
                Channel = model.Channel,
                RecipientUserId = string.IsNullOrWhiteSpace(model.RecipientUserId) ? null : model.RecipientUserId,
                RecipientCompanyId = companyId,
                IsRead = model.IsRead,
                ReadAtUtc = model.ReadAtUtc,
                SentAtUtc = model.SentAtUtc,
                RelatedEntityType = model.RelatedEntityType,
                RelatedEntityId = model.RelatedEntityId,
                Notes = model.Notes
            };

            _dbContext.Notifications.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateNotificationAsync(Guid companyId, NotificationEditViewModel model)
        {
            var entity = await _dbContext.Notifications
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    (x.RecipientCompanyId == companyId ||
                     (x.RecipientUser != null && x.RecipientUser.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(model.RecipientUserId) &&
                !await CompanyOwnsUserAsync(companyId, model.RecipientUserId))
            {
                return false;
            }

            entity.Title = model.Title;
            entity.Message = model.Message;
            entity.NotificationType = model.NotificationType;
            entity.Channel = model.Channel;
            entity.RecipientUserId = string.IsNullOrWhiteSpace(model.RecipientUserId) ? null : model.RecipientUserId;
            entity.RecipientCompanyId = companyId;
            entity.IsRead = model.IsRead;
            entity.ReadAtUtc = model.ReadAtUtc;
            entity.SentAtUtc = model.SentAtUtc;
            entity.RelatedEntityType = model.RelatedEntityType;
            entity.RelatedEntityId = model.RelatedEntityId;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Notifications
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    (x.RecipientCompanyId == companyId ||
                     (x.RecipientUser != null && x.RecipientUser.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NotificationSubscriptionListItemViewModel>> GetNotificationSubscriptionsAsync(Guid companyId) =>
            await _dbContext.NotificationSubscriptions
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)))
                .OrderBy(x => x.EventKey)
                .Select(x => new NotificationSubscriptionListItemViewModel
                {
                    Id = x.Id,
                    UserDisplay = x.User != null ? (x.User.UserName ?? x.User.Email) : null,
                    EventKey = x.EventKey,
                    Channel = x.Channel.ToString(),
                    IsEnabled = x.IsEnabled,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<NotificationSubscriptionCreateViewModel> GetCreateNotificationSubscriptionModelAsync(Guid companyId) =>
            new NotificationSubscriptionCreateViewModel
            {
                UserOptions = await GetCompanyUserOptionsAsync(companyId)
            };

        public async Task<NotificationSubscriptionEditViewModel?> GetNotificationSubscriptionForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.NotificationSubscriptions
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)))
                .Select(x => new NotificationSubscriptionEditViewModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    EventKey = x.EventKey,
                    Channel = x.Channel,
                    IsEnabled = x.IsEnabled,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.UserOptions = await GetCompanyUserOptionsAsync(companyId);
            }

            return model;
        }

        public async Task<Guid?> CreateNotificationSubscriptionAsync(Guid companyId, NotificationSubscriptionCreateViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.UserId) &&
                !await CompanyOwnsUserAsync(companyId, model.UserId))
            {
                return null;
            }

            var entity = new NotificationSubscription
            {
                UserId = string.IsNullOrWhiteSpace(model.UserId) ? null : model.UserId,
                CompanyId = companyId,
                EventKey = model.EventKey,
                Channel = model.Channel,
                IsEnabled = model.IsEnabled,
                Notes = model.Notes
            };

            _dbContext.NotificationSubscriptions.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateNotificationSubscriptionAsync(Guid companyId, NotificationSubscriptionEditViewModel model)
        {
            var entity = await _dbContext.NotificationSubscriptions
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(model.UserId) &&
                !await CompanyOwnsUserAsync(companyId, model.UserId))
            {
                return false;
            }

            entity.UserId = string.IsNullOrWhiteSpace(model.UserId) ? null : model.UserId;
            entity.CompanyId = companyId;
            entity.EventKey = model.EventKey;
            entity.Channel = model.Channel;
            entity.IsEnabled = model.IsEnabled;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationSubscriptionAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.NotificationSubscriptions
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AuditLogListItemViewModel>> GetAuditLogsAsync(Guid companyId)
        {
            var shipmentIds = await GetCompanyShipmentIdsAsync(companyId);

            return await _dbContext.AuditLogs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    (
                        (x.EntityType == "Shipment" && shipmentIds.Contains(x.EntityId)) ||
                        (x.EntityType == "Invoice" &&
                         _dbContext.Invoices.Any(i =>
                             !i.IsDeleted &&
                             i.Id == x.EntityId &&
                             i.BillToCompanyId == companyId)) ||
                        (x.EntityType == "Booking" &&
                         _dbContext.Bookings.Any(b =>
                             !b.IsDeleted &&
                             b.Id == x.EntityId &&
                             ((b.ShipmentId != null && b.Shipment!.CustomerCompanyId == companyId) ||
                              b.CarrierCompanyId == companyId)))
                    ))
                .OrderByDescending(x => x.ActionAtUtc)
                .Select(x => new AuditLogListItemViewModel
                {
                    Id = x.Id,
                    ActionType = x.ActionType.ToString(),
                    EntityType = x.EntityType,
                    EntityId = x.EntityId,
                    UserName = x.UserName,
                    ActionAtUtc = x.ActionAtUtc,
                    IpAddress = x.IpAddress,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SavedFilterListItemViewModel>> GetSavedFiltersAsync(Guid companyId) =>
            await _dbContext.SavedFilters
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)))
                .OrderBy(x => x.EntityType)
                .ThenBy(x => x.Name)
                .Select(x => new SavedFilterListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    EntityType = x.EntityType,
                    UserDisplay = x.User != null ? (x.User.UserName ?? x.User.Email) : null,
                    IsDefault = x.IsDefault,
                    FilterJson = x.FilterJson
                })
                .ToListAsync();

        public async Task<SavedFilterCreateViewModel> GetCreateSavedFilterModelAsync(Guid companyId) =>
            new SavedFilterCreateViewModel
            {
                UserOptions = await GetCompanyUserOptionsAsync(companyId)
            };

        public async Task<SavedFilterEditViewModel?> GetSavedFilterForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.SavedFilters
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)))
                .Select(x => new SavedFilterEditViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    EntityType = x.EntityType,
                    FilterJson = x.FilterJson,
                    UserId = x.UserId,
                    IsDefault = x.IsDefault
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.UserOptions = await GetCompanyUserOptionsAsync(companyId);
            }

            return model;
        }

        public async Task<Guid?> CreateSavedFilterAsync(Guid companyId, SavedFilterCreateViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.UserId) &&
                !await CompanyOwnsUserAsync(companyId, model.UserId))
            {
                return null;
            }

            var entity = new SavedFilter
            {
                Name = model.Name,
                EntityType = model.EntityType,
                FilterJson = model.FilterJson,
                UserId = string.IsNullOrWhiteSpace(model.UserId) ? null : model.UserId,
                CompanyId = companyId,
                IsDefault = model.IsDefault
            };

            _dbContext.SavedFilters.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateSavedFilterAsync(Guid companyId, SavedFilterEditViewModel model)
        {
            var entity = await _dbContext.SavedFilters
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(model.UserId) &&
                !await CompanyOwnsUserAsync(companyId, model.UserId))
            {
                return false;
            }

            entity.Name = model.Name;
            entity.EntityType = model.EntityType;
            entity.FilterJson = model.FilterJson;
            entity.UserId = string.IsNullOrWhiteSpace(model.UserId) ? null : model.UserId;
            entity.CompanyId = companyId;
            entity.IsDefault = model.IsDefault;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSavedFilterAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.SavedFilters
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    (x.CompanyId == companyId ||
                     (x.User != null && x.User.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CompanyDashboardConfigListItemViewModel>> GetCompanyDashboardConfigsAsync(Guid companyId) =>
            await _dbContext.CompanyDashboardConfigs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.DashboardKey)
                .Select(x => new CompanyDashboardConfigListItemViewModel
                {
                    Id = x.Id,
                    DashboardKey = x.DashboardKey,
                    IsActive = x.IsActive,
                    LayoutJson = x.LayoutJson,
                    WidgetSettingsJson = x.WidgetSettingsJson
                })
                .ToListAsync();

        public Task<CompanyDashboardConfigCreateViewModel> GetCreateCompanyDashboardConfigModelAsync(Guid companyId) =>
            Task.FromResult(new CompanyDashboardConfigCreateViewModel());

        public async Task<CompanyDashboardConfigEditViewModel?> GetCompanyDashboardConfigForEditAsync(Guid companyId, Guid id) =>
            await _dbContext.CompanyDashboardConfigs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id && x.CompanyId == companyId)
                .Select(x => new CompanyDashboardConfigEditViewModel
                {
                    Id = x.Id,
                    DashboardKey = x.DashboardKey,
                    LayoutJson = x.LayoutJson,
                    WidgetSettingsJson = x.WidgetSettingsJson,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();

        public async Task<Guid?> CreateCompanyDashboardConfigAsync(Guid companyId, CompanyDashboardConfigCreateViewModel model)
        {
            var entity = new CompanyDashboardConfig
            {
                CompanyId = companyId,
                DashboardKey = model.DashboardKey,
                LayoutJson = model.LayoutJson,
                WidgetSettingsJson = model.WidgetSettingsJson,
                IsActive = model.IsActive
            };

            _dbContext.CompanyDashboardConfigs.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateCompanyDashboardConfigAsync(Guid companyId, CompanyDashboardConfigEditViewModel model)
        {
            var entity = await _dbContext.CompanyDashboardConfigs
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.DashboardKey = model.DashboardKey;
            entity.LayoutJson = model.LayoutJson;
            entity.WidgetSettingsJson = model.WidgetSettingsJson;
            entity.IsActive = model.IsActive;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCompanyDashboardConfigAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.CompanyDashboardConfigs
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingListItemViewModel>> GetBookingsAsync(Guid companyId) =>
            await _dbContext.Bookings
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     x.CarrierCompanyId == companyId))
                .OrderByDescending(x => x.RequestedAtUtc ?? x.CreatedAtUtc)
                .Select(x => new BookingListItemViewModel
                {
                    Id = x.Id,
                    BookingNo = x.BookingNo,
                    CarrierCompanyName = x.CarrierCompany != null ? x.CarrierCompany.Name : null,
                    ShipmentNo = x.Shipment != null ? x.Shipment.ShipmentNo : null,
                    Status = x.Status.ToString(),
                    TransportMode = x.TransportMode.ToString(),
                    RequestedAtUtc = x.RequestedAtUtc,
                    ConfirmedAtUtc = x.ConfirmedAtUtc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<BookingCreateViewModel> GetCreateBookingModelAsync(Guid companyId) =>
            new BookingCreateViewModel
            {
                CarrierCompanyOptions = await GetCarrierCompanyOptionsAsync(),
                ShipmentOptions = await GetCompanyShipmentOptionsAsync(companyId)
            };

        public async Task<BookingEditViewModel?> GetBookingForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.Bookings
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     x.CarrierCompanyId == companyId))
                .Select(x => new BookingEditViewModel
                {
                    Id = x.Id,
                    BookingNo = x.BookingNo,
                    CarrierCompanyId = x.CarrierCompanyId,
                    ShipmentId = x.ShipmentId,
                    Status = x.Status,
                    TransportMode = x.TransportMode,
                    RequestedAtUtc = x.RequestedAtUtc,
                    ConfirmedAtUtc = x.ConfirmedAtUtc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.CarrierCompanyOptions = await GetCarrierCompanyOptionsAsync();
                model.ShipmentOptions = await GetCompanyShipmentOptionsAsync(companyId);
            }

            return model;
        }

        public async Task<Guid?> CreateBookingAsync(Guid companyId, BookingCreateViewModel model)
        {
            if (model.ShipmentId.HasValue &&
                !await CompanyOwnsShipmentAsync(companyId, model.ShipmentId.Value) &&
                model.CarrierCompanyId != companyId)
            {
                return null;
            }

            var entity = new Booking
            {
                BookingNo = model.BookingNo,
                CarrierCompanyId = model.CarrierCompanyId,
                ShipmentId = model.ShipmentId,
                Status = model.Status,
                TransportMode = model.TransportMode,
                RequestedAtUtc = model.RequestedAtUtc,
                ConfirmedAtUtc = model.ConfirmedAtUtc,
                CarrierReference = model.CarrierReference,
                Notes = model.Notes
            };

            _dbContext.Bookings.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateBookingAsync(Guid companyId, BookingEditViewModel model)
        {
            var entity = await _dbContext.Bookings
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     x.CarrierCompanyId == companyId));

            if (entity == null)
            {
                return false;
            }

            if (model.ShipmentId.HasValue &&
                !await CompanyOwnsShipmentAsync(companyId, model.ShipmentId.Value) &&
                model.CarrierCompanyId != companyId)
            {
                return false;
            }

            entity.BookingNo = model.BookingNo;
            entity.CarrierCompanyId = model.CarrierCompanyId;
            entity.ShipmentId = model.ShipmentId;
            entity.Status = model.Status;
            entity.TransportMode = model.TransportMode;
            entity.RequestedAtUtc = model.RequestedAtUtc;
            entity.ConfirmedAtUtc = model.ConfirmedAtUtc;
            entity.CarrierReference = model.CarrierReference;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Bookings
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     x.CarrierCompanyId == companyId));

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingLegListItemViewModel>> GetBookingLegsAsync(Guid companyId) =>
            await _dbContext.BookingLegs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.Booking.ShipmentId != null && x.Booking.Shipment!.CustomerCompanyId == companyId) ||
                     x.Booking.CarrierCompanyId == companyId))
                .OrderBy(x => x.Booking.BookingNo)
                .ThenBy(x => x.LegNo)
                .Select(x => new BookingLegListItemViewModel
                {
                    Id = x.Id,
                    BookingDisplay = x.Booking.BookingNo,
                    LegNo = x.LegNo,
                    ShipmentLegDisplay = x.ShipmentLeg != null
                        ? x.ShipmentLeg.Shipment.ShipmentNo + " / Leg " + x.ShipmentLeg.LegNo
                        : null,
                    OriginLocationName = x.OriginLocation != null ? x.OriginLocation.Name : null,
                    DestinationLocationName = x.DestinationLocation != null ? x.DestinationLocation.Name : null,
                    ETD_Utc = x.ETD_Utc,
                    ETA_Utc = x.ETA_Utc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<BookingLegCreateViewModel> GetCreateBookingLegModelAsync(Guid companyId) =>
            new BookingLegCreateViewModel
            {
                BookingOptions = await GetBookingOptionsAsync(companyId),
                ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId),
                LocationOptions = await GetOperationLocationOptionsAsync()
            };

        public async Task<BookingLegEditViewModel?> GetBookingLegForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.BookingLegs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.Booking.ShipmentId != null && x.Booking.Shipment!.CustomerCompanyId == companyId) ||
                     x.Booking.CarrierCompanyId == companyId))
                .Select(x => new BookingLegEditViewModel
                {
                    Id = x.Id,
                    BookingId = x.BookingId,
                    ShipmentLegId = x.ShipmentLegId,
                    LegNo = x.LegNo,
                    OriginLocationId = x.OriginLocationId,
                    DestinationLocationId = x.DestinationLocationId,
                    ETD_Utc = x.ETD_Utc,
                    ETA_Utc = x.ETA_Utc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.BookingOptions = await GetBookingOptionsAsync(companyId);
                model.ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId);
                model.LocationOptions = await GetOperationLocationOptionsAsync();
            }

            return model;
        }

        public async Task<Guid?> CreateBookingLegAsync(Guid companyId, BookingLegCreateViewModel model)
        {
            if (!await CompanyOwnsBookingAsync(companyId, model.BookingId))
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue &&
                !await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return null;
            }

            var entity = new BookingLeg
            {
                BookingId = model.BookingId,
                ShipmentLegId = model.ShipmentLegId,
                LegNo = model.LegNo,
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                ETD_Utc = model.ETD_Utc,
                ETA_Utc = model.ETA_Utc,
                CarrierReference = model.CarrierReference,
                Notes = model.Notes
            };

            _dbContext.BookingLegs.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateBookingLegAsync(Guid companyId, BookingLegEditViewModel model)
        {
            var entity = await _dbContext.BookingLegs
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    ((x.Booking.ShipmentId != null && x.Booking.Shipment!.CustomerCompanyId == companyId) ||
                     x.Booking.CarrierCompanyId == companyId));

            if (entity == null)
            {
                return false;
            }

            if (!await CompanyOwnsBookingAsync(companyId, model.BookingId))
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue &&
                !await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return false;
            }

            entity.BookingId = model.BookingId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.LegNo = model.LegNo;
            entity.OriginLocationId = model.OriginLocationId;
            entity.DestinationLocationId = model.DestinationLocationId;
            entity.ETD_Utc = model.ETD_Utc;
            entity.ETA_Utc = model.ETA_Utc;
            entity.CarrierReference = model.CarrierReference;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingLegAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.BookingLegs
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.Booking.ShipmentId != null && x.Booking.Shipment!.CustomerCompanyId == companyId) ||
                     x.Booking.CarrierCompanyId == companyId));

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ConsolidationListItemViewModel>> GetConsolidationsAsync(Guid companyId) =>
            await _dbContext.Consolidations
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Shipments.Any(s => !s.IsDeleted && s.Shipment.CustomerCompanyId == companyId))
                .OrderByDescending(x => x.PlannedDepartureUtc ?? x.CreatedAtUtc)
                .Select(x => new ConsolidationListItemViewModel
                {
                    Id = x.Id,
                    ConsolidationNo = x.ConsolidationNo,
                    ConsolidationType = x.ConsolidationType.ToString(),
                    Status = x.Status.ToString(),
                    TransportMode = x.TransportMode.ToString(),
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    MasterReference = x.MasterReference,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<IEnumerable<ConsolidationShipmentListItemViewModel>> GetConsolidationShipmentsAsync(Guid companyId) =>
            await _dbContext.ConsolidationShipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ConsolidationShipmentListItemViewModel
                {
                    Id = x.Id,
                    ConsolidationDisplay = x.Consolidation.ConsolidationNo,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    ShipmentLegDisplay = x.ShipmentLeg != null
                        ? x.ShipmentLeg.Shipment.ShipmentNo + " / Leg " + x.ShipmentLeg.LegNo
                        : null,
                    AllocatedWeightKg = x.AllocatedWeightKg,
                    AllocatedVolumeCbm = x.AllocatedVolumeCbm,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<ConsolidationShipmentCreateViewModel> GetCreateConsolidationShipmentModelAsync(Guid companyId) =>
            new ConsolidationShipmentCreateViewModel
            {
                ConsolidationOptions = await GetVisibleConsolidationOptionsAsync(companyId),
                ShipmentOptions = await GetCompanyShipmentOptionsAsync(companyId),
                ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId)
            };

        public async Task<ConsolidationShipmentEditViewModel?> GetConsolidationShipmentForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.ConsolidationShipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id && x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new ConsolidationShipmentEditViewModel
                {
                    Id = x.Id,
                    ConsolidationId = x.ConsolidationId,
                    ShipmentId = x.ShipmentId,
                    ShipmentLegId = x.ShipmentLegId,
                    AllocatedWeightKg = x.AllocatedWeightKg,
                    AllocatedVolumeCbm = x.AllocatedVolumeCbm,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                model.ConsolidationOptions = await GetVisibleConsolidationOptionsAsync(companyId);
                model.ShipmentOptions = await GetCompanyShipmentOptionsAsync(companyId);
                model.ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId);
            }

            return model;
        }

        public async Task<Guid?> CreateConsolidationShipmentAsync(Guid companyId, ConsolidationShipmentCreateViewModel model)
        {
            if (!await CompanyOwnsShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue &&
                !await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return null;
            }

            if (!await VisibleConsolidationAsync(companyId, model.ConsolidationId))
            {
                return null;
            }

            var entity = new ConsolidationShipment
            {
                ConsolidationId = model.ConsolidationId,
                ShipmentId = model.ShipmentId,
                ShipmentLegId = model.ShipmentLegId,
                AllocatedWeightKg = model.AllocatedWeightKg,
                AllocatedVolumeCbm = model.AllocatedVolumeCbm,
                Notes = model.Notes
            };

            _dbContext.ConsolidationShipments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateConsolidationShipmentAsync(Guid companyId, ConsolidationShipmentEditViewModel model)
        {
            var entity = await _dbContext.ConsolidationShipments
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            if (!await CompanyOwnsShipmentAsync(companyId, model.ShipmentId))
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue &&
                !await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return false;
            }

            if (!await VisibleConsolidationAsync(companyId, model.ConsolidationId))
            {
                return false;
            }

            entity.ConsolidationId = model.ConsolidationId;
            entity.ShipmentId = model.ShipmentId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.AllocatedWeightKg = model.AllocatedWeightKg;
            entity.AllocatedVolumeCbm = model.AllocatedVolumeCbm;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteConsolidationShipmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ConsolidationShipments
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ResourceCalendarListItemViewModel>> GetResourceCalendarsAsync(Guid companyId)
        {
            var lookup = await GetOperationResourceLookupAsync(companyId);

            return _dbContext.ResourceCalendars
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.DateUtc)
                .AsEnumerable()
                .Where(x => lookup.ContainsKey((x.ResourceType, x.ResourceId)))
                .Select(x => new ResourceCalendarListItemViewModel
                {
                    Id = x.Id,
                    ResourceType = x.ResourceType.ToString(),
                    ResourceDisplay = lookup[(x.ResourceType, x.ResourceId)],
                    DateUtc = x.DateUtc,
                    Status = x.Status.ToString(),
                    PlannedCapacity = x.PlannedCapacity,
                    ReservedCapacity = x.ReservedCapacity,
                    UsedCapacity = x.UsedCapacity,
                    Notes = x.Notes
                })
                .ToList();
        }

        public async Task<ResourceCalendarCreateViewModel> GetCreateResourceCalendarModelAsync(Guid companyId) =>
            new ResourceCalendarCreateViewModel
            {
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId)
            };

        public async Task<ResourceCalendarEditViewModel?> GetResourceCalendarForEditAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ResourceCalendars
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return null;
            }

            return new ResourceCalendarEditViewModel
            {
                Id = entity.Id,
                ResourceType = entity.ResourceType,
                ResourceId = entity.ResourceId,
                DateUtc = entity.DateUtc,
                Status = entity.Status,
                PlannedCapacity = entity.PlannedCapacity,
                ReservedCapacity = entity.ReservedCapacity,
                UsedCapacity = entity.UsedCapacity,
                Notes = entity.Notes,
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId)
            };
        }

        public async Task<Guid?> CreateResourceCalendarAsync(Guid companyId, ResourceCalendarCreateViewModel model)
        {
            if (!await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return null;
            }

            var entity = new ResourceCalendar
            {
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                DateUtc = model.DateUtc,
                Status = model.Status,
                PlannedCapacity = model.PlannedCapacity,
                ReservedCapacity = model.ReservedCapacity,
                UsedCapacity = model.UsedCapacity,
                Notes = model.Notes
            };

            _dbContext.ResourceCalendars.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateResourceCalendarAsync(Guid companyId, ResourceCalendarEditViewModel model)
        {
            var entity = await _dbContext.ResourceCalendars
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return false;
            }

            entity.ResourceType = model.ResourceType;
            entity.ResourceId = model.ResourceId;
            entity.DateUtc = model.DateUtc;
            entity.Status = model.Status;
            entity.PlannedCapacity = model.PlannedCapacity;
            entity.ReservedCapacity = model.ReservedCapacity;
            entity.UsedCapacity = model.UsedCapacity;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteResourceCalendarAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ResourceCalendars
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ResourceAvailabilityListItemViewModel>> GetResourceAvailabilitiesAsync(Guid companyId)
        {
            var lookup = await GetOperationResourceLookupAsync(companyId);

            return _dbContext.ResourceAvailabilities
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.AvailableFromUtc)
                .AsEnumerable()
                .Where(x => lookup.ContainsKey((x.ResourceType, x.ResourceId)))
                .Select(x => new ResourceAvailabilityListItemViewModel
                {
                    Id = x.Id,
                    ResourceType = x.ResourceType.ToString(),
                    ResourceDisplay = lookup[(x.ResourceType, x.ResourceId)],
                    AvailableFromUtc = x.AvailableFromUtc,
                    AvailableToUtc = x.AvailableToUtc,
                    Status = x.Status.ToString(),
                    Reason = x.Reason,
                    Notes = x.Notes
                })
                .ToList();
        }

        public async Task<ResourceAvailabilityCreateViewModel> GetCreateResourceAvailabilityModelAsync(Guid companyId) =>
            new ResourceAvailabilityCreateViewModel
            {
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId)
            };

        public async Task<ResourceAvailabilityEditViewModel?> GetResourceAvailabilityForEditAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ResourceAvailabilities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return null;
            }

            return new ResourceAvailabilityEditViewModel
            {
                Id = entity.Id,
                ResourceType = entity.ResourceType,
                ResourceId = entity.ResourceId,
                AvailableFromUtc = entity.AvailableFromUtc,
                AvailableToUtc = entity.AvailableToUtc,
                Status = entity.Status,
                Reason = entity.Reason,
                Notes = entity.Notes,
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId)
            };
        }

        public async Task<Guid?> CreateResourceAvailabilityAsync(Guid companyId, ResourceAvailabilityCreateViewModel model)
        {
            if (!await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return null;
            }

            var entity = new ResourceAvailability
            {
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                AvailableFromUtc = model.AvailableFromUtc,
                AvailableToUtc = model.AvailableToUtc,
                Status = model.Status,
                Reason = model.Reason,
                Notes = model.Notes
            };

            _dbContext.ResourceAvailabilities.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateResourceAvailabilityAsync(Guid companyId, ResourceAvailabilityEditViewModel model)
        {
            var entity = await _dbContext.ResourceAvailabilities
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return false;
            }

            entity.ResourceType = model.ResourceType;
            entity.ResourceId = model.ResourceId;
            entity.AvailableFromUtc = model.AvailableFromUtc;
            entity.AvailableToUtc = model.AvailableToUtc;
            entity.Status = model.Status;
            entity.Reason = model.Reason;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteResourceAvailabilityAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ResourceAvailabilities
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CapacityReservationListItemViewModel>> GetCapacityReservationsAsync(Guid companyId)
        {
            var lookup = await GetOperationResourceLookupAsync(companyId);

            return _dbContext.CapacityReservations
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     (x.ShipmentLegId != null && x.ShipmentLeg!.Shipment.CustomerCompanyId == companyId)))
                .OrderByDescending(x => x.ReservedFromUtc ?? x.CreatedAtUtc)
                .AsEnumerable()
                .Where(x => lookup.ContainsKey((x.ResourceType, x.ResourceId)))
                .Select(x => new CapacityReservationListItemViewModel
                {
                    Id = x.Id,
                    ResourceType = x.ResourceType.ToString(),
                    ResourceDisplay = lookup[(x.ResourceType, x.ResourceId)],
                    ShipmentNo = x.Shipment?.ShipmentNo,
                    ShipmentLegDisplay = x.ShipmentLeg != null
                        ? x.ShipmentLeg.Shipment.ShipmentNo + " / Leg " + x.ShipmentLeg.LegNo
                        : null,
                    ReservedWeightKg = x.ReservedWeightKg,
                    ReservedVolumeCbm = x.ReservedVolumeCbm,
                    ReservedUnitCount = x.ReservedUnitCount,
                    ReservedFromUtc = x.ReservedFromUtc,
                    ReservedToUtc = x.ReservedToUtc,
                    Notes = x.Notes
                })
                .ToList();
        }

        public async Task<CapacityReservationCreateViewModel> GetCreateCapacityReservationModelAsync(Guid companyId) =>
            new CapacityReservationCreateViewModel
            {
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId),
                ShipmentOptions = await GetCompanyShipmentOptionsAsync(companyId),
                ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId)
            };

        public async Task<CapacityReservationEditViewModel?> GetCapacityReservationForEditAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.CapacityReservations
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     (x.ShipmentLegId != null && x.ShipmentLeg!.Shipment.CustomerCompanyId == companyId)));

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return null;
            }

            return new CapacityReservationEditViewModel
            {
                Id = entity.Id,
                ResourceType = entity.ResourceType,
                ResourceId = entity.ResourceId,
                ShipmentId = entity.ShipmentId,
                ShipmentLegId = entity.ShipmentLegId,
                ReservedWeightKg = entity.ReservedWeightKg,
                ReservedVolumeCbm = entity.ReservedVolumeCbm,
                ReservedUnitCount = entity.ReservedUnitCount,
                ReservedFromUtc = entity.ReservedFromUtc,
                ReservedToUtc = entity.ReservedToUtc,
                Notes = entity.Notes,
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId),
                ShipmentOptions = await GetCompanyShipmentOptionsAsync(companyId),
                ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId)
            };
        }

        public async Task<Guid?> CreateCapacityReservationAsync(Guid companyId, CapacityReservationCreateViewModel model)
        {
            if (!await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return null;
            }

            if (model.ShipmentId.HasValue &&
                !await CompanyOwnsShipmentAsync(companyId, model.ShipmentId.Value))
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue &&
                !await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return null;
            }

            var entity = new CapacityReservation
            {
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                ShipmentId = model.ShipmentId,
                ShipmentLegId = model.ShipmentLegId,
                ReservedWeightKg = model.ReservedWeightKg,
                ReservedVolumeCbm = model.ReservedVolumeCbm,
                ReservedUnitCount = model.ReservedUnitCount,
                ReservedFromUtc = model.ReservedFromUtc,
                ReservedToUtc = model.ReservedToUtc,
                Notes = model.Notes
            };

            _dbContext.CapacityReservations.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateCapacityReservationAsync(Guid companyId, CapacityReservationEditViewModel model)
        {
            var entity = await _dbContext.CapacityReservations
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     (x.ShipmentLegId != null && x.ShipmentLeg!.Shipment.CustomerCompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            if (!await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return false;
            }

            if (model.ShipmentId.HasValue &&
                !await CompanyOwnsShipmentAsync(companyId, model.ShipmentId.Value))
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue &&
                !await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return false;
            }

            entity.ResourceType = model.ResourceType;
            entity.ResourceId = model.ResourceId;
            entity.ShipmentId = model.ShipmentId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.ReservedWeightKg = model.ReservedWeightKg;
            entity.ReservedVolumeCbm = model.ReservedVolumeCbm;
            entity.ReservedUnitCount = model.ReservedUnitCount;
            entity.ReservedFromUtc = model.ReservedFromUtc;
            entity.ReservedToUtc = model.ReservedToUtc;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCapacityReservationAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.CapacityReservations
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     (x.ShipmentLegId != null && x.ShipmentLeg!.Shipment.CustomerCompanyId == companyId)));

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AssignmentListItemViewModel>> GetAssignmentsAsync(Guid companyId)
        {
            var lookup = await GetOperationResourceLookupAsync(companyId);

            return _dbContext.Assignments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentLeg.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.AssignedAtUtc ?? x.CreatedAtUtc)
                .AsEnumerable()
                .Where(x => lookup.ContainsKey((x.ResourceType, x.ResourceId)))
                .Select(x => new AssignmentListItemViewModel
                {
                    Id = x.Id,
                    ShipmentLegDisplay = x.ShipmentLeg.Shipment.ShipmentNo + " / Leg " + x.ShipmentLeg.LegNo,
                    ResourceType = x.ResourceType.ToString(),
                    ResourceDisplay = lookup[(x.ResourceType, x.ResourceId)],
                    Status = x.Status.ToString(),
                    AssignedAtUtc = x.AssignedAtUtc,
                    ReferenceNo = x.ReferenceNo,
                    Notes = x.Notes
                })
                .ToList();
        }

        public async Task<AssignmentCreateViewModel> GetCreateAssignmentModelAsync(Guid companyId) =>
            new AssignmentCreateViewModel
            {
                ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId),
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId)
            };

        public async Task<AssignmentEditViewModel?> GetAssignmentForEditAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Assignments
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    x.ShipmentLeg.Shipment.CustomerCompanyId == companyId);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return null;
            }

            return new AssignmentEditViewModel
            {
                Id = entity.Id,
                ShipmentLegId = entity.ShipmentLegId,
                ResourceType = entity.ResourceType,
                ResourceId = entity.ResourceId,
                Status = entity.Status,
                AssignedAtUtc = entity.AssignedAtUtc,
                ReferenceNo = entity.ReferenceNo,
                Notes = entity.Notes,
                ShipmentLegOptions = await GetCompanyShipmentLegOptionsAsync(companyId),
                ResourceOptions = await GetOperationResourceOptionsAsync(companyId)
            };
        }

        public async Task<Guid?> CreateAssignmentAsync(Guid companyId, AssignmentCreateViewModel model)
        {
            if (!await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId))
            {
                return null;
            }

            if (!await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return null;
            }

            var entity = new Assignment
            {
                ShipmentLegId = model.ShipmentLegId,
                ResourceType = model.ResourceType,
                ResourceId = model.ResourceId,
                Status = model.Status,
                AssignedAtUtc = model.AssignedAtUtc,
                ReferenceNo = model.ReferenceNo,
                Notes = model.Notes
            };

            _dbContext.Assignments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAssignmentAsync(Guid companyId, AssignmentEditViewModel model)
        {
            var entity = await _dbContext.Assignments
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    x.ShipmentLeg.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            if (!await CompanyOwnsShipmentLegAsync(companyId, model.ShipmentLegId) ||
                !await CompanyOwnsOperationResourceAsync(companyId, model.ResourceType, model.ResourceId))
            {
                return false;
            }

            entity.ShipmentLegId = model.ShipmentLegId;
            entity.ResourceType = model.ResourceType;
            entity.ResourceId = model.ResourceId;
            entity.Status = model.Status;
            entity.AssignedAtUtc = model.AssignedAtUtc;
            entity.ReferenceNo = model.ReferenceNo;
            entity.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAssignmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Assignments
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    x.ShipmentLeg.Shipment.CustomerCompanyId == companyId);

            if (entity == null || !await CompanyOwnsOperationResourceAsync(companyId, entity.ResourceType, entity.ResourceId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UtilizationSnapshotListItemViewModel>> GetUtilizationSnapshotsAsync(Guid companyId)
        {
            var lookup = await GetOperationResourceLookupAsync(companyId);

            return _dbContext.UtilizationSnapshots
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.SnapshotDateUtc)
                .AsEnumerable()
                .Where(x => lookup.ContainsKey((x.ResourceType, x.ResourceId)))
                .Select(x => new UtilizationSnapshotListItemViewModel
                {
                    Id = x.Id,
                    ResourceType = x.ResourceType.ToString(),
                    ResourceDisplay = lookup[(x.ResourceType, x.ResourceId)],
                    SnapshotDateUtc = x.SnapshotDateUtc,
                    TotalCapacity = x.TotalCapacity,
                    UsedCapacity = x.UsedCapacity,
                    FreeCapacity = x.FreeCapacity,
                    UtilizationPercent = x.UtilizationPercent,
                    Notes = x.Notes
                })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetCompanyUserOptionsAsync(Guid companyId) =>
            await _dbContext.AspNetUsers
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId)
                .OrderBy(x => x.UserName)
                .Select(x => new SelectListItem(x.UserName ?? x.Email ?? x.Id, x.Id))
                .ToListAsync();

        private async Task<List<SelectListItem>> GetCarrierCompanyOptionsAsync() =>
            await _dbContext.Companies
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();

        private async Task<List<SelectListItem>> GetCompanyShipmentOptionsAsync(Guid companyId) =>
            await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem(x.ShipmentNo, x.Id.ToString()))
                .ToListAsync();

        private async Task<List<SelectListItem>> GetCompanyShipmentLegOptionsAsync(Guid companyId) =>
            await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderBy(x => x.Shipment.ShipmentNo)
                .ThenBy(x => x.LegNo)
                .Select(x => new SelectListItem(
                    x.Shipment.ShipmentNo + " / Leg " + x.LegNo + " / " + x.Mode,
                    x.Id.ToString()))
                .ToListAsync();

        private async Task<List<SelectListItem>> GetBookingOptionsAsync(Guid companyId) =>
            await _dbContext.Bookings
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     x.CarrierCompanyId == companyId))
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem(x.BookingNo, x.Id.ToString()))
                .ToListAsync();

        private async Task<List<SelectListItem>> GetVisibleConsolidationOptionsAsync(Guid companyId) =>
            await _dbContext.Consolidations
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Shipments.Any(s => !s.IsDeleted && s.Shipment.CustomerCompanyId == companyId))
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem(x.ConsolidationNo, x.Id.ToString()))
                .ToListAsync();

        private async Task<List<SelectListItem>> GetOperationLocationOptionsAsync() =>
            await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name + " (" + x.Code + ")", x.Id.ToString()))
                .ToListAsync();

        private async Task<bool> CompanyOwnsUserAsync(Guid companyId, string userId) =>
            await _dbContext.AspNetUsers
                .AsNoTracking()
                .AnyAsync(x => x.Id == userId && x.CompanyId == companyId);

        private async Task<bool> CompanyOwnsShipmentAsync(Guid companyId, Guid shipmentId) =>
            await _dbContext.Shipments
                .AsNoTracking()
                .AnyAsync(x => !x.IsDeleted && x.Id == shipmentId && x.CustomerCompanyId == companyId);

        private async Task<bool> CompanyOwnsShipmentLegAsync(Guid companyId, Guid shipmentLegId) =>
            await _dbContext.ShipmentLegs
                .AsNoTracking()
                .AnyAsync(x => !x.IsDeleted && x.Id == shipmentLegId && x.Shipment.CustomerCompanyId == companyId);

        private async Task<bool> CompanyOwnsBookingAsync(Guid companyId, Guid bookingId) =>
            await _dbContext.Bookings
                .AsNoTracking()
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Id == bookingId &&
                    ((x.ShipmentId != null && x.Shipment!.CustomerCompanyId == companyId) ||
                     x.CarrierCompanyId == companyId));

        private async Task<bool> VisibleConsolidationAsync(Guid companyId, Guid consolidationId) =>
            await _dbContext.Consolidations
                .AsNoTracking()
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Id == consolidationId &&
                    x.Shipments.Any(s => !s.IsDeleted && s.Shipment.CustomerCompanyId == companyId));

        private async Task<List<Guid>> GetCompanyShipmentIdsAsync(Guid companyId) =>
            await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .Select(x => x.Id)
                .ToListAsync();

        private async Task<bool> CompanyOwnsOperationResourceAsync(Guid companyId, ResourceType type, Guid resourceId)
        {
            return type switch
            {
                ResourceType.Vessel => await _dbContext.Vessels
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDeleted && x.Id == resourceId && x.CompanyId == companyId),

                ResourceType.Vehicle => await _dbContext.Vehicles
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDeleted && x.Id == resourceId && x.CompanyId == companyId),

                ResourceType.Driver => await _dbContext.Drivers
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDeleted && x.Id == resourceId && x.CompanyId == companyId),

                ResourceType.Container => await _dbContext.Containers
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDeleted && x.Id == resourceId && x.OwnerCompanyId == companyId),

                ResourceType.Voyage => await _dbContext.Voyages
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDeleted && x.Id == resourceId && x.Vessel.CompanyId == companyId),

                ResourceType.Trip => await _dbContext.Trips
                    .AsNoTracking()
                    .AnyAsync(x =>
                        !x.IsDeleted &&
                        x.Id == resourceId &&
                        ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                         (x.DriverId != null && x.Driver!.CompanyId == companyId))),

                _ => false
            };
        }

        private async Task<Dictionary<(ResourceType, Guid), string>> GetOperationResourceLookupAsync(Guid companyId)
        {
            var dict = new Dictionary<(ResourceType, Guid), string>();

            foreach (var x in await _dbContext.Vessels
                         .AsNoTracking()
                         .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                         .Select(x => new { x.Id, x.Name })
                         .ToListAsync())
            {
                dict[(ResourceType.Vessel, x.Id)] = x.Name;
            }

            foreach (var x in await _dbContext.Vehicles
                         .AsNoTracking()
                         .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                         .Select(x => new { x.Id, x.RegistrationNumber })
                         .ToListAsync())
            {
                dict[(ResourceType.Vehicle, x.Id)] = x.RegistrationNumber;
            }

            foreach (var x in await _dbContext.Drivers
                         .AsNoTracking()
                         .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                         .Select(x => new { x.Id, x.FullName })
                         .ToListAsync())
            {
                dict[(ResourceType.Driver, x.Id)] = x.FullName;
            }

            foreach (var x in await _dbContext.Containers
                         .AsNoTracking()
                         .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId)
                         .Select(x => new { x.Id, x.ContainerNumber })
                         .ToListAsync())
            {
                dict[(ResourceType.Container, x.Id)] = x.ContainerNumber;
            }

            foreach (var x in await _dbContext.Voyages
                         .AsNoTracking()
                         .Where(x => !x.IsDeleted && x.Vessel.CompanyId == companyId)
                         .Select(x => new { x.Id, x.VoyageNumber })
                         .ToListAsync())
            {
                dict[(ResourceType.Voyage, x.Id)] = x.VoyageNumber;
            }

            foreach (var x in await _dbContext.Trips
                         .AsNoTracking()
                         .Where(x =>
                             !x.IsDeleted &&
                             ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                              (x.DriverId != null && x.Driver!.CompanyId == companyId)))
                         .Select(x => new { x.Id, x.TripNo })
                         .ToListAsync())
            {
                dict[(ResourceType.Trip, x.Id)] = x.TripNo;
            }

            return dict;
        }

        private async Task<List<SelectListItem>> GetOperationResourceOptionsAsync(Guid companyId)
        {
            var lookup = await GetOperationResourceLookupAsync(companyId);

            return lookup
                .OrderBy(x => x.Key.Item1.ToString())
                .ThenBy(x => x.Value)
                .Select(x => new SelectListItem($"[{x.Key.Item1}] {x.Value}", x.Key.Item2.ToString()))
                .ToList();
        }
    }
}