using LogisticManagementApp.Domain.Assets.Air;
using LogisticManagementApp.Models.CompanyPortal.Assets.Air;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyAirHomeViewModel> GetAirAssetsHomeAsync()
        {
            return Task.FromResult(
                new CompanyAirHomeViewModel
                {
                    Cards = new[]
                    {
                        new AirAssetCardViewModel
                        {
                            Title = "Aircraft",
                            Description = "Самолети и основни параметри.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Aircraft),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateAircraft)
                        },
                        new AirAssetCardViewModel
                        {
                            Title = "Flights",
                            Description = "Планирани и активни полети.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Flights),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateFlight)
                        },
                        new AirAssetCardViewModel
                        {
                            Title = "Flight Segments",
                            Description = "Сегменти по полети.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.FlightSegments),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateFlightSegment)
                        },
                        new AirAssetCardViewModel
                        {
                            Title = "Air Crew Members",
                            Description = "Екипажите на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.AirCrewMembers),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateAirCrewMember)
                        },
                        new AirAssetCardViewModel
                        {
                            Title = "Air Crew Assignments",
                            Description = "Разпределения на екипаж към полети.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.AirCrewAssignments),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateAirCrewAssignment)
                        },
                        new AirAssetCardViewModel
                        {
                            Title = "ULDs",
                            Description = "Управление на ULD оборудване.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.ULDs),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateULD)
                        }
                    }
                });
        }

        public async Task<IEnumerable<AircraftListItemViewModel>> GetAircraftAsync(Guid companyId)
        {
            return await _dbContext.Aircraft
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.TailNumber)
                .Select(x => new AircraftListItemViewModel
                {
                    Id = x.Id,
                    TailNumber = x.TailNumber,
                    AircraftType = x.AircraftType.ToString(),
                    Manufacturer = x.Manufacturer,
                    Model = x.Model,
                    MaxPayloadKg = x.MaxPayloadKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<AircraftCreateViewModel> GetCreateAircraftModelAsync(Guid companyId) =>
            Task.FromResult(new AircraftCreateViewModel());

        public async Task<Guid?> CreateAircraftAsync(Guid companyId, AircraftCreateViewModel model)
        {
            var entity = new Aircraft
            {
                CompanyId = companyId,
                TailNumber = model.TailNumber.Trim(),
                AircraftType = model.AircraftType,
                Manufacturer = TrimOrNull(model.Manufacturer),
                Model = TrimOrNull(model.Model),
                MaxPayloadKg = model.MaxPayloadKg,
                MaxVolumeCbm = model.MaxVolumeCbm,
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Aircraft.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<AircraftEditViewModel?> GetAircraftForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.Aircraft
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new AircraftEditViewModel
                {
                    Id = x.Id,
                    TailNumber = x.TailNumber,
                    AircraftType = x.AircraftType,
                    Manufacturer = x.Manufacturer,
                    Model = x.Model,
                    MaxPayloadKg = x.MaxPayloadKg,
                    MaxVolumeCbm = x.MaxVolumeCbm,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAircraftAsync(Guid companyId, AircraftEditViewModel model)
        {
            var entity = await _dbContext.Aircraft
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.TailNumber = model.TailNumber.Trim();
            entity.AircraftType = model.AircraftType;
            entity.Manufacturer = TrimOrNull(model.Manufacturer);
            entity.Model = TrimOrNull(model.Model);
            entity.MaxPayloadKg = model.MaxPayloadKg;
            entity.MaxVolumeCbm = model.MaxVolumeCbm;
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAircraftAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Aircraft
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<FlightListItemViewModel>> GetFlightsAsync(Guid companyId)
        {
            return await _dbContext.Flights
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Aircraft.CompanyId == companyId)
                .OrderByDescending(x => x.ScheduledDepartureUtc)
                .ThenBy(x => x.FlightNumber)
                .Select(x => new FlightListItemViewModel
                {
                    Id = x.Id,
                    FlightNumber = x.FlightNumber,
                    AircraftDisplay = x.Aircraft.TailNumber + " / " + x.Aircraft.AircraftType,
                    OriginLocation = x.OriginLocation != null ? x.OriginLocation.Name : null,
                    DestinationLocation = x.DestinationLocation != null ? x.DestinationLocation.Name : null,
                    Status = x.Status.ToString(),
                    ScheduledDepartureUtc = x.ScheduledDepartureUtc,
                    ScheduledArrivalUtc = x.ScheduledArrivalUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<FlightCreateViewModel> GetCreateFlightModelAsync(Guid companyId)
        {
            var model = new FlightCreateViewModel();
            await PopulateFlightOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateFlightAsync(Guid companyId, FlightCreateViewModel model)
        {
            if (!await OwnsAircraftAsync(companyId, model.AircraftId))
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

            var entity = new Flight
            {
                AircraftId = model.AircraftId,
                FlightNumber = model.FlightNumber.Trim(),
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                Status = model.Status,
                ScheduledDepartureUtc = model.ScheduledDepartureUtc,
                ScheduledArrivalUtc = model.ScheduledArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Flights.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<FlightEditViewModel?> GetFlightForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.Flights
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Aircraft.CompanyId == companyId && x.Id == id)
                .Select(x => new FlightEditViewModel
                {
                    Id = x.Id,
                    AircraftId = x.AircraftId,
                    FlightNumber = x.FlightNumber,
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
                await PopulateFlightOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateFlightAsync(Guid companyId, FlightEditViewModel model)
        {
            var entity = await _dbContext.Flights
                .Include(x => x.Aircraft)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Aircraft.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await OwnsAircraftAsync(companyId, model.AircraftId))
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

            entity.AircraftId = model.AircraftId;
            entity.FlightNumber = model.FlightNumber.Trim();
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

        public async Task<bool> DeleteFlightAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Flights
                .Include(x => x.Aircraft)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Aircraft.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<FlightSegmentListItemViewModel>> GetFlightSegmentsAsync(Guid companyId)
        {
            return await _dbContext.FlightSegments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Flight.Aircraft.CompanyId == companyId)
                .OrderBy(x => x.Flight.FlightNumber)
                .ThenBy(x => x.SegmentNo)
                .Select(x => new FlightSegmentListItemViewModel
                {
                    Id = x.Id,
                    FlightId = x.FlightId,
                    FlightNumber = x.Flight.FlightNumber,
                    AircraftDisplay = x.Flight.Aircraft.TailNumber + " / " + x.Flight.Aircraft.AircraftType,
                    SegmentNo = x.SegmentNo,
                    OriginLocation = x.OriginLocation != null ? x.OriginLocation.Name : null,
                    DestinationLocation = x.DestinationLocation != null ? x.DestinationLocation.Name : null,
                    ScheduledDepartureUtc = x.ScheduledDepartureUtc,
                    ScheduledArrivalUtc = x.ScheduledArrivalUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<FlightSegmentCreateViewModel> GetCreateFlightSegmentModelAsync(Guid companyId)
        {
            var model = new FlightSegmentCreateViewModel();
            await PopulateFlightSegmentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateFlightSegmentAsync(Guid companyId, FlightSegmentCreateViewModel model)
        {
            if (!await OwnsFlightAsync(companyId, model.FlightId))
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

            var entity = new FlightSegment
            {
                FlightId = model.FlightId,
                SegmentNo = model.SegmentNo,
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                ScheduledDepartureUtc = model.ScheduledDepartureUtc,
                ScheduledArrivalUtc = model.ScheduledArrivalUtc,
                ActualDepartureUtc = model.ActualDepartureUtc,
                ActualArrivalUtc = model.ActualArrivalUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.FlightSegments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<FlightSegmentEditViewModel?> GetFlightSegmentForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.FlightSegments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Flight.Aircraft.CompanyId == companyId && x.Id == id)
                .Select(x => new FlightSegmentEditViewModel
                {
                    Id = x.Id,
                    FlightId = x.FlightId,
                    SegmentNo = x.SegmentNo,
                    OriginLocationId = x.OriginLocationId,
                    DestinationLocationId = x.DestinationLocationId,
                    ScheduledDepartureUtc = x.ScheduledDepartureUtc,
                    ScheduledArrivalUtc = x.ScheduledArrivalUtc,
                    ActualDepartureUtc = x.ActualDepartureUtc,
                    ActualArrivalUtc = x.ActualArrivalUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateFlightSegmentOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateFlightSegmentAsync(Guid companyId, FlightSegmentEditViewModel model)
        {
            var entity = await _dbContext.FlightSegments
                .Include(x => x.Flight)
                .ThenInclude(x => x.Aircraft)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Flight.Aircraft.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await OwnsFlightAsync(companyId, model.FlightId))
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

            entity.FlightId = model.FlightId;
            entity.SegmentNo = model.SegmentNo;
            entity.OriginLocationId = model.OriginLocationId;
            entity.DestinationLocationId = model.DestinationLocationId;
            entity.ScheduledDepartureUtc = model.ScheduledDepartureUtc;
            entity.ScheduledArrivalUtc = model.ScheduledArrivalUtc;
            entity.ActualDepartureUtc = model.ActualDepartureUtc;
            entity.ActualArrivalUtc = model.ActualArrivalUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFlightSegmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.FlightSegments
                .Include(x => x.Flight)
                .ThenInclude(x => x.Aircraft)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Flight.Aircraft.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AirCrewMemberListItemViewModel>> GetAirCrewMembersAsync(Guid companyId)
        {
            return await _dbContext.AirCrewMembers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.FullName)
                .Select(x => new AirCrewMemberListItemViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    CrewRole = x.CrewRole.ToString(),
                    LicenseNumber = x.LicenseNumber,
                    Phone = x.Phone,
                    Status = x.Status.ToString(),
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<AirCrewMemberCreateViewModel> GetCreateAirCrewMemberModelAsync(Guid companyId) =>
            Task.FromResult(new AirCrewMemberCreateViewModel());

        public async Task<Guid?> CreateAirCrewMemberAsync(Guid companyId, AirCrewMemberCreateViewModel model)
        {
            var entity = new AirCrewMember
            {
                CompanyId = companyId,
                FullName = model.FullName.Trim(),
                CrewRole = model.CrewRole,
                LicenseNumber = TrimOrNull(model.LicenseNumber),
                Phone = TrimOrNull(model.Phone),
                Status = model.Status,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.AirCrewMembers.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<AirCrewMemberEditViewModel?> GetAirCrewMemberForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.AirCrewMembers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id)
                .Select(x => new AirCrewMemberEditViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    CrewRole = x.CrewRole,
                    LicenseNumber = x.LicenseNumber,
                    Phone = x.Phone,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAirCrewMemberAsync(Guid companyId, AirCrewMemberEditViewModel model)
        {
            var entity = await _dbContext.AirCrewMembers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.FullName = model.FullName.Trim();
            entity.CrewRole = model.CrewRole;
            entity.LicenseNumber = TrimOrNull(model.LicenseNumber);
            entity.Phone = TrimOrNull(model.Phone);
            entity.Status = model.Status;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAirCrewMemberAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.AirCrewMembers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<AirCrewAssignmentListItemViewModel>> GetAirCrewAssignmentsAsync(Guid companyId)
        {
            return await _dbContext.AirCrewAssignments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Flight.Aircraft.CompanyId == companyId && x.AirCrewMember.CompanyId == companyId)
                .OrderByDescending(x => x.AssignedAtUtc)
                .ThenBy(x => x.Flight.FlightNumber)
                .Select(x => new AirCrewAssignmentListItemViewModel
                {
                    Id = x.Id,
                    FlightId = x.FlightId,
                    AirCrewMemberId = x.AirCrewMemberId,
                    FlightNumber = x.Flight.FlightNumber,
                    AircraftDisplay = x.Flight.Aircraft.TailNumber + " / " + x.Flight.Aircraft.AircraftType,
                    CrewMemberName = x.AirCrewMember.FullName,
                    AssignedRole = x.AssignedRole.ToString(),
                    AssignedAtUtc = x.AssignedAtUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<AirCrewAssignmentCreateViewModel> GetCreateAirCrewAssignmentModelAsync(Guid companyId)
        {
            var model = new AirCrewAssignmentCreateViewModel();
            await PopulateAirCrewAssignmentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateAirCrewAssignmentAsync(Guid companyId, AirCrewAssignmentCreateViewModel model)
        {
            if (!await OwnsFlightAsync(companyId, model.FlightId) ||
                !await OwnsAirCrewMemberAsync(companyId, model.AirCrewMemberId))
            {
                return null;
            }

            var entity = new AirCrewAssignment
            {
                FlightId = model.FlightId,
                AirCrewMemberId = model.AirCrewMemberId,
                AssignedRole = model.AssignedRole,
                AssignedAtUtc = model.AssignedAtUtc,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.AirCrewAssignments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<AirCrewAssignmentEditViewModel?> GetAirCrewAssignmentForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.AirCrewAssignments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == id && x.Flight.Aircraft.CompanyId == companyId && x.AirCrewMember.CompanyId == companyId)
                .Select(x => new AirCrewAssignmentEditViewModel
                {
                    Id = x.Id,
                    FlightId = x.FlightId,
                    AirCrewMemberId = x.AirCrewMemberId,
                    AssignedRole = x.AssignedRole,
                    AssignedAtUtc = x.AssignedAtUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateAirCrewAssignmentOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateAirCrewAssignmentAsync(Guid companyId, AirCrewAssignmentEditViewModel model)
        {
            var entity = await _dbContext.AirCrewAssignments
                .Include(x => x.Flight)
                .ThenInclude(x => x.Aircraft)
                .Include(x => x.AirCrewMember)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Flight.Aircraft.CompanyId == companyId && x.AirCrewMember.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            if (!await OwnsFlightAsync(companyId, model.FlightId) ||
                !await OwnsAirCrewMemberAsync(companyId, model.AirCrewMemberId))
            {
                return false;
            }

            entity.FlightId = model.FlightId;
            entity.AirCrewMemberId = model.AirCrewMemberId;
            entity.AssignedRole = model.AssignedRole;
            entity.AssignedAtUtc = model.AssignedAtUtc;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAirCrewAssignmentAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.AirCrewAssignments
                .Include(x => x.Flight)
                .ThenInclude(x => x.Aircraft)
                .Include(x => x.AirCrewMember)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id && x.Flight.Aircraft.CompanyId == companyId && x.AirCrewMember.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UldListItemViewModel>> GetUldsAsync(Guid companyId)
        {
            return await _dbContext.ULDs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId)
                .OrderBy(x => x.UldNumber)
                .Select(x => new UldListItemViewModel
                {
                    Id = x.Id,
                    UldNumber = x.UldNumber,
                    UldType = x.UldType.ToString(),
                    Status = x.Status.ToString(),
                    TareWeightKg = x.TareWeightKg,
                    MaxGrossWeightKg = x.MaxGrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public Task<UldCreateViewModel> GetCreateUldModelAsync(Guid companyId) =>
            Task.FromResult(new UldCreateViewModel());

        public async Task<Guid?> CreateUldAsync(Guid companyId, UldCreateViewModel model)
        {
            var entity = new ULD
            {
                OwnerCompanyId = companyId,
                UldNumber = model.UldNumber.Trim(),
                UldType = model.UldType,
                Status = model.Status,
                TareWeightKg = model.TareWeightKg,
                MaxGrossWeightKg = model.MaxGrossWeightKg,
                VolumeCbm = model.VolumeCbm,
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.ULDs.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<UldEditViewModel?> GetUldForEditAsync(Guid companyId, Guid id)
        {
            return await _dbContext.ULDs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == id)
                .Select(x => new UldEditViewModel
                {
                    Id = x.Id,
                    UldNumber = x.UldNumber,
                    UldType = x.UldType,
                    Status = x.Status,
                    TareWeightKg = x.TareWeightKg,
                    MaxGrossWeightKg = x.MaxGrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUldAsync(Guid companyId, UldEditViewModel model)
        {
            var entity = await _dbContext.ULDs
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.UldNumber = model.UldNumber.Trim();
            entity.UldType = model.UldType;
            entity.Status = model.Status;
            entity.TareWeightKg = model.TareWeightKg;
            entity.MaxGrossWeightKg = model.MaxGrossWeightKg;
            entity.VolumeCbm = model.VolumeCbm;
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUldAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ULDs
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private Task<bool> OwnsAircraftAsync(Guid companyId, Guid aircraftId) =>
            _dbContext.Aircraft.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == aircraftId);

        private Task<bool> OwnsFlightAsync(Guid companyId, Guid flightId) =>
            _dbContext.Flights.AnyAsync(x => !x.IsDeleted && x.Aircraft.CompanyId == companyId && x.Id == flightId);

        private Task<bool> OwnsAirCrewMemberAsync(Guid companyId, Guid airCrewMemberId) =>
            _dbContext.AirCrewMembers.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == airCrewMemberId);

        private Task<bool> ExistsLocationAsync(Guid locationId) =>
            _dbContext.Locations.AnyAsync(x => !x.IsDeleted && x.Id == locationId);

        private async Task PopulateFlightOptionsAsync(Guid companyId, FlightCreateViewModel model)
        {
            model.AircraftOptions = await GetAircraftOptionsAsync(companyId);
            model.LocationOptions = await GetAirLocationOptionsAsync();
        }

        private async Task PopulateFlightOptionsAsync(Guid companyId, FlightEditViewModel model)
        {
            model.AircraftOptions = await GetAircraftOptionsAsync(companyId);
            model.LocationOptions = await GetAirLocationOptionsAsync();
        }

        private async Task PopulateFlightSegmentOptionsAsync(Guid companyId, FlightSegmentCreateViewModel model)
        {
            model.FlightOptions = await GetFlightOptionsAsync(companyId);
            model.LocationOptions = await GetAirLocationOptionsAsync();
        }

        private async Task PopulateFlightSegmentOptionsAsync(Guid companyId, FlightSegmentEditViewModel model)
        {
            model.FlightOptions = await GetFlightOptionsAsync(companyId);
            model.LocationOptions = await GetAirLocationOptionsAsync();
        }

        private async Task PopulateAirCrewAssignmentOptionsAsync(Guid companyId, AirCrewAssignmentCreateViewModel model)
        {
            model.FlightOptions = await GetFlightOptionsAsync(companyId);
            model.AirCrewMemberOptions = await GetAirCrewMemberOptionsAsync(companyId);
        }

        private async Task PopulateAirCrewAssignmentOptionsAsync(Guid companyId, AirCrewAssignmentEditViewModel model)
        {
            model.FlightOptions = await GetFlightOptionsAsync(companyId);
            model.AirCrewMemberOptions = await GetAirCrewMemberOptionsAsync(companyId);
        }

        private async Task<IEnumerable<SelectListItem>> GetAircraftOptionsAsync(Guid companyId) =>
            await _dbContext.Aircraft
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.TailNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TailNumber + " / " + x.AircraftType
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetFlightOptionsAsync(Guid companyId) =>
            await _dbContext.Flights
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Aircraft.CompanyId == companyId)
                .OrderBy(x => x.FlightNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FlightNumber + " / " + x.Aircraft.TailNumber
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetAirCrewMemberOptionsAsync(Guid companyId) =>
            await _dbContext.AirCrewMembers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName + " / " + x.CrewRole
                })
                .ToListAsync();

        private async Task<IEnumerable<SelectListItem>> GetAirLocationOptionsAsync() =>
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
    }
}