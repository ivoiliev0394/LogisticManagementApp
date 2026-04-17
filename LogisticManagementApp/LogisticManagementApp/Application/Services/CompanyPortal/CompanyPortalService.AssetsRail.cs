using LogisticManagementApp.Domain.Assets.Rail;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Models.CompanyPortal.Assets.Rail;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyRailHomeViewModel> GetRailAssetsHomeAsync() =>
            Task.FromResult(
                new CompanyRailHomeViewModel
                {
                    Cards = new[]
                    {
                        new RailAssetCardViewModel
                        {
                            Title = "Trains",
                            Description = "Влакове на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Trains),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateTrain)
                        },
                        new RailAssetCardViewModel
                        {
                            Title = "Rail Cars",
                            Description = "ЖП вагони на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RailCars),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRailCar)
                        },
                        new RailAssetCardViewModel
                        {
                            Title = "Rail Services",
                            Description = "ЖП услуги и маршрути.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RailServices),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRailService)
                        },
                        new RailAssetCardViewModel
                        {
                            Title = "Rail Movements",
                            Description = "ЖП движения.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RailMovements),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateRailMovement)
                        }
                    }
                });

        public async Task<IEnumerable<TrainListItemViewModel>> GetTrainsAsync(Guid companyId) =>
            await _dbContext.Trains
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.TrainNumber)
                .Select(x => new TrainListItemViewModel
                {
                    Id = x.Id,
                    TrainNumber = x.TrainNumber,
                    TrainType = x.TrainType.ToString(),
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();

        public Task<TrainCreateViewModel> GetCreateTrainModelAsync(Guid companyId) =>
            Task.FromResult(new TrainCreateViewModel());

        public async Task<Guid?> CreateTrainAsync(Guid companyId, TrainCreateViewModel model)
        {
            var entity = new Train
            {
                CompanyId = companyId,
                TrainNumber = model.TrainNumber.Trim(),
                TrainType = model.TrainType,
                MaxWeightKg = model.MaxWeightKg,
                MaxVolumeCbm = model.MaxVolumeCbm,
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Trains.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TrainEditViewModel?> GetTrainForEditAsync(Guid companyId, Guid id) =>
            await _dbContext.Trains
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new TrainEditViewModel
                {
                    Id = x.Id,
                    TrainNumber = x.TrainNumber,
                    TrainType = x.TrainType,
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

        public async Task<bool> UpdateTrainAsync(Guid companyId, TrainEditViewModel model)
        {
            var entity = await _dbContext.Trains
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.TrainNumber = model.TrainNumber.Trim();
            entity.TrainType = model.TrainType;
            entity.MaxWeightKg = model.MaxWeightKg;
            entity.MaxVolumeCbm = model.MaxVolumeCbm;
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTrainAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Trains
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RailCarListItemViewModel>> GetRailCarsAsync(Guid companyId) =>
            await _dbContext.RailCars
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.RailCarNumber)
                .Select(x => new RailCarListItemViewModel
                {
                    Id = x.Id,
                    RailCarNumber = x.RailCarNumber,
                    RailCarType = x.RailCarType.ToString(),
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();

        public Task<RailCarCreateViewModel> GetCreateRailCarModelAsync(Guid companyId) =>
            Task.FromResult(new RailCarCreateViewModel());

        public async Task<Guid?> CreateRailCarAsync(Guid companyId, RailCarCreateViewModel model)
        {
            var entity = new RailCar
            {
                CompanyId = companyId,
                RailCarNumber = model.RailCarNumber.Trim(),
                RailCarType = model.RailCarType,
                MaxWeightKg = model.MaxWeightKg,
                MaxVolumeCbm = model.MaxVolumeCbm,
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RailCars.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RailCarEditViewModel?> GetRailCarForEditAsync(Guid companyId, Guid id) =>
            await _dbContext.RailCars
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new RailCarEditViewModel
                {
                    Id = x.Id,
                    RailCarNumber = x.RailCarNumber,
                    RailCarType = x.RailCarType,
                    MaxWeightKg = x.MaxWeightKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

        public async Task<bool> UpdateRailCarAsync(Guid companyId, RailCarEditViewModel model)
        {
            var entity = await _dbContext.RailCars
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.RailCarNumber = model.RailCarNumber.Trim();
            entity.RailCarType = model.RailCarType;
            entity.MaxWeightKg = model.MaxWeightKg;
            entity.MaxVolumeCbm = model.MaxVolumeCbm;
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRailCarAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RailCars
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RailServiceListItemViewModel>> GetRailServicesAsync(Guid companyId) =>
            await _dbContext.RailServices
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ServiceCode)
                .Select(x => new RailServiceListItemViewModel
                {
                    Id = x.Id,
                    ServiceCode = x.ServiceCode,
                    Name = x.Name,
                    OriginLocation = x.OriginLocation != null ? x.OriginLocation.Name : null,
                    DestinationLocation = x.DestinationLocation != null ? x.DestinationLocation.Name : null,
                    EstimatedTransitDays = x.EstimatedTransitDays,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<RailServiceCreateViewModel> GetCreateRailServiceModelAsync(Guid companyId)
        {
            var model = new RailServiceCreateViewModel();
            await PopulateRailServiceOptionsAsync(model);
            return model;
        }

        public async Task<Guid?> CreateRailServiceAsync(Guid companyId, RailServiceCreateViewModel model)
        {
            if (model.OriginLocationId.HasValue && !await ExistsLocationAsync(model.OriginLocationId.Value))
            {
                return null;
            }

            if (model.DestinationLocationId.HasValue && !await ExistsLocationAsync(model.DestinationLocationId.Value))
            {
                return null;
            }

            var entity = new RailService
            {
                ServiceCode = model.ServiceCode.Trim(),
                Name = model.Name.Trim(),
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                TransportMode = TransportMode.Rail,
                EstimatedTransitDays = model.EstimatedTransitDays,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RailServices.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RailServiceEditViewModel?> GetRailServiceForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.RailServices
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id)
                .Select(x => new RailServiceEditViewModel
                {
                    Id = x.Id,
                    ServiceCode = x.ServiceCode,
                    Name = x.Name,
                    OriginLocationId = x.OriginLocationId,
                    DestinationLocationId = x.DestinationLocationId,
                    EstimatedTransitDays = x.EstimatedTransitDays,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateRailServiceOptionsAsync(model);
            }

            return model;
        }

        public async Task<bool> UpdateRailServiceAsync(Guid companyId, RailServiceEditViewModel model)
        {
            var entity = await _dbContext.RailServices
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (model.OriginLocationId.HasValue && !await ExistsLocationAsync(model.OriginLocationId.Value))
            {
                return false;
            }

            if (model.DestinationLocationId.HasValue && !await ExistsLocationAsync(model.DestinationLocationId.Value))
            {
                return false;
            }

            entity.ServiceCode = model.ServiceCode.Trim();
            entity.Name = model.Name.Trim();
            entity.OriginLocationId = model.OriginLocationId;
            entity.DestinationLocationId = model.DestinationLocationId;
            entity.EstimatedTransitDays = model.EstimatedTransitDays;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRailServiceAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RailServices
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RailMovementListItemViewModel>> GetRailMovementsAsync(Guid companyId) =>
            await _dbContext.RailMovements
                .AsNoTracking()
                .Where(x => !x.IsDeleted && (!x.TrainId.HasValue || x.Train!.CompanyId == companyId))
                .OrderByDescending(x => x.ScheduledDepartureUtc)
                .ThenBy(x => x.MovementNo)
                .Select(x => new RailMovementListItemViewModel
                {
                    Id = x.Id,
                    MovementNo = x.MovementNo,
                    TrainNumber = x.Train != null ? x.Train.TrainNumber : null,
                    RailServiceDisplay = x.RailService != null ? x.RailService.ServiceCode + " / " + x.RailService.Name : null,
                    OriginLocation = x.OriginLocation != null ? x.OriginLocation.Name : null,
                    DestinationLocation = x.DestinationLocation != null ? x.DestinationLocation.Name : null,
                    Status = x.Status.ToString(),
                    ScheduledDepartureUtc = x.ScheduledDepartureUtc,
                    ScheduledArrivalUtc = x.ScheduledArrivalUtc,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<RailMovementCreateViewModel> GetCreateRailMovementModelAsync(Guid companyId)
        {
            var model = new RailMovementCreateViewModel();
            await PopulateRailMovementOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateRailMovementAsync(Guid companyId, RailMovementCreateViewModel model)
        {
            if (model.TrainId.HasValue && !await OwnsTrainAsync(companyId, model.TrainId.Value))
            {
                return null;
            }

            if (model.RailServiceId.HasValue && !await ExistsRailServiceAsync(model.RailServiceId.Value))
            {
                return null;
            }

            if (model.OriginLocationId.HasValue && !await ExistsLocationAsync(model.OriginLocationId.Value))
            {
                return null;
            }

            if (model.DestinationLocationId.HasValue && !await ExistsLocationAsync(model.DestinationLocationId.Value))
            {
                return null;
            }

            var entity = new RailMovement
            {
                TrainId = model.TrainId,
                RailServiceId = model.RailServiceId,
                MovementNo = model.MovementNo.Trim(),
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                Status = model.Status,
                ScheduledDepartureUtc = model.ScheduledDepartureUtc,
                ScheduledArrivalUtc = model.ScheduledArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.RailMovements.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<RailMovementEditViewModel?> GetRailMovementForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.RailMovements
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id && (!x.TrainId.HasValue || x.Train!.CompanyId == companyId))
                .Select(x => new RailMovementEditViewModel
                {
                    Id = x.Id,
                    TrainId = x.TrainId,
                    RailServiceId = x.RailServiceId,
                    MovementNo = x.MovementNo,
                    OriginLocationId = x.OriginLocationId,
                    DestinationLocationId = x.DestinationLocationId,
                    Status = x.Status,
                    ScheduledDepartureUtc = x.ScheduledDepartureUtc,
                    ScheduledArrivalUtc = x.ScheduledArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateRailMovementOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateRailMovementAsync(Guid companyId, RailMovementEditViewModel model)
        {
            var entity = await _dbContext.RailMovements
                .Include(x => x.Train)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (model.TrainId.HasValue && !await OwnsTrainAsync(companyId, model.TrainId.Value))
            {
                return false;
            }

            if (model.RailServiceId.HasValue && !await ExistsRailServiceAsync(model.RailServiceId.Value))
            {
                return false;
            }

            if (model.OriginLocationId.HasValue && !await ExistsLocationAsync(model.OriginLocationId.Value))
            {
                return false;
            }

            if (model.DestinationLocationId.HasValue && !await ExistsLocationAsync(model.DestinationLocationId.Value))
            {
                return false;
            }

            entity.TrainId = model.TrainId;
            entity.RailServiceId = model.RailServiceId;
            entity.MovementNo = model.MovementNo.Trim();
            entity.OriginLocationId = model.OriginLocationId;
            entity.DestinationLocationId = model.DestinationLocationId;
            entity.Status = model.Status;
            entity.ScheduledDepartureUtc = model.ScheduledDepartureUtc;
            entity.ScheduledArrivalUtc = model.ScheduledArrivalUtc;
            entity.ActualDepartureUtc = model.ActualDepartureUtc;
            entity.ActualArrivalUtc = model.ActualArrivalUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRailMovementAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.RailMovements
                .Include(x => x.Train)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null || (entity.TrainId.HasValue && entity.Train?.CompanyId != companyId))
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private Task<bool> OwnsTrainAsync(Guid companyId, Guid trainId) =>
            _dbContext.Trains.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == trainId);

        private Task<bool> ExistsRailServiceAsync(Guid railServiceId) =>
            _dbContext.RailServices.AnyAsync(x => !x.IsDeleted && x.Id == railServiceId);

        private async Task PopulateRailServiceOptionsAsync(RailServiceCreateViewModel model) =>
            model.LocationOptions = await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();

        private async Task PopulateRailServiceOptionsAsync(RailServiceEditViewModel model) =>
            model.LocationOptions = await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();

        private async Task PopulateRailMovementOptionsAsync(Guid companyId, RailMovementCreateViewModel model)
        {
            model.TrainOptions = await _dbContext.Trains
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.TrainNumber)
                .Select(x => new SelectListItem(x.TrainNumber, x.Id.ToString()))
                .ToListAsync();

            model.RailServiceOptions = await _dbContext.RailServices
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ServiceCode)
                .Select(x => new SelectListItem(x.ServiceCode + " / " + x.Name, x.Id.ToString()))
                .ToListAsync();

            model.LocationOptions = await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();
        }

        private async Task PopulateRailMovementOptionsAsync(Guid companyId, RailMovementEditViewModel model)
        {
            model.TrainOptions = await _dbContext.Trains
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.TrainNumber)
                .Select(x => new SelectListItem(x.TrainNumber, x.Id.ToString()))
                .ToListAsync();

            model.RailServiceOptions = await _dbContext.RailServices
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ServiceCode)
                .Select(x => new SelectListItem(x.ServiceCode + " / " + x.Name, x.Id.ToString()))
                .ToListAsync();

            model.LocationOptions = await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToListAsync();
        }
    }
}