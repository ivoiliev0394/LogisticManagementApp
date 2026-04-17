using LogisticManagementApp.Domain.Assets.Road;
using LogisticManagementApp.Models.CompanyPortal.Assets.Road;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyRoadHomeViewModel> GetRoadAssetsHomeAsync()
        {
            return Task.FromResult(
                new CompanyRoadHomeViewModel
                {
                    Cards = new[]
                    {
                        new RoadAssetCardViewModel
                        {
                            Title = "Vehicles",
                            Description = "Превозни средства на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Vehicles),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateVehicle)
                        },
                        new RoadAssetCardViewModel
                        {
                            Title = "Drivers",
                            Description = "Шофьори и лицензи.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Drivers),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateDriver)
                        },
                        new RoadAssetCardViewModel
                        {
                            Title = "Trips",
                            Description = "Планирани и активни road trips.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Trips),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateTrip)
                        },
                        new RoadAssetCardViewModel
                        {
                            Title = "Trip Stops",
                            Description = "Спирки по road маршрутите.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.TripStops),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateTripStop)
                        },
                        new RoadAssetCardViewModel
                        {
                            Title = "Trip Shipments",
                            Description = "Пратки, разпределени към trips.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.TripShipments),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateTripShipment)
                        }
                    }
                });
        }

        public async Task<IEnumerable<VehicleListItemViewModel>> GetVehiclesAsync(Guid companyId)
        {
            return await _dbContext.Vehicles
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.RegistrationNumber)
                .Select(x => new VehicleListItemViewModel
                {
                    Id = x.Id,
                    RegistrationNumber = x.RegistrationNumber,
                    VehicleType = x.VehicleType.ToString(),
                    Brand = x.Brand,
                    Model = x.Model,
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<VehicleCreateViewModel> GetCreateVehicleModelAsync(Guid companyId) =>
            Task.FromResult(new VehicleCreateViewModel());

        public async Task<Guid?> CreateVehicleAsync(Guid companyId, VehicleCreateViewModel model)
        {
            var entity = new Vehicle
            {
                CompanyId = companyId,
                RegistrationNumber = model.RegistrationNumber.Trim(),
                VehicleType = model.VehicleType,
                Brand = TrimOrNull(model.Brand),
                Model = TrimOrNull(model.Model),
                MaxWeightKg = model.MaxWeightKg,
                MaxVolumeCbm = model.MaxVolumeCbm,
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Vehicles.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<VehicleEditViewModel?> GetVehicleForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.Vehicles
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new VehicleEditViewModel
                {
                    Id = x.Id,
                    RegistrationNumber = x.RegistrationNumber,
                    VehicleType = x.VehicleType,
                    Brand = x.Brand,
                    Model = x.Model,
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateVehicleAsync(Guid companyId, VehicleEditViewModel model)
        {
            var entity = await _dbContext.Vehicles
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.RegistrationNumber = model.RegistrationNumber.Trim();
            entity.VehicleType = model.VehicleType;
            entity.Brand = TrimOrNull(model.Brand);
            entity.Model = TrimOrNull(model.Model);
            entity.MaxWeightKg = model.MaxWeightKg;
            entity.MaxVolumeCbm = model.MaxVolumeCbm;
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVehicleAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Vehicles
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DriverListItemViewModel>> GetDriversAsync(Guid companyId)
        {
            return await _dbContext.Drivers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.FullName)
                .Select(x => new DriverListItemViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    LicenseCategory = x.LicenseCategory.ToString(),
                    LicenseNumber = x.LicenseNumber,
                    Phone = x.Phone,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<DriverCreateViewModel> GetCreateDriverModelAsync(Guid companyId) =>
            Task.FromResult(new DriverCreateViewModel());

        public async Task<Guid?> CreateDriverAsync(Guid companyId, DriverCreateViewModel model)
        {
            var entity = new Driver
            {
                CompanyId = companyId,
                FullName = model.FullName.Trim(),
                LicenseCategory = model.LicenseCategory,
                LicenseNumber = TrimOrNull(model.LicenseNumber),
                Phone = TrimOrNull(model.Phone),
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Drivers.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<DriverEditViewModel?> GetDriverForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.Drivers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new DriverEditViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    LicenseCategory = x.LicenseCategory,
                    LicenseNumber = x.LicenseNumber,
                    Phone = x.Phone,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateDriverAsync(Guid companyId, DriverEditViewModel model)
        {
            var entity = await _dbContext.Drivers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.FullName = model.FullName.Trim();
            entity.LicenseCategory = model.LicenseCategory;
            entity.LicenseNumber = TrimOrNull(model.LicenseNumber);
            entity.Phone = TrimOrNull(model.Phone);
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDriverAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Drivers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TripListItemViewModel>> GetTripsAsync(Guid companyId)
        {
            return await _dbContext.Trips
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                     (x.DriverId != null && x.Driver!.CompanyId == companyId)))
                .OrderByDescending(x => x.PlannedDepartureUtc)
                .ThenBy(x => x.TripNo)
                .Select(x => new TripListItemViewModel
                {
                    Id = x.Id,
                    TripNo = x.TripNo,
                    VehicleDisplay = x.Vehicle != null ? x.Vehicle.RegistrationNumber : null,
                    DriverName = x.Driver != null ? x.Driver.FullName : null,
                    OriginLocation = x.OriginLocation != null ? x.OriginLocation.Name : null,
                    DestinationLocation = x.DestinationLocation != null ? x.DestinationLocation.Name : null,
                    Status = x.Status.ToString(),
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<TripCreateViewModel> GetCreateTripModelAsync(Guid companyId)
        {
            var model = new TripCreateViewModel();
            await PopulateTripOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateTripAsync(Guid companyId, TripCreateViewModel model)
        {
            if (model.VehicleId.HasValue && !await OwnsVehicleAsync(companyId, model.VehicleId.Value))
            {
                return null;
            }

            if (model.DriverId.HasValue && !await OwnsDriverAsync(companyId, model.DriverId.Value))
            {
                return null;
            }

            var entity = new Trip
            {
                TripNo = model.TripNo.Trim(),
                VehicleId = model.VehicleId,
                DriverId = model.DriverId,
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                Status = model.Status,
                PlannedDepartureUtc = model.PlannedDepartureUtc,
                PlannedArrivalUtc = model.PlannedArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Trips.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TripEditViewModel?> GetTripForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.Trips
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                     (x.DriverId != null && x.Driver!.CompanyId == companyId)))
                .Select(x => new TripEditViewModel
                {
                    Id = x.Id,
                    TripNo = x.TripNo,
                    VehicleId = x.VehicleId,
                    DriverId = x.DriverId,
                    OriginLocationId = x.OriginLocationId,
                    DestinationLocationId = x.DestinationLocationId,
                    Status = x.Status,
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateTripOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateTripAsync(Guid companyId, TripEditViewModel model)
        {
            var entity = await _dbContext.Trips
                .Include(x => x.Vehicle)
                .Include(x => x.Driver)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                     (x.DriverId != null && x.Driver!.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            if (model.VehicleId.HasValue && !await OwnsVehicleAsync(companyId, model.VehicleId.Value))
            {
                return false;
            }

            if (model.DriverId.HasValue && !await OwnsDriverAsync(companyId, model.DriverId.Value))
            {
                return false;
            }

            entity.TripNo = model.TripNo.Trim();
            entity.VehicleId = model.VehicleId;
            entity.DriverId = model.DriverId;
            entity.OriginLocationId = model.OriginLocationId;
            entity.DestinationLocationId = model.DestinationLocationId;
            entity.Status = model.Status;
            entity.PlannedDepartureUtc = model.PlannedDepartureUtc;
            entity.PlannedArrivalUtc = model.PlannedArrivalUtc;
            entity.ActualDepartureUtc = model.ActualDepartureUtc;
            entity.ActualArrivalUtc = model.ActualArrivalUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTripAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Trips
                .Include(x => x.Vehicle)
                .Include(x => x.Driver)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                     (x.DriverId != null && x.Driver!.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TripStopListItemViewModel>> GetTripStopsAsync(Guid companyId)
        {
            return await _dbContext.TripStops
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)))
                .OrderBy(x => x.Trip.TripNo)
                .ThenBy(x => x.SequenceNumber)
                .Select(x => new TripStopListItemViewModel
                {
                    Id = x.Id,
                    TripId = x.TripId,
                    TripNo = x.Trip.TripNo,
                    LocationName = x.Location.Name,
                    SequenceNumber = x.SequenceNumber,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<TripStopCreateViewModel> GetCreateTripStopModelAsync(Guid companyId)
        {
            var model = new TripStopCreateViewModel();
            await PopulateTripStopOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateTripStopAsync(Guid companyId, TripStopCreateViewModel model)
        {
            if (!await OwnsTripAsync(companyId, model.TripId))
            {
                return null;
            }

            var entity = new TripStop
            {
                TripId = model.TripId,
                LocationId = model.LocationId,
                SequenceNumber = model.SequenceNumber,
                PlannedArrivalUtc = model.PlannedArrivalUtc,
                PlannedDepartureUtc = model.PlannedDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.TripStops.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TripStopEditViewModel?> GetTripStopForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.TripStops
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)))
                .Select(x => new TripStopEditViewModel
                {
                    Id = x.Id,
                    TripId = x.TripId,
                    LocationId = x.LocationId,
                    SequenceNumber = x.SequenceNumber,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateTripStopOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateTripStopAsync(Guid companyId, TripStopEditViewModel model)
        {
            var entity = await _dbContext.TripStops
                .Include(x => x.Trip)
                .ThenInclude(x => x.Vehicle)
                .Include(x => x.Trip)
                .ThenInclude(x => x.Driver)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)));

            if (entity == null || !await OwnsTripAsync(companyId, model.TripId))
            {
                return false;
            }

            entity.TripId = model.TripId;
            entity.LocationId = model.LocationId;
            entity.SequenceNumber = model.SequenceNumber;
            entity.PlannedArrivalUtc = model.PlannedArrivalUtc;
            entity.PlannedDepartureUtc = model.PlannedDepartureUtc;
            entity.ActualArrivalUtc = model.ActualArrivalUtc;
            entity.ActualDepartureUtc = model.ActualDepartureUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTripStopAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.TripStops
                .Include(x => x.Trip)
                .ThenInclude(x => x.Vehicle)
                .Include(x => x.Trip)
                .ThenInclude(x => x.Driver)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TripShipmentListItemViewModel>> GetTripShipmentsAsync(Guid companyId)
        {
            return await _dbContext.TripShipments
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Shipment.CustomerCompanyId == companyId &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)))
                .OrderBy(x => x.Trip.TripNo)
                .ThenBy(x => x.Priority)
                .Select(x => new TripShipmentListItemViewModel
                {
                    Id = x.Id,
                    TripId = x.TripId,
                    TripNo = x.Trip.TripNo,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    ShipmentLegDisplay = x.ShipmentLeg != null
                        ? "Leg " + x.ShipmentLeg.LegNo + " / " + x.ShipmentLeg.Mode
                        : null,
                    PickupStopDisplay = x.PickupTripStop != null
                        ? x.PickupTripStop.SequenceNumber + " / " + x.PickupTripStop.Location.Name
                        : null,
                    DeliveryStopDisplay = x.DeliveryTripStop != null
                        ? x.DeliveryTripStop.SequenceNumber + " / " + x.DeliveryTripStop.Location.Name
                        : null,
                    Priority = x.Priority,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<TripShipmentCreateViewModel> GetCreateTripShipmentModelAsync(Guid companyId)
        {
            var model = new TripShipmentCreateViewModel();
            await PopulateTripShipmentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateTripShipmentAsync(Guid companyId, TripShipmentCreateViewModel model)
        {
            if (!await OwnsTripAsync(companyId, model.TripId) || !await OwnsShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue && !await OwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return null;
            }

            if (model.PickupTripStopId.HasValue && !await OwnsTripStopAsync(companyId, model.PickupTripStopId.Value))
            {
                return null;
            }

            if (model.DeliveryTripStopId.HasValue && !await OwnsTripStopAsync(companyId, model.DeliveryTripStopId.Value))
            {
                return null;
            }

            var entity = new TripShipment
            {
                TripId = model.TripId,
                ShipmentId = model.ShipmentId,
                ShipmentLegId = model.ShipmentLegId,
                PickupTripStopId = model.PickupTripStopId,
                DeliveryTripStopId = model.DeliveryTripStopId,
                Priority = model.Priority,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.TripShipments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TripShipmentEditViewModel?> GetTripShipmentForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.TripShipments
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    x.Shipment.CustomerCompanyId == companyId &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)))
                .Select(x => new TripShipmentEditViewModel
                {
                    Id = x.Id,
                    TripId = x.TripId,
                    ShipmentId = x.ShipmentId,
                    ShipmentLegId = x.ShipmentLegId,
                    PickupTripStopId = x.PickupTripStopId,
                    DeliveryTripStopId = x.DeliveryTripStopId,
                    Priority = x.Priority,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateTripShipmentOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateTripShipmentAsync(Guid companyId, TripShipmentEditViewModel model)
        {
            var entity = await _dbContext.TripShipments
                .Include(x => x.Trip)
                .ThenInclude(x => x.Vehicle)
                .Include(x => x.Trip)
                .ThenInclude(x => x.Driver)
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    x.Shipment.CustomerCompanyId == companyId &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            if (!await OwnsTripAsync(companyId, model.TripId) || !await OwnsShipmentAsync(companyId, model.ShipmentId))
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue && !await OwnsShipmentLegAsync(companyId, model.ShipmentLegId.Value))
            {
                return false;
            }

            if (model.PickupTripStopId.HasValue && !await OwnsTripStopAsync(companyId, model.PickupTripStopId.Value))
            {
                return false;
            }

            if (model.DeliveryTripStopId.HasValue && !await OwnsTripStopAsync(companyId, model.DeliveryTripStopId.Value))
            {
                return false;
            }

            entity.TripId = model.TripId;
            entity.ShipmentId = model.ShipmentId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.PickupTripStopId = model.PickupTripStopId;
            entity.DeliveryTripStopId = model.DeliveryTripStopId;
            entity.Priority = model.Priority;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTripShipmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.TripShipments
                .Include(x => x.Trip)
                .ThenInclude(x => x.Vehicle)
                .Include(x => x.Trip)
                .ThenInclude(x => x.Driver)
                .Include(x => x.Shipment)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    x.Shipment.CustomerCompanyId == companyId &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)));

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private Task<bool> OwnsVehicleAsync(Guid companyId, Guid vehicleId) =>
            _dbContext.Vehicles.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == vehicleId);

        private Task<bool> OwnsDriverAsync(Guid companyId, Guid driverId) =>
            _dbContext.Drivers.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == driverId);

        private Task<bool> OwnsTripAsync(Guid companyId, Guid tripId) =>
            _dbContext.Trips.AnyAsync(x =>
                !x.IsDeleted &&
                x.Id == tripId &&
                ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                 (x.DriverId != null && x.Driver!.CompanyId == companyId)));

        private Task<bool> OwnsTripStopAsync(Guid companyId, Guid tripStopId) =>
            _dbContext.TripStops.AnyAsync(x =>
                !x.IsDeleted &&
                x.Id == tripStopId &&
                ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                 (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)));

        private Task<bool> OwnsShipmentAsync(Guid companyId, Guid shipmentId) =>
            _dbContext.Shipments.AnyAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == shipmentId);

        private Task<bool> OwnsShipmentLegAsync(Guid companyId, Guid shipmentLegId) =>
            _dbContext.ShipmentLegs.AnyAsync(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId && x.Id == shipmentLegId);

        private async Task PopulateTripOptionsAsync(Guid companyId, TripCreateViewModel model)
        {
            model.VehicleOptions = await GetVehicleOptionsAsync(companyId);
            model.DriverOptions = await GetDriverOptionsAsync(companyId);
            model.LocationOptions = await GetRoadLocationOptionsAsync();
        }

        private async Task PopulateTripOptionsAsync(Guid companyId, TripEditViewModel model)
        {
            model.VehicleOptions = await GetVehicleOptionsAsync(companyId);
            model.DriverOptions = await GetDriverOptionsAsync(companyId);
            model.LocationOptions = await GetRoadLocationOptionsAsync();
        }

        private async Task PopulateTripStopOptionsAsync(Guid companyId, TripStopCreateViewModel model)
        {
            model.TripOptions = await GetTripOptionsAsync(companyId);
            model.LocationOptions = await GetRoadLocationOptionsAsync();
        }

        private async Task PopulateTripStopOptionsAsync(Guid companyId, TripStopEditViewModel model)
        {
            model.TripOptions = await GetTripOptionsAsync(companyId);
            model.LocationOptions = await GetRoadLocationOptionsAsync();
        }

        private async Task PopulateTripShipmentOptionsAsync(Guid companyId, TripShipmentCreateViewModel model)
        {
            model.TripOptions = await GetTripOptionsAsync(companyId);
            model.ShipmentOptions = await GetShipmentOptionsAsync(companyId);
            model.ShipmentLegOptions = await GetShipmentLegOptionsAsync(companyId);
            model.TripStopOptions = await GetTripStopOptionsAsync(companyId);
        }

        private async Task PopulateTripShipmentOptionsAsync(Guid companyId, TripShipmentEditViewModel model)
        {
            model.TripOptions = await GetTripOptionsAsync(companyId);
            model.ShipmentOptions = await GetShipmentOptionsAsync(companyId);
            model.ShipmentLegOptions = await GetShipmentLegOptionsAsync(companyId);
            model.TripStopOptions = await GetTripStopOptionsAsync(companyId);
        }

        private async Task<IEnumerable<SelectListItem>> GetVehicleOptionsAsync(Guid companyId) =>
            await _dbContext.Vehicles
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.RegistrationNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RegistrationNumber + " / " + x.VehicleType
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetDriverOptionsAsync(Guid companyId) =>
            await _dbContext.Drivers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName + " / " + x.LicenseCategory
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetTripOptionsAsync(Guid companyId) =>
            await _dbContext.Trips
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.VehicleId != null && x.Vehicle!.CompanyId == companyId) ||
                     (x.DriverId != null && x.Driver!.CompanyId == companyId)))
                .OrderBy(x => x.TripNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TripNo
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetRoadLocationOptionsAsync() =>
            await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name + " (" + x.Code + ")"
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetShipmentOptionsAsync(Guid companyId) =>
            await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShipmentNo
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetShipmentLegOptionsAsync(Guid companyId) =>
            await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderBy(x => x.Shipment.ShipmentNo)
                .ThenBy(x => x.LegNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Shipment.ShipmentNo + " / Leg " + x.LegNo + " / " + x.Mode
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetTripStopOptionsAsync(Guid companyId) =>
            await _dbContext.TripStops
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    ((x.Trip.VehicleId != null && x.Trip.Vehicle!.CompanyId == companyId) ||
                     (x.Trip.DriverId != null && x.Trip.Driver!.CompanyId == companyId)))
                .OrderBy(x => x.Trip.TripNo)
                .ThenBy(x => x.SequenceNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Trip.TripNo + " / #" + x.SequenceNumber + " / " + x.Location.Name
                })
                .ToListAsync();
    }
}