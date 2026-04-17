using LogisticManagementApp.Domain.Assets.Sea;
using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Models.CompanyPortal.Assets.Sea;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyAssetsHomeViewModel> GetAssetsHomeAsync()
        {
            return Task.FromResult(
                new CompanyAssetsHomeViewModel
                {
                    Cards = new[]
                    {
                        new AssetsHomeCardViewModel
                        {
                            Title = "Sea",
                            Description = "Кораби, позиции, рейсове, спирки и екипаж.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.SeaAssets)
                        },
                        new AssetsHomeCardViewModel
                        {
                            Title = "Road",
                            Description = "Пътни активи и операции.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RoadAssets)
                        },
                        new AssetsHomeCardViewModel
                        {
                            Title = "Air",
                            Description = "Въздушни активи и полети.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.AirAssets)
                        },
                        new AssetsHomeCardViewModel
                        {
                            Title = "Rail",
                            Description = "ЖП активи и движения.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.RailAssets)
                        },
                        new AssetsHomeCardViewModel
                        {
                            Title = "Cargo Units",
                            Description = "Контейнери и cargo units.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CargoUnitAssets)
                        }
                    }
                });
        }

        public Task<CompanySeaHomeViewModel> GetSeaAssetsHomeAsync()
        {
            return Task.FromResult(
                new CompanySeaHomeViewModel
                {
                    Cards = new[]
                    {
                        new SeaAssetCardViewModel
                        {
                            Title = "Vessels",
                            Description = "Флотът на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Vessels),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateVessel)
                        },
                        new SeaAssetCardViewModel
                        {
                            Title = "Vessel Positions",
                            Description = "Последни и исторически позиции.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.VesselPositions),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateVesselPosition)
                        },
                        new SeaAssetCardViewModel
                        {
                            Title = "Voyages",
                            Description = "Планирани и активни voyages.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Voyages),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateVoyage)
                        },
                        new SeaAssetCardViewModel
                        {
                            Title = "Voyage Stops",
                            Description = "Спирки по voyage маршрути.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.VoyageStops),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateVoyageStop)
                        },
                        new SeaAssetCardViewModel
                        {
                            Title = "Vessel Crew Members",
                            Description = "Морски екипажи на фирмата.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.VesselCrewMembers),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateVesselCrewMember)
                        },
                        new SeaAssetCardViewModel
                        {
                            Title = "Crew Assignments",
                            Description = "Разпределения на crew към voyage.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CrewAssignments),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateCrewAssignment)
                        }
                    }
                });
        }

        public async Task<IEnumerable<VesselListItemViewModel>> GetVesselsAsync(Guid companyId)
        {
            return await _dbContext.Vessels
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.Name)
                .Select(x => new VesselListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImoNumber = x.ImoNumber,
                    MmsiNumber = x.MmsiNumber,
                    VesselType = x.VesselType.ToString(),
                    CapacityTeu = x.CapacityTeu,
                    DeadweightTons = x.DeadweightTons,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<VesselCreateViewModel> GetCreateVesselModelAsync(Guid companyId) =>
            Task.FromResult(new VesselCreateViewModel());

        public async Task<Guid?> CreateVesselAsync(Guid companyId, VesselCreateViewModel model)
        {
            var entity = new Vessel
            {
                CompanyId = companyId,
                Name = model.Name.Trim(),
                ImoNumber = TrimOrNull(model.ImoNumber),
                MmsiNumber = TrimOrNull(model.MmsiNumber),
                VesselType = model.VesselType,
                CapacityTeu = model.CapacityTeu,
                DeadweightTons = model.DeadweightTons,
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Vessels.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<VesselEditViewModel?> GetVesselForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.Vessels
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new VesselEditViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImoNumber = x.ImoNumber,
                    MmsiNumber = x.MmsiNumber,
                    VesselType = x.VesselType,
                    CapacityTeu = x.CapacityTeu,
                    DeadweightTons = x.DeadweightTons,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateVesselAsync(Guid companyId, VesselEditViewModel model)
        {
            var entity = await _dbContext.Vessels
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.Name = model.Name.Trim();
            entity.ImoNumber = TrimOrNull(model.ImoNumber);
            entity.MmsiNumber = TrimOrNull(model.MmsiNumber);
            entity.VesselType = model.VesselType;
            entity.CapacityTeu = model.CapacityTeu;
            entity.DeadweightTons = model.DeadweightTons;
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVesselAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Vessels
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<VesselPositionListItemViewModel>> GetVesselPositionsAsync(Guid companyId)
        {
            return await _dbContext.VesselPositions
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Vessel.CompanyId == companyId)
                .OrderByDescending(x => x.PositionTimeUtc)
                .Select(x => new VesselPositionListItemViewModel
                {
                    Id = x.Id,
                    VesselId = x.VesselId,
                    VesselName = x.Vessel.Name,
                    PositionTimeUtc = x.PositionTimeUtc,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    SpeedKnots = x.SpeedKnots,
                    CourseDegrees = x.CourseDegrees,
                    Source = x.Source,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<VesselPositionCreateViewModel> GetCreateVesselPositionModelAsync(Guid companyId)
        {
            var model = new VesselPositionCreateViewModel();
            await PopulateVesselOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateVesselPositionAsync(Guid companyId, VesselPositionCreateViewModel model)
        {
            if (!await OwnsVesselAsync(companyId, model.VesselId))
            {
                return null;
            }

            var entity = new VesselPosition
            {
                VesselId = model.VesselId,
                PositionTimeUtc = model.PositionTimeUtc,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                SpeedKnots = model.SpeedKnots,
                CourseDegrees = model.CourseDegrees,
                Source = TrimOrNull(model.Source),
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.VesselPositions.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<VesselPositionEditViewModel?> GetVesselPositionForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.VesselPositions
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Vessel.CompanyId == companyId && x.Id == id)
                .Select(x => new VesselPositionEditViewModel
                {
                    Id = x.Id,
                    VesselId = x.VesselId,
                    PositionTimeUtc = x.PositionTimeUtc,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    SpeedKnots = x.SpeedKnots,
                    CourseDegrees = x.CourseDegrees,
                    Source = x.Source,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateVesselOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateVesselPositionAsync(Guid companyId, VesselPositionEditViewModel model)
        {
            var entity = await _dbContext.VesselPositions
                .Include(x => x.Vessel)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Vessel.CompanyId == companyId);

            if (entity == null || !await OwnsVesselAsync(companyId, model.VesselId))
            {
                return false;
            }

            entity.VesselId = model.VesselId;
            entity.PositionTimeUtc = model.PositionTimeUtc;
            entity.Latitude = model.Latitude;
            entity.Longitude = model.Longitude;
            entity.SpeedKnots = model.SpeedKnots;
            entity.CourseDegrees = model.CourseDegrees;
            entity.Source = TrimOrNull(model.Source);
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVesselPositionAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.VesselPositions
                .Include(x => x.Vessel)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.Vessel.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<VoyageListItemViewModel>> GetVoyagesAsync(Guid companyId)
        {
            return await _dbContext.Voyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Vessel.CompanyId == companyId)
                .OrderByDescending(x => x.PlannedDepartureUtc)
                .ThenBy(x => x.VoyageNumber)
                .Select(x => new VoyageListItemViewModel
                {
                    Id = x.Id,
                    VesselId = x.VesselId,
                    VesselName = x.Vessel.Name,
                    VoyageNumber = x.VoyageNumber,
                    Status = x.Status.ToString(),
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    OriginPort = x.OriginPort,
                    DestinationPort = x.DestinationPort,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<VoyageCreateViewModel> GetCreateVoyageModelAsync(Guid companyId)
        {
            var model = new VoyageCreateViewModel();
            await PopulateVesselOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateVoyageAsync(Guid companyId, VoyageCreateViewModel model)
        {
            if (!await OwnsVesselAsync(companyId, model.VesselId))
            {
                return null;
            }

            var entity = new Voyage
            {
                VesselId = model.VesselId,
                VoyageNumber = model.VoyageNumber.Trim(),
                Status = model.Status,
                PlannedDepartureUtc = model.PlannedDepartureUtc,
                PlannedArrivalUtc = model.PlannedArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                OriginPort = model.OriginPort.Trim(),
                DestinationPort = model.DestinationPort.Trim(),
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Voyages.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<VoyageEditViewModel?> GetVoyageForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.Voyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Vessel.CompanyId == companyId && x.Id == id)
                .Select(x => new VoyageEditViewModel
                {
                    Id = x.Id,
                    VesselId = x.VesselId,
                    VoyageNumber = x.VoyageNumber,
                    Status = x.Status,
                    PlannedDepartureUtc = x.PlannedDepartureUtc,
                    PlannedArrivalUtc = x.PlannedArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    OriginPort = x.OriginPort,
                    DestinationPort = x.DestinationPort,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateVesselOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateVoyageAsync(Guid companyId, VoyageEditViewModel model)
        {
            var entity = await _dbContext.Voyages
                .Include(x => x.Vessel)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Vessel.CompanyId == companyId);

            if (entity == null || !await OwnsVesselAsync(companyId, model.VesselId))
            {
                return false;
            }

            entity.VesselId = model.VesselId;
            entity.VoyageNumber = model.VoyageNumber.Trim();
            entity.Status = model.Status;
            entity.PlannedDepartureUtc = model.PlannedDepartureUtc;
            entity.PlannedArrivalUtc = model.PlannedArrivalUtc;
            entity.ActualDepartureUtc = model.ActualDepartureUtc;
            entity.ActualArrivalUtc = model.ActualArrivalUtc;
            entity.OriginPort = model.OriginPort.Trim();
            entity.DestinationPort = model.DestinationPort.Trim();
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVoyageAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Voyages
                .Include(x => x.Vessel)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.Vessel.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<VoyageStopListItemViewModel>> GetVoyageStopsAsync(Guid companyId)
        {
            return await _dbContext.VoyageStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Voyage.Vessel.CompanyId == companyId)
                .OrderBy(x => x.Voyage.VoyageNumber)
                .ThenBy(x => x.SequenceNumber)
                .Select(x => new VoyageStopListItemViewModel
                {
                    Id = x.Id,
                    VoyageId = x.VoyageId,
                    VoyageNumber = x.Voyage.VoyageNumber,
                    VesselName = x.Voyage.Vessel.Name,
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

        public async Task<VoyageStopCreateViewModel> GetCreateVoyageStopModelAsync(Guid companyId)
        {
            var model = new VoyageStopCreateViewModel();
            await PopulateVoyageStopOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateVoyageStopAsync(Guid companyId, VoyageStopCreateViewModel model)
        {
            if (!await OwnsVoyageAsync(companyId, model.VoyageId) ||
                !await _dbContext.Locations.AnyAsync(x => !x.IsDeleted && x.Id == model.LocationId))
            {
                return null;
            }

            var entity = new VoyageStop
            {
                VoyageId = model.VoyageId,
                LocationId = model.LocationId,
                SequenceNumber = model.SequenceNumber,
                PlannedArrivalUtc = model.PlannedArrivalUtc,
                PlannedDepartureUtc = model.PlannedDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.VoyageStops.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<VoyageStopEditViewModel?> GetVoyageStopForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.VoyageStops
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Voyage.Vessel.CompanyId == companyId && x.Id == id)
                .Select(x => new VoyageStopEditViewModel
                {
                    Id = x.Id,
                    VoyageId = x.VoyageId,
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
                await PopulateVoyageStopOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateVoyageStopAsync(Guid companyId, VoyageStopEditViewModel model)
        {
            var entity = await _dbContext.VoyageStops
                .Include(x => x.Voyage)
                .ThenInclude(x => x.Vessel)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Voyage.Vessel.CompanyId == companyId);

            if (entity == null ||
                !await OwnsVoyageAsync(companyId, model.VoyageId) ||
                !await _dbContext.Locations.AnyAsync(x => !x.IsDeleted && x.Id == model.LocationId))
            {
                return false;
            }

            entity.VoyageId = model.VoyageId;
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

        public async Task<bool> DeleteVoyageStopAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.VoyageStops
                .Include(x => x.Voyage)
                .ThenInclude(x => x.Vessel)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.Voyage.Vessel.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<VesselCrewMemberListItemViewModel>> GetVesselCrewMembersAsync(Guid companyId)
        {
            return await _dbContext.VesselCrewMembers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.FullName)
                .Select(x => new VesselCrewMemberListItemViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    CrewRole = x.CrewRole.ToString(),
                    SeamanBookNumber = x.SeamanBookNumber,
                    CertificateNumber = x.CertificateNumber,
                    Phone = x.Phone,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<VesselCrewMemberCreateViewModel> GetCreateVesselCrewMemberModelAsync(Guid companyId) =>
            Task.FromResult(new VesselCrewMemberCreateViewModel());

        public async Task<Guid?> CreateVesselCrewMemberAsync(Guid companyId, VesselCrewMemberCreateViewModel model)
        {
            var entity = new VesselCrewMember
            {
                CompanyId = companyId,
                FullName = model.FullName.Trim(),
                CrewRole = model.CrewRole,
                SeamanBookNumber = TrimOrNull(model.SeamanBookNumber),
                CertificateNumber = TrimOrNull(model.CertificateNumber),
                Phone = TrimOrNull(model.Phone),
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.VesselCrewMembers.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<VesselCrewMemberEditViewModel?> GetVesselCrewMemberForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.VesselCrewMembers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new VesselCrewMemberEditViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    CrewRole = x.CrewRole,
                    SeamanBookNumber = x.SeamanBookNumber,
                    CertificateNumber = x.CertificateNumber,
                    Phone = x.Phone,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateVesselCrewMemberAsync(Guid companyId, VesselCrewMemberEditViewModel model)
        {
            var entity = await _dbContext.VesselCrewMembers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.FullName = model.FullName.Trim();
            entity.CrewRole = model.CrewRole;
            entity.SeamanBookNumber = TrimOrNull(model.SeamanBookNumber);
            entity.CertificateNumber = TrimOrNull(model.CertificateNumber);
            entity.Phone = TrimOrNull(model.Phone);
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVesselCrewMemberAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.VesselCrewMembers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CrewAssignmentListItemViewModel>> GetCrewAssignmentsAsync(Guid companyId)
        {
            return await _dbContext.CrewAssignments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Voyage.Vessel.CompanyId == companyId && x.VesselCrewMember.CompanyId == companyId)
                .OrderByDescending(x => x.AssignedAtUtc)
                .Select(x => new CrewAssignmentListItemViewModel
                {
                    Id = x.Id,
                    VoyageId = x.VoyageId,
                    VesselCrewMemberId = x.VesselCrewMemberId,
                    VoyageNumber = x.Voyage.VoyageNumber,
                    VesselName = x.Voyage.Vessel.Name,
                    CrewMemberName = x.VesselCrewMember.FullName,
                    AssignedRole = x.AssignedRole.ToString(),
                    AssignedAtUtc = x.AssignedAtUtc,
                    FromUtc = x.FromUtc,
                    ToUtc = x.ToUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<CrewAssignmentCreateViewModel> GetCreateCrewAssignmentModelAsync(Guid companyId)
        {
            var model = new CrewAssignmentCreateViewModel();
            await PopulateCrewAssignmentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateCrewAssignmentAsync(Guid companyId, CrewAssignmentCreateViewModel model)
        {
            if (!await OwnsVoyageAsync(companyId, model.VoyageId) ||
                !await OwnsCrewMemberAsync(companyId, model.VesselCrewMemberId))
            {
                return null;
            }

            var entity = new CrewAssignment
            {
                VoyageId = model.VoyageId,
                VesselCrewMemberId = model.VesselCrewMemberId,
                AssignedRole = model.AssignedRole,
                AssignedAtUtc = model.AssignedAtUtc,
                FromUtc = model.FromUtc,
                ToUtc = model.ToUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.CrewAssignments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CrewAssignmentEditViewModel?> GetCrewAssignmentForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.CrewAssignments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Voyage.Vessel.CompanyId == companyId && x.VesselCrewMember.CompanyId == companyId && x.Id == id)
                .Select(x => new CrewAssignmentEditViewModel
                {
                    Id = x.Id,
                    VoyageId = x.VoyageId,
                    VesselCrewMemberId = x.VesselCrewMemberId,
                    AssignedRole = x.AssignedRole,
                    AssignedAtUtc = x.AssignedAtUtc,
                    FromUtc = x.FromUtc,
                    ToUtc = x.ToUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateCrewAssignmentOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateCrewAssignmentAsync(Guid companyId, CrewAssignmentEditViewModel model)
        {
            var entity = await _dbContext.CrewAssignments
                .Include(x => x.Voyage)
                .ThenInclude(x => x.Vessel)
                .Include(x => x.VesselCrewMember)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == model.Id &&
                    x.Voyage.Vessel.CompanyId == companyId &&
                    x.VesselCrewMember.CompanyId == companyId);

            if (entity == null ||
                !await OwnsVoyageAsync(companyId, model.VoyageId) ||
                !await OwnsCrewMemberAsync(companyId, model.VesselCrewMemberId))
            {
                return false;
            }

            entity.VoyageId = model.VoyageId;
            entity.VesselCrewMemberId = model.VesselCrewMemberId;
            entity.AssignedRole = model.AssignedRole;
            entity.AssignedAtUtc = model.AssignedAtUtc;
            entity.FromUtc = model.FromUtc;
            entity.ToUtc = model.ToUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCrewAssignmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.CrewAssignments
                .Include(x => x.Voyage)
                .ThenInclude(x => x.Vessel)
                .Include(x => x.VesselCrewMember)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    x.Id == id &&
                    x.Voyage.Vessel.CompanyId == companyId &&
                    x.VesselCrewMember.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private static string? TrimOrNull(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static void SoftDelete(dynamic entity)
        {
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
        }

        private Task<bool> OwnsVesselAsync(Guid companyId, Guid vesselId) =>
            _dbContext.Vessels.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == vesselId);

        private Task<bool> OwnsVoyageAsync(Guid companyId, Guid voyageId) =>
            _dbContext.Voyages.AnyAsync(x => !x.IsDeleted && x.Vessel.CompanyId == companyId && x.Id == voyageId);

        private Task<bool> OwnsCrewMemberAsync(Guid companyId, Guid crewMemberId) =>
            _dbContext.VesselCrewMembers.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == crewMemberId);

        private async Task PopulateVesselOptionsAsync(Guid companyId, VesselPositionCreateViewModel model) =>
            model.VesselOptions = await GetVesselOptionsAsync(companyId);

        private async Task PopulateVesselOptionsAsync(Guid companyId, VesselPositionEditViewModel model) =>
            model.VesselOptions = await GetVesselOptionsAsync(companyId);

        private async Task PopulateVesselOptionsAsync(Guid companyId, VoyageCreateViewModel model) =>
            model.VesselOptions = await GetVesselOptionsAsync(companyId);

        private async Task PopulateVesselOptionsAsync(Guid companyId, VoyageEditViewModel model) =>
            model.VesselOptions = await GetVesselOptionsAsync(companyId);

        private async Task PopulateVoyageStopOptionsAsync(Guid companyId, VoyageStopCreateViewModel model)
        {
            model.VoyageOptions = await GetVoyageOptionsAsync(companyId);
            model.LocationOptions = await GetLocationOptionsAsync();
        }

        private async Task PopulateVoyageStopOptionsAsync(Guid companyId, VoyageStopEditViewModel model)
        {
            model.VoyageOptions = await GetVoyageOptionsAsync(companyId);
            model.LocationOptions = await GetLocationOptionsAsync();
        }

        private async Task PopulateCrewAssignmentOptionsAsync(Guid companyId, CrewAssignmentCreateViewModel model)
        {
            model.VoyageOptions = await GetVoyageOptionsAsync(companyId);
            model.CrewMemberOptions = await GetCrewMemberOptionsAsync(companyId);
        }

        private async Task PopulateCrewAssignmentOptionsAsync(Guid companyId, CrewAssignmentEditViewModel model)
        {
            model.VoyageOptions = await GetVoyageOptionsAsync(companyId);
            model.CrewMemberOptions = await GetCrewMemberOptionsAsync(companyId);
        }

        private async Task<IEnumerable<SelectListItem>> GetVesselOptionsAsync(Guid companyId) =>
            await _dbContext.Vessels
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetVoyageOptionsAsync(Guid companyId) =>
            await _dbContext.Voyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Vessel.CompanyId == companyId)
                .OrderBy(x => x.VoyageNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyageNumber + " / " + x.Vessel.Name
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetCrewMemberOptionsAsync(Guid companyId) =>
            await _dbContext.VesselCrewMembers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName + " / " + x.CrewRole
                })
                .ToListAsync();
    }
}