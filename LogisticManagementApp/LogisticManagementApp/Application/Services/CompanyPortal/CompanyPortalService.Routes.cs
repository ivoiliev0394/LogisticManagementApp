using LogisticManagementApp.Domain.Routes;
using LogisticManagementApp.Models.CompanyPortal.Routes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyRoutesHomeViewModel> GetRoutesHomeAsync() =>
            Task.FromResult(
                new CompanyRoutesHomeViewModel
                {
                    Cards = new[]
                    {
                        new RouteCardViewModel
                        {
                            Title = "Routes",
                            Description = "Основни маршрути на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Routes),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRoute)
                        },
                        new RouteCardViewModel
                        {
                            Title = "Route Stops",
                            Description = "Спирки и последователност по маршрут.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RouteStops),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRouteStop)
                        },
                        new RouteCardViewModel
                        {
                            Title = "Route Plans",
                            Description = "Дневни и оперативни планове по маршрут.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RoutePlans),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRoutePlan)
                        },
                        new RouteCardViewModel
                        {
                            Title = "Route Plan Stops",
                            Description = "Планирани спирания в конкретен route plan.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RoutePlanStops),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRoutePlanStop)
                        },
                        new RouteCardViewModel
                        {
                            Title = "Route Plan Shipments",
                            Description = "Пратки, вързани към конкретен route plan.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RoutePlanShipments),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRoutePlanShipment)
                        }
                    }
                });

        public async Task<IEnumerable<RouteListItemViewModel>> GetRoutesAsync(Guid companyId) =>
            await _dbContext.Routes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.RouteCode)
                .Select(x => new RouteListItemViewModel
                {
                    Id = x.Id,
                    RouteCode = x.RouteCode,
                    Name = x.Name,
                    TransportMode = x.TransportMode.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();

        public Task<RouteCreateViewModel> GetCreateRouteModelAsync(Guid companyId) =>
            Task.FromResult(new RouteCreateViewModel());

        public async Task<Guid?> CreateRouteAsync(Guid companyId, RouteCreateViewModel model)
        {
            var entity = new Domain.Routes.Route
            {
                CompanyId = companyId,
                RouteCode = model.RouteCode.Trim(),
                Name = model.Name.Trim(),
                TransportMode = model.TransportMode,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Routes.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RouteEditViewModel?> GetRouteForEditAsync(Guid companyId, Guid id) =>
            await _dbContext.Routes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new RouteEditViewModel
                {
                    Id = x.Id,
                    RouteCode = x.RouteCode,
                    Name = x.Name,
                    TransportMode = x.TransportMode,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

        public async Task<bool> UpdateRouteAsync(Guid companyId, RouteEditViewModel model)
        {
            var entity = await _dbContext.Routes
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.RouteCode = model.RouteCode.Trim();
            entity.Name = model.Name.Trim();
            entity.TransportMode = model.TransportMode;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRouteAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Routes
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RouteStopListItemViewModel>> GetRouteStopsAsync(Guid companyId) =>
            await _dbContext.RouteStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Route.CompanyId == companyId)
                .OrderBy(x => x.Route.RouteCode)
                .ThenBy(x => x.SequenceNo)
                .Select(x => new RouteStopListItemViewModel
                {
                    Id = x.Id,
                    RouteDisplay = x.Route.RouteCode + " / " + x.Route.Name,
                    LocationName = x.Location.Name,
                    SequenceNo = x.SequenceNo,
                    StopType = x.StopType.ToString(),
                    PlannedArrivalTime = x.PlannedArrivalTime,
                    PlannedDepartureTime = x.PlannedDepartureTime,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<RouteStopCreateViewModel> GetCreateRouteStopModelAsync(Guid companyId)
        {
            var model = new RouteStopCreateViewModel();
            await PopulateRouteStopOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateRouteStopAsync(Guid companyId, RouteStopCreateViewModel model)
        {
            if (!await OwnsRouteAsync(companyId, model.RouteId) || !await ExistsLocationAsync(model.LocationId))
            {
                return null;
            }

            var entity = new RouteStop
            {
                RouteId = model.RouteId,
                LocationId = model.LocationId,
                SequenceNo = model.SequenceNo,
                StopType = model.StopType,
                PlannedArrivalTime = model.PlannedArrivalTime,
                PlannedDepartureTime = model.PlannedDepartureTime,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RouteStops.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RouteStopEditViewModel?> GetRouteStopForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.RouteStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == id)
                .Select(x => new RouteStopEditViewModel
                {
                    Id = x.Id,
                    RouteId = x.RouteId,
                    LocationId = x.LocationId,
                    SequenceNo = x.SequenceNo,
                    StopType = x.StopType,
                    PlannedArrivalTime = x.PlannedArrivalTime,
                    PlannedDepartureTime = x.PlannedDepartureTime,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateRouteStopOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateRouteStopAsync(Guid companyId, RouteStopEditViewModel model)
        {
            var entity = await _dbContext.RouteStops
                .Include(x => x.Route)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == model.Id);

            if (entity == null || !await OwnsRouteAsync(companyId, model.RouteId) || !await ExistsLocationAsync(model.LocationId))
            {
                return false;
            }

            entity.RouteId = model.RouteId;
            entity.LocationId = model.LocationId;
            entity.SequenceNo = model.SequenceNo;
            entity.StopType = model.StopType;
            entity.PlannedArrivalTime = model.PlannedArrivalTime;
            entity.PlannedDepartureTime = model.PlannedDepartureTime;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRouteStopAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RouteStops
                .Include(x => x.Route)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RoutePlanListItemViewModel>> GetRoutePlansAsync(Guid companyId) =>
            await _dbContext.RoutePlans
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Route.CompanyId == companyId)
                .OrderByDescending(x => x.PlanDateUtc)
                .ThenBy(x => x.Route.RouteCode)
                .Select(x => new RoutePlanListItemViewModel
                {
                    Id = x.Id,
                    RouteDisplay = x.Route.RouteCode + " / " + x.Route.Name,
                    PlanDateUtc = x.PlanDateUtc,
                    Status = x.Status.ToString(),
                    PlanReference = x.PlanReference,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<RoutePlanCreateViewModel> GetCreateRoutePlanModelAsync(Guid companyId)
        {
            var model = new RoutePlanCreateViewModel();
            await PopulateRoutePlanOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateRoutePlanAsync(Guid companyId, RoutePlanCreateViewModel model)
        {
            if (!await OwnsRouteAsync(companyId, model.RouteId))
            {
                return null;
            }

            var entity = new RoutePlan
            {
                RouteId = model.RouteId,
                PlanDateUtc = model.PlanDateUtc,
                Status = model.Status,
                PlanReference = TrimOrNull(model.PlanReference),
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RoutePlans.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RoutePlanEditViewModel?> GetRoutePlanForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.RoutePlans
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == id)
                .Select(x => new RoutePlanEditViewModel
                {
                    Id = x.Id,
                    RouteId = x.RouteId,
                    PlanDateUtc = x.PlanDateUtc,
                    Status = x.Status,
                    PlanReference = x.PlanReference,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateRoutePlanOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateRoutePlanAsync(Guid companyId, RoutePlanEditViewModel model)
        {
            var entity = await _dbContext.RoutePlans
                .Include(x => x.Route)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == model.Id);

            if (entity == null || !await OwnsRouteAsync(companyId, model.RouteId))
            {
                return false;
            }

            entity.RouteId = model.RouteId;
            entity.PlanDateUtc = model.PlanDateUtc;
            entity.Status = model.Status;
            entity.PlanReference = TrimOrNull(model.PlanReference);
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoutePlanAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RoutePlans
                .Include(x => x.Route)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RoutePlanStopListItemViewModel>> GetRoutePlanStopsAsync(Guid companyId) =>
            await _dbContext.RoutePlanStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId)
                .OrderByDescending(x => x.RoutePlan.PlanDateUtc)
                .ThenBy(x => x.SequenceNo)
                .Select(x => new RoutePlanStopListItemViewModel
                {
                    Id = x.Id,
                    RoutePlanDisplay = x.RoutePlan.Route.RouteCode + " / " + x.RoutePlan.PlanDateUtc.ToString("yyyy-MM-dd"),
                    RouteStopDisplay = x.RouteStop != null
                        ? x.RouteStop.Route.RouteCode + " / #" + x.RouteStop.SequenceNo
                        : null,
                    LocationName = x.Location.Name,
                    SequenceNo = x.SequenceNo,
                    StopType = x.StopType.ToString(),
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<RoutePlanStopCreateViewModel> GetCreateRoutePlanStopModelAsync(Guid companyId)
        {
            var model = new RoutePlanStopCreateViewModel();
            await PopulateRoutePlanStopOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateRoutePlanStopAsync(Guid companyId, RoutePlanStopCreateViewModel model)
        {
            if (!await OwnsRoutePlanAsync(companyId, model.RoutePlanId) || !await ExistsLocationAsync(model.LocationId))
            {
                return null;
            }

            if (model.RouteStopId.HasValue && !await OwnsRouteStopAsync(companyId, model.RouteStopId.Value))
            {
                return null;
            }

            var entity = new RoutePlanStop
            {
                RoutePlanId = model.RoutePlanId,
                RouteStopId = model.RouteStopId,
                LocationId = model.LocationId,
                SequenceNo = model.SequenceNo,
                StopType = model.StopType,
                PlannedArrivalUtc = model.PlannedArrivalUtc,
                PlannedDepartureUtc = model.PlannedDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RoutePlanStops.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RoutePlanStopEditViewModel?> GetRoutePlanStopForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.RoutePlanStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId && x.Id == id)
                .Select(x => new RoutePlanStopEditViewModel
                {
                    Id = x.Id,
                    RoutePlanId = x.RoutePlanId,
                    RouteStopId = x.RouteStopId,
                    LocationId = x.LocationId,
                    SequenceNo = x.SequenceNo,
                    StopType = x.StopType,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateRoutePlanStopOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateRoutePlanStopAsync(Guid companyId, RoutePlanStopEditViewModel model)
        {
            var entity = await _dbContext.RoutePlanStops
                .Include(x => x.RoutePlan)
                .ThenInclude(x => x.Route)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId && x.Id == model.Id);

            if (entity == null || !await OwnsRoutePlanAsync(companyId, model.RoutePlanId) || !await ExistsLocationAsync(model.LocationId))
            {
                return false;
            }

            if (model.RouteStopId.HasValue && !await OwnsRouteStopAsync(companyId, model.RouteStopId.Value))
            {
                return false;
            }

            entity.RoutePlanId = model.RoutePlanId;
            entity.RouteStopId = model.RouteStopId;
            entity.LocationId = model.LocationId;
            entity.SequenceNo = model.SequenceNo;
            entity.StopType = model.StopType;
            entity.PlannedArrivalUtc = model.PlannedArrivalUtc;
            entity.PlannedDepartureUtc = model.PlannedDepartureUtc;
            entity.ActualArrivalUtc = model.ActualArrivalUtc;
            entity.ActualDepartureUtc = model.ActualDepartureUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoutePlanStopAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RoutePlanStops
                .Include(x => x.RoutePlan)
                .ThenInclude(x => x.Route)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RoutePlanShipmentListItemViewModel>> GetRoutePlanShipmentsAsync(Guid companyId) =>
            await _dbContext.RoutePlanShipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.RoutePlan.PlanDateUtc)
                .ThenBy(x => x.Priority)
                .Select(x => new RoutePlanShipmentListItemViewModel
                {
                    Id = x.Id,
                    RoutePlanDisplay = x.RoutePlan.Route.RouteCode + " / " + x.RoutePlan.PlanDateUtc.ToString("yyyy-MM-dd"),
                    ShipmentNo = x.Shipment.ShipmentNo,
                    PickupStopDisplay = x.PickupStop != null
                        ? "#" + x.PickupStop.SequenceNo + " / " + x.PickupStop.Location.Name
                        : null,
                    DeliveryStopDisplay = x.DeliveryStop != null
                        ? "#" + x.DeliveryStop.SequenceNo + " / " + x.DeliveryStop.Location.Name
                        : null,
                    Priority = x.Priority,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<RoutePlanShipmentCreateViewModel> GetCreateRoutePlanShipmentModelAsync(Guid companyId)
        {
            var model = new RoutePlanShipmentCreateViewModel();
            await PopulateRoutePlanShipmentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateRoutePlanShipmentAsync(Guid companyId, RoutePlanShipmentCreateViewModel model)
        {
            if (!await OwnsRoutePlanAsync(companyId, model.RoutePlanId) || !await OwnsShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            if (model.PickupStopId.HasValue && !await OwnsRoutePlanStopAsync(companyId, model.PickupStopId.Value))
            {
                return null;
            }

            if (model.DeliveryStopId.HasValue && !await OwnsRoutePlanStopAsync(companyId, model.DeliveryStopId.Value))
            {
                return null;
            }

            var entity = new RoutePlanShipment
            {
                RoutePlanId = model.RoutePlanId,
                ShipmentId = model.ShipmentId,
                PickupStopId = model.PickupStopId,
                DeliveryStopId = model.DeliveryStopId,
                Priority = model.Priority,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RoutePlanShipments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RoutePlanShipmentEditViewModel?> GetRoutePlanShipmentForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.RoutePlanShipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId && x.Shipment.CustomerCompanyId == companyId && x.Id == id)
                .Select(x => new RoutePlanShipmentEditViewModel
                {
                    Id = x.Id,
                    RoutePlanId = x.RoutePlanId,
                    ShipmentId = x.ShipmentId,
                    PickupStopId = x.PickupStopId,
                    DeliveryStopId = x.DeliveryStopId,
                    Priority = x.Priority,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateRoutePlanShipmentOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateRoutePlanShipmentAsync(Guid companyId, RoutePlanShipmentEditViewModel model)
        {
            var entity = await _dbContext.RoutePlanShipments
                .Include(x => x.RoutePlan)
                .ThenInclude(x => x.Route)
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.RoutePlan.Route.CompanyId == companyId &&
                    x.Shipment.CustomerCompanyId == companyId &&
                    x.Id == model.Id);

            if (entity == null || !await OwnsRoutePlanAsync(companyId, model.RoutePlanId) || !await OwnsShipmentAsync(companyId, model.ShipmentId))
            {
                return false;
            }

            if (model.PickupStopId.HasValue && !await OwnsRoutePlanStopAsync(companyId, model.PickupStopId.Value))
            {
                return false;
            }

            if (model.DeliveryStopId.HasValue && !await OwnsRoutePlanStopAsync(companyId, model.DeliveryStopId.Value))
            {
                return false;
            }

            entity.RoutePlanId = model.RoutePlanId;
            entity.ShipmentId = model.ShipmentId;
            entity.PickupStopId = model.PickupStopId;
            entity.DeliveryStopId = model.DeliveryStopId;
            entity.Priority = model.Priority;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoutePlanShipmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RoutePlanShipments
                .Include(x => x.RoutePlan)
                .ThenInclude(x => x.Route)
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.RoutePlan.Route.CompanyId == companyId &&
                    x.Shipment.CustomerCompanyId == companyId &&
                    x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private Task<bool> OwnsRouteAsync(Guid companyId, Guid routeId) =>
            _dbContext.Routes.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == routeId);

        private Task<bool> OwnsRouteStopAsync(Guid companyId, Guid routeStopId) =>
            _dbContext.RouteStops.AnyAsync(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == routeStopId);

        private Task<bool> OwnsRoutePlanAsync(Guid companyId, Guid routePlanId) =>
            _dbContext.RoutePlans.AnyAsync(x => !x.IsDeleted && x.Route.CompanyId == companyId && x.Id == routePlanId);

        private Task<bool> OwnsRoutePlanStopAsync(Guid companyId, Guid routePlanStopId) =>
            _dbContext.RoutePlanStops.AnyAsync(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId && x.Id == routePlanStopId);

        private async Task PopulateRouteStopOptionsAsync(Guid companyId, RouteStopCreateViewModel model)
        {
            model.RouteOptions = await GetRouteOptionsAsync(companyId);
            model.LocationOptions = await GetLocationOptionsAsync();
        }

        private async Task PopulateRouteStopOptionsAsync(Guid companyId, RouteStopEditViewModel model)
        {
            model.RouteOptions = await GetRouteOptionsAsync(companyId);
            model.LocationOptions = await GetLocationOptionsAsync();
        }

        private async Task PopulateRoutePlanOptionsAsync(Guid companyId, RoutePlanCreateViewModel model)
        {
            model.RouteOptions = await GetRouteOptionsAsync(companyId);
        }

        private async Task PopulateRoutePlanOptionsAsync(Guid companyId, RoutePlanEditViewModel model)
        {
            model.RouteOptions = await GetRouteOptionsAsync(companyId);
        }

        private async Task PopulateRoutePlanStopOptionsAsync(Guid companyId, RoutePlanStopCreateViewModel model)
        {
            model.RoutePlanOptions = await GetRoutePlanOptionsAsync(companyId);
            model.RouteStopOptions = await GetRouteStopOptionsAsync(companyId);
            model.LocationOptions = await GetLocationOptionsAsync();
        }

        private async Task PopulateRoutePlanStopOptionsAsync(Guid companyId, RoutePlanStopEditViewModel model)
        {
            model.RoutePlanOptions = await GetRoutePlanOptionsAsync(companyId);
            model.RouteStopOptions = await GetRouteStopOptionsAsync(companyId);
            model.LocationOptions = await GetLocationOptionsAsync();
        }

        private async Task PopulateRoutePlanShipmentOptionsAsync(Guid companyId, RoutePlanShipmentCreateViewModel model)
        {
            model.RoutePlanOptions = await GetRoutePlanOptionsAsync(companyId);
            model.ShipmentOptions = await GetShipmentOptionsAsync(companyId);
            model.RoutePlanStopOptions = await GetRoutePlanStopOptionsAsync(companyId);
        }

        private async Task PopulateRoutePlanShipmentOptionsAsync(Guid companyId, RoutePlanShipmentEditViewModel model)
        {
            model.RoutePlanOptions = await GetRoutePlanOptionsAsync(companyId);
            model.ShipmentOptions = await GetShipmentOptionsAsync(companyId);
            model.RoutePlanStopOptions = await GetRoutePlanStopOptionsAsync(companyId);
        }

        private async Task<IEnumerable<SelectListItem>> GetRouteOptionsAsync(Guid companyId) =>
            await _dbContext.Routes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.RouteCode)
                .Select(x => new SelectListItem(x.RouteCode + " / " + x.Name, x.Id.ToString()))
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetRoutePlanOptionsAsync(Guid companyId) =>
            await _dbContext.RoutePlans
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Route.CompanyId == companyId)
                .OrderByDescending(x => x.PlanDateUtc)
                .Select(x => new SelectListItem(
                    x.Route.RouteCode + " / " + x.PlanDateUtc.ToString("yyyy-MM-dd") + (x.PlanReference != null ? " / " + x.PlanReference : ""),
                    x.Id.ToString()))
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetRouteStopOptionsAsync(Guid companyId) =>
            await _dbContext.RouteStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Route.CompanyId == companyId)
                .OrderBy(x => x.Route.RouteCode)
                .ThenBy(x => x.SequenceNo)
                .Select(x => new SelectListItem(
                    x.Route.RouteCode + " / #" + x.SequenceNo + " / " + x.Location.Name,
                    x.Id.ToString()))
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetRoutePlanStopOptionsAsync(Guid companyId) =>
            await _dbContext.RoutePlanStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.RoutePlan.Route.CompanyId == companyId)
                .OrderByDescending(x => x.RoutePlan.PlanDateUtc)
                .ThenBy(x => x.SequenceNo)
                .Select(x => new SelectListItem(
                    x.RoutePlan.Route.RouteCode + " / " + x.RoutePlan.PlanDateUtc.ToString("yyyy-MM-dd") + " / #" + x.SequenceNo + " / " + x.Location.Name,
                    x.Id.ToString()))
                .ToListAsync();
    }
}