using LogisticManagementApp.Domain.Assets.CargoUnits;
using LogisticManagementApp.Models.CompanyPortal.Assets.CargoUnits;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        public Task<CompanyCargoUnitHomeViewModel> GetCargoUnitAssetsHomeAsync() =>
            Task.FromResult(
                new CompanyCargoUnitHomeViewModel
                {
                    Cards = new[]
                    {
                        new CargoUnitAssetCardViewModel
                        {
                            Title = "Containers",
                            Description = "Контейнери на компанията.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.Containers),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateContainer)
                        },
                        new CargoUnitAssetCardViewModel
                        {
                            Title = "Container Seals",
                            Description = "Пломби към контейнери.",
                            ActionName = nameof(LogisticManagementApp.Controllers.CompanyController.ContainerSeals),
                            CreateActionName = nameof(LogisticManagementApp.Controllers.CompanyController.CreateContainerSeal)
                        }
                    }
                });

        public async Task<IEnumerable<ContainerListItemViewModel>> GetContainersAsync(Guid companyId) =>
            await _dbContext.Containers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId)
                .OrderBy(x => x.ContainerNumber)
                .Select(x => new ContainerListItemViewModel
                {
                    Id = x.Id,
                    ContainerNumber = x.ContainerNumber,
                    ContainerType = x.ContainerType.ToString(),
                    Status = x.Status.ToString(),
                    TareWeightKg = x.TareWeightKg,
                    MaxGrossWeightKg = x.MaxGrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    SealNumber = x.SealNumber,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();

        public Task<ContainerCreateViewModel> GetCreateContainerModelAsync(Guid companyId) =>
            Task.FromResult(new ContainerCreateViewModel());

        public async Task<Guid?> CreateContainerAsync(Guid companyId, ContainerCreateViewModel model)
        {
            var entity = new Container
            {
                OwnerCompanyId = companyId,
                ContainerNumber = model.ContainerNumber.Trim(),
                ContainerType = model.ContainerType,
                Status = model.Status,
                TareWeightKg = model.TareWeightKg,
                MaxGrossWeightKg = model.MaxGrossWeightKg,
                VolumeCbm = model.VolumeCbm,
                SealNumber = TrimOrNull(model.SealNumber),
                IsActive = model.IsActive,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.Containers.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<ContainerEditViewModel?> GetContainerForEditAsync(Guid companyId, Guid id) =>
            await _dbContext.Containers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == id)
                .Select(x => new ContainerEditViewModel
                {
                    Id = x.Id,
                    ContainerNumber = x.ContainerNumber,
                    ContainerType = x.ContainerType,
                    Status = x.Status,
                    TareWeightKg = x.TareWeightKg,
                    MaxGrossWeightKg = x.MaxGrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    SealNumber = x.SealNumber,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

        public async Task<bool> UpdateContainerAsync(Guid companyId, ContainerEditViewModel model)
        {
            var entity = await _dbContext.Containers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            entity.ContainerNumber = model.ContainerNumber.Trim();
            entity.ContainerType = model.ContainerType;
            entity.Status = model.Status;
            entity.TareWeightKg = model.TareWeightKg;
            entity.MaxGrossWeightKg = model.MaxGrossWeightKg;
            entity.VolumeCbm = model.VolumeCbm;
            entity.SealNumber = TrimOrNull(model.SealNumber);
            entity.IsActive = model.IsActive;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContainerAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.Containers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == id);

            if (entity == null)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ContainerSealListItemViewModel>> GetContainerSealsAsync(Guid companyId) =>
            await _dbContext.ContainerSeals
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Container.OwnerCompanyId == companyId)
                .OrderByDescending(x => x.AppliedAtUtc)
                .ThenBy(x => x.SealNumber)
                .Select(x => new ContainerSealListItemViewModel
                {
                    Id = x.Id,
                    ContainerId = x.ContainerId,
                    ContainerNumber = x.Container.ContainerNumber,
                    SealNumber = x.SealNumber,
                    AppliedAtUtc = x.AppliedAtUtc,
                    AppliedBy = x.AppliedBy,
                    RemovedAtUtc = x.RemovedAtUtc,
                    RemovedBy = x.RemovedBy,
                    IsActiveSeal = x.IsActiveSeal,
                    Notes = x.Notes
                })
                .ToListAsync();

        public async Task<ContainerSealCreateViewModel> GetCreateContainerSealModelAsync(Guid companyId)
        {
            var model = new ContainerSealCreateViewModel();
            await PopulateContainerSealOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateContainerSealAsync(Guid companyId, ContainerSealCreateViewModel model)
        {
            if (!await OwnsContainerAsync(companyId, model.ContainerId))
            {
                return null;
            }

            var entity = new ContainerSeal
            {
                ContainerId = model.ContainerId,
                SealNumber = model.SealNumber.Trim(),
                AppliedAtUtc = model.AppliedAtUtc,
                AppliedBy = TrimOrNull(model.AppliedBy),
                RemovedAtUtc = model.RemovedAtUtc,
                RemovedBy = TrimOrNull(model.RemovedBy),
                IsActiveSeal = model.IsActiveSeal,
                Notes = TrimOrNull(model.Notes)
            };

            _dbContext.ContainerSeals.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<ContainerSealEditViewModel?> GetContainerSealForEditAsync(Guid companyId, Guid id)
        {
            var model = await _dbContext.ContainerSeals
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Container.OwnerCompanyId == companyId && x.Id == id)
                .Select(x => new ContainerSealEditViewModel
                {
                    Id = x.Id,
                    ContainerId = x.ContainerId,
                    SealNumber = x.SealNumber,
                    AppliedAtUtc = x.AppliedAtUtc,
                    AppliedBy = x.AppliedBy,
                    RemovedAtUtc = x.RemovedAtUtc,
                    RemovedBy = x.RemovedBy,
                    IsActiveSeal = x.IsActiveSeal,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model != null)
            {
                await PopulateContainerSealOptionsAsync(companyId, model);
            }

            return model;
        }

        public async Task<bool> UpdateContainerSealAsync(Guid companyId, ContainerSealEditViewModel model)
        {
            var entity = await _dbContext.ContainerSeals
                .Include(x => x.Container)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null || !await OwnsContainerAsync(companyId, model.ContainerId))
            {
                return false;
            }

            entity.ContainerId = model.ContainerId;
            entity.SealNumber = model.SealNumber.Trim();
            entity.AppliedAtUtc = model.AppliedAtUtc;
            entity.AppliedBy = TrimOrNull(model.AppliedBy);
            entity.RemovedAtUtc = model.RemovedAtUtc;
            entity.RemovedBy = TrimOrNull(model.RemovedBy);
            entity.IsActiveSeal = model.IsActiveSeal;
            entity.Notes = TrimOrNull(model.Notes);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteContainerSealAsync(Guid companyId, Guid id)
        {
            var entity = await _dbContext.ContainerSeals
                .Include(x => x.Container)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);

            if (entity == null || entity.Container.OwnerCompanyId != companyId)
            {
                return false;
            }

            SoftDelete(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        private Task<bool> OwnsContainerAsync(Guid companyId, Guid containerId) =>
            _dbContext.Containers.AnyAsync(x => !x.IsDeleted && x.OwnerCompanyId == companyId && x.Id == containerId);

        private async Task PopulateContainerSealOptionsAsync(Guid companyId, ContainerSealCreateViewModel model) =>
            model.ContainerOptions = await _dbContext.Containers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId)
                .OrderBy(x => x.ContainerNumber)
                .Select(x => new SelectListItem(x.ContainerNumber, x.Id.ToString()))
                .ToListAsync();

        private async Task PopulateContainerSealOptionsAsync(Guid companyId, ContainerSealEditViewModel model) =>
            model.ContainerOptions = await _dbContext.Containers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.OwnerCompanyId == companyId)
                .OrderBy(x => x.ContainerNumber)
                .Select(x => new SelectListItem(x.ContainerNumber, x.Id.ToString()))
                .ToListAsync();
    }
}