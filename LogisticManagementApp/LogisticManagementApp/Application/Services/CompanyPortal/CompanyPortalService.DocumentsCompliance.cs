using LogisticManagementApp.Domain.Compliance;
using LogisticManagementApp.Domain.Documents;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Models.CompanyPortal.Compliance;
using LogisticManagementApp.Models.CompanyPortal.Documents;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService
    {
        #region Documents

        public async Task<IEnumerable<FileResourceListItemViewModel>> GetFileResourcesAsync(Guid companyId)
        {
            return await _dbContext.FileResources
                .AsNoTracking()
                .Where(fr =>
                    !fr.IsDeleted &&
                    (
                        _dbContext.Documents.Any(d =>
                            !d.IsDeleted &&
                            d.FileResourceId == fr.Id &&
                            (d.IssuedByCompanyId == companyId || d.Shipment.CustomerCompanyId == companyId)) ||
                        _dbContext.DocumentVersions.Any(v =>
                            !v.IsDeleted &&
                            v.FileResourceId == fr.Id &&
                            (v.Document.IssuedByCompanyId == companyId || v.Document.Shipment.CustomerCompanyId == companyId)) ||
                        _dbContext.DocumentTemplates.Any(t =>
                            !t.IsDeleted &&
                            t.FileResourceId == fr.Id &&
                            t.CompanyId == companyId) ||
                        _dbContext.DGDocuments.Any(dg =>
                            !dg.IsDeleted &&
                            dg.FileResourceId == fr.Id &&
                            dg.DangerousGoodsDeclaration.Shipment.CustomerCompanyId == companyId)
                    ))
                .OrderByDescending(x => x.UploadedAtUtc)
                .Select(x => new FileResourceListItemViewModel
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    ContentType = x.ContentType,
                    SizeBytes = x.SizeBytes,
                    StorageKey = x.StorageKey,
                    UploadedAtUtc = x.UploadedAtUtc
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CompanyDocumentListItemViewModel>> GetDocumentsAsync(Guid companyId)
        {
            return await _dbContext.Documents
                .AsNoTracking()
                .Where(x => !x.IsDeleted && (x.IssuedByCompanyId == companyId || x.Shipment.CustomerCompanyId == companyId))
                .OrderByDescending(x => x.IssuedAtUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyDocumentListItemViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    DocumentType = x.DocumentType.ToString(),
                    FileName = x.FileResource.FileName,
                    DocumentNo = x.DocumentNo,
                    IssuedAtUtc = x.IssuedAtUtc,
                    IssuedByCompanyName = x.IssuedByCompany != null ? x.IssuedByCompany.Name : null,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentVersionListItemViewModel>> GetDocumentVersionsAsync(Guid companyId)
        {
            return await _dbContext.DocumentVersions
                .AsNoTracking()
                .Where(x => !x.IsDeleted && (x.Document.IssuedByCompanyId == companyId || x.Document.Shipment.CustomerCompanyId == companyId))
                .OrderByDescending(x => x.CreatedOnUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new DocumentVersionListItemViewModel
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    DocumentNo = x.Document.DocumentNo ?? x.Document.Shipment.ShipmentNo,
                    FileName = x.FileResource.FileName,
                    VersionNo = x.VersionNo,
                    CreatedOnUtc = x.CreatedOnUtc,
                    ChangeDescription = x.ChangeDescription
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentTemplateListItemViewModel>> GetDocumentTemplatesAsync(Guid companyId)
        {
            return await _dbContext.DocumentTemplates
                .AsNoTracking()
                .Where(x => !x.IsDeleted && (x.CompanyId == companyId || x.CompanyId == null))
                .OrderByDescending(x => x.CompanyId == companyId)
                .ThenBy(x => x.Name)
                .Select(x => new DocumentTemplateListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    TemplateType = x.TemplateType.ToString(),
                    FileName = x.FileResource.FileName,
                    IsDefault = x.IsDefault,
                    IsActive = x.IsActive,
                    IsGlobal = x.CompanyId == null,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<DocumentCreateViewModel> GetCreateDocumentModelAsync(Guid companyId)
        {
            var model = new DocumentCreateViewModel();
            await PopulateDocumentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<DocumentEditViewModel?> GetDocumentForEditAsync(Guid companyId, Guid documentId)
        {
            var model = await _dbContext.Documents
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == documentId && (x.IssuedByCompanyId == companyId || x.Shipment.CustomerCompanyId == companyId))
                .Select(x => new DocumentEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    DocumentType = x.DocumentType,
                    FileResourceId = x.FileResourceId,
                    DocumentNo = x.DocumentNo,
                    IssuedAtUtc = x.IssuedAtUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateDocumentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateDocumentAsync(Guid companyId, DocumentCreateViewModel model)
        {
            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!shipmentExists || !fileExists)
            {
                return null;
            }

            var entity = new Document
            {
                ShipmentId = model.ShipmentId,
                DocumentType = model.DocumentType,
                FileResourceId = model.FileResourceId,
                DocumentNo = string.IsNullOrWhiteSpace(model.DocumentNo) ? null : model.DocumentNo.Trim(),
                IssuedAtUtc = model.IssuedAtUtc,
                IssuedByCompanyId = companyId,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.Documents.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateDocumentAsync(Guid companyId, DocumentEditViewModel model)
        {
            var entity = await _dbContext.Documents
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.IssuedByCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!shipmentExists || !fileExists)
            {
                return false;
            }

            entity.ShipmentId = model.ShipmentId;
            entity.DocumentType = model.DocumentType;
            entity.FileResourceId = model.FileResourceId;
            entity.DocumentNo = string.IsNullOrWhiteSpace(model.DocumentNo) ? null : model.DocumentNo.Trim();
            entity.IssuedAtUtc = model.IssuedAtUtc;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDocumentAsync(Guid companyId, Guid documentId)
        {
            var entity = await _dbContext.Documents
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == documentId && x.IssuedByCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentVersionCreateViewModel> GetCreateDocumentVersionModelAsync(Guid companyId)
        {
            var model = new DocumentVersionCreateViewModel();
            await PopulateDocumentVersionOptionsAsync(companyId, model);
            return model;
        }

        public async Task<DocumentVersionEditViewModel?> GetDocumentVersionForEditAsync(Guid companyId, Guid versionId)
        {
            var model = await _dbContext.DocumentVersions
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == versionId && (x.Document.IssuedByCompanyId == companyId || x.Document.Shipment.CustomerCompanyId == companyId))
                .Select(x => new DocumentVersionEditViewModel
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    FileResourceId = x.FileResourceId,
                    VersionNo = x.VersionNo,
                    CreatedOnUtc = x.CreatedOnUtc,
                    ChangeDescription = x.ChangeDescription
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateDocumentVersionOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateDocumentVersionAsync(Guid companyId, DocumentVersionCreateViewModel model)
        {
            var documentExists = await _dbContext.Documents
                .AnyAsync(x => !x.IsDeleted && x.Id == model.DocumentId && (x.IssuedByCompanyId == companyId || x.Shipment.CustomerCompanyId == companyId));

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!documentExists || !fileExists)
            {
                return null;
            }

            var entity = new DocumentVersion
            {
                DocumentId = model.DocumentId,
                FileResourceId = model.FileResourceId,
                VersionNo = model.VersionNo,
                CreatedOnUtc = model.CreatedOnUtc,
                ChangeDescription = string.IsNullOrWhiteSpace(model.ChangeDescription) ? null : model.ChangeDescription.Trim()
            };

            _dbContext.DocumentVersions.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateDocumentVersionAsync(Guid companyId, DocumentVersionEditViewModel model)
        {
            var entity = await _dbContext.DocumentVersions
                .Include(x => x.Document)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && (x.Document.IssuedByCompanyId == companyId || x.Document.Shipment.CustomerCompanyId == companyId));

            if (entity == null)
            {
                return false;
            }

            var documentExists = await _dbContext.Documents
                .AnyAsync(x => !x.IsDeleted && x.Id == model.DocumentId && (x.IssuedByCompanyId == companyId || x.Shipment.CustomerCompanyId == companyId));

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!documentExists || !fileExists)
            {
                return false;
            }

            entity.DocumentId = model.DocumentId;
            entity.FileResourceId = model.FileResourceId;
            entity.VersionNo = model.VersionNo;
            entity.CreatedOnUtc = model.CreatedOnUtc;
            entity.ChangeDescription = string.IsNullOrWhiteSpace(model.ChangeDescription) ? null : model.ChangeDescription.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDocumentVersionAsync(Guid companyId, Guid versionId)
        {
            var entity = await _dbContext.DocumentVersions
                .Include(x => x.Document)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == versionId && (x.Document.IssuedByCompanyId == companyId || x.Document.Shipment.CustomerCompanyId == companyId));

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentTemplateCreateViewModel> GetCreateDocumentTemplateModelAsync(Guid companyId)
        {
            var model = new DocumentTemplateCreateViewModel();
            await PopulateDocumentTemplateOptionsAsync(companyId, model);
            return model;
        }

        public async Task<DocumentTemplateEditViewModel?> GetDocumentTemplateForEditAsync(Guid companyId, Guid templateId)
        {
            var model = await _dbContext.DocumentTemplates
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == templateId && x.CompanyId == companyId)
                .Select(x => new DocumentTemplateEditViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    TemplateType = x.TemplateType,
                    FileResourceId = x.FileResourceId,
                    IsDefault = x.IsDefault,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateDocumentTemplateOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateDocumentTemplateAsync(Guid companyId, DocumentTemplateCreateViewModel model)
        {
            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!fileExists)
            {
                return null;
            }

            var entity = new DocumentTemplate
            {
                Name = model.Name.Trim(),
                TemplateType = model.TemplateType,
                CompanyId = companyId,
                FileResourceId = model.FileResourceId,
                IsDefault = model.IsDefault,
                IsActive = model.IsActive,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.DocumentTemplates.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateDocumentTemplateAsync(Guid companyId, DocumentTemplateEditViewModel model)
        {
            var entity = await _dbContext.DocumentTemplates
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!fileExists)
            {
                return false;
            }

            entity.Name = model.Name.Trim();
            entity.TemplateType = model.TemplateType;
            entity.FileResourceId = model.FileResourceId;
            entity.IsDefault = model.IsDefault;
            entity.IsActive = model.IsActive;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDocumentTemplateAsync(Guid companyId, Guid templateId)
        {
            var entity = await _dbContext.DocumentTemplates
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == templateId && x.CompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Compliance

        public async Task<IEnumerable<DangerousGoodsDeclarationListItemViewModel>> GetDangerousGoodsDeclarationsAsync(Guid companyId)
        {
            return await _dbContext.DangerousGoodsDeclarations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new DangerousGoodsDeclarationListItemViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    PackageNo = x.Package != null ? x.Package.PackageNo : null,
                    UnNumber = x.UnNumber,
                    ProperShippingName = x.ProperShippingName,
                    HazardClass = x.HazardClass.ToString(),
                    PackingGroup = x.PackingGroup.HasValue ? x.PackingGroup.Value.ToString() : null,
                    NetQuantity = x.NetQuantity,
                    QuantityUnit = x.QuantityUnit,
                    RequiresSpecialHandling = x.RequiresSpecialHandling,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TemperatureRequirementListItemViewModel>> GetTemperatureRequirementsAsync(Guid companyId)
        {
            return await _dbContext.TemperatureRequirements
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new TemperatureRequirementListItemViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    MinTemperatureCelsius = x.MinTemperatureCelsius,
                    MaxTemperatureCelsius = x.MaxTemperatureCelsius,
                    RequiresTemperatureMonitoring = x.RequiresTemperatureMonitoring,
                    TemperatureUnit = x.TemperatureUnit,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ComplianceCheckListItemViewModel>> GetComplianceChecksAsync(Guid companyId)
        {
            return await _dbContext.ComplianceChecks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CheckedAtUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new ComplianceCheckListItemViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    CheckType = x.CheckType,
                    Status = x.Status.ToString(),
                    CheckedAtUtc = x.CheckedAtUtc,
                    CheckedBy = x.CheckedBy,
                    ResultDetails = x.ResultDetails,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DGDocumentListItemViewModel>> GetDGDocumentsAsync(Guid companyId)
        {
            return await _dbContext.DGDocuments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.DangerousGoodsDeclaration.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new DGDocumentListItemViewModel
                {
                    Id = x.Id,
                    DangerousGoodsDeclarationId = x.DangerousGoodsDeclarationId,
                    ShipmentNo = x.DangerousGoodsDeclaration.Shipment.ShipmentNo,
                    UnNumber = x.DangerousGoodsDeclaration.UnNumber,
                    FileName = x.FileResource.FileName,
                    DocumentName = x.DocumentName,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<DangerousGoodsDeclarationCreateViewModel> GetCreateDangerousGoodsDeclarationModelAsync(Guid companyId)
        {
            var model = new DangerousGoodsDeclarationCreateViewModel();
            await PopulateDangerousGoodsDeclarationOptionsAsync(companyId, model);
            return model;
        }

        public async Task<DangerousGoodsDeclarationEditViewModel?> GetDangerousGoodsDeclarationForEditAsync(Guid companyId, Guid declarationId)
        {
            var model = await _dbContext.DangerousGoodsDeclarations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == declarationId && x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new DangerousGoodsDeclarationEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    PackageId = x.PackageId,
                    UnNumber = x.UnNumber,
                    ProperShippingName = x.ProperShippingName,
                    HazardClass = x.HazardClass,
                    PackingGroup = x.PackingGroup,
                    NetQuantity = x.NetQuantity,
                    QuantityUnit = x.QuantityUnit,
                    HandlingInstructions = x.HandlingInstructions,
                    RequiresSpecialHandling = x.RequiresSpecialHandling,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateDangerousGoodsDeclarationOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateDangerousGoodsDeclarationAsync(Guid companyId, DangerousGoodsDeclarationCreateViewModel model)
        {
            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            if (!shipmentExists)
            {
                return null;
            }

            if (model.PackageId.HasValue)
            {
                var packageExists = await _dbContext.Packages
                    .AnyAsync(x =>
                        !x.IsDeleted &&
                        x.Id == model.PackageId.Value &&
                        x.Shipment.CustomerCompanyId == companyId &&
                        x.ShipmentId == model.ShipmentId);

                if (!packageExists)
                {
                    return null;
                }
            }

            var entity = new DangerousGoodsDeclaration
            {
                ShipmentId = model.ShipmentId,
                PackageId = model.PackageId,
                UnNumber = model.UnNumber.Trim(),
                ProperShippingName = model.ProperShippingName.Trim(),
                HazardClass = model.HazardClass,
                PackingGroup = model.PackingGroup,
                NetQuantity = model.NetQuantity,
                QuantityUnit = string.IsNullOrWhiteSpace(model.QuantityUnit) ? null : model.QuantityUnit.Trim(),
                HandlingInstructions = string.IsNullOrWhiteSpace(model.HandlingInstructions) ? null : model.HandlingInstructions.Trim(),
                RequiresSpecialHandling = model.RequiresSpecialHandling,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.DangerousGoodsDeclarations.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateDangerousGoodsDeclarationAsync(Guid companyId, DangerousGoodsDeclarationEditViewModel model)
        {
            var entity = await _dbContext.DangerousGoodsDeclarations
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            if (!shipmentExists)
            {
                return false;
            }

            if (model.PackageId.HasValue)
            {
                var packageExists = await _dbContext.Packages
                    .AnyAsync(x =>
                        !x.IsDeleted &&
                        x.Id == model.PackageId.Value &&
                        x.Shipment.CustomerCompanyId == companyId &&
                        x.ShipmentId == model.ShipmentId);

                if (!packageExists)
                {
                    return false;
                }
            }

            entity.ShipmentId = model.ShipmentId;
            entity.PackageId = model.PackageId;
            entity.UnNumber = model.UnNumber.Trim();
            entity.ProperShippingName = model.ProperShippingName.Trim();
            entity.HazardClass = model.HazardClass;
            entity.PackingGroup = model.PackingGroup;
            entity.NetQuantity = model.NetQuantity;
            entity.QuantityUnit = string.IsNullOrWhiteSpace(model.QuantityUnit) ? null : model.QuantityUnit.Trim();
            entity.HandlingInstructions = string.IsNullOrWhiteSpace(model.HandlingInstructions) ? null : model.HandlingInstructions.Trim();
            entity.RequiresSpecialHandling = model.RequiresSpecialHandling;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDangerousGoodsDeclarationAsync(Guid companyId, Guid declarationId)
        {
            var entity = await _dbContext.DangerousGoodsDeclarations
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == declarationId && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TemperatureRequirementCreateViewModel> GetCreateTemperatureRequirementModelAsync(Guid companyId)
        {
            var model = new TemperatureRequirementCreateViewModel();
            await PopulateTemperatureRequirementOptionsAsync(companyId, model);
            return model;
        }

        public async Task<TemperatureRequirementEditViewModel?> GetTemperatureRequirementForEditAsync(Guid companyId, Guid requirementId)
        {
            var model = await _dbContext.TemperatureRequirements
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == requirementId && x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new TemperatureRequirementEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    MinTemperatureCelsius = x.MinTemperatureCelsius,
                    MaxTemperatureCelsius = x.MaxTemperatureCelsius,
                    RequiresTemperatureMonitoring = x.RequiresTemperatureMonitoring,
                    TemperatureUnit = x.TemperatureUnit,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateTemperatureRequirementOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateTemperatureRequirementAsync(Guid companyId, TemperatureRequirementCreateViewModel model)
        {
            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            if (!shipmentExists)
            {
                return null;
            }

            var entity = new TemperatureRequirement
            {
                ShipmentId = model.ShipmentId,
                MinTemperatureCelsius = model.MinTemperatureCelsius,
                MaxTemperatureCelsius = model.MaxTemperatureCelsius,
                RequiresTemperatureMonitoring = model.RequiresTemperatureMonitoring,
                TemperatureUnit = string.IsNullOrWhiteSpace(model.TemperatureUnit) ? null : model.TemperatureUnit.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.TemperatureRequirements.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateTemperatureRequirementAsync(Guid companyId, TemperatureRequirementEditViewModel model)
        {
            var entity = await _dbContext.TemperatureRequirements
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            if (!shipmentExists)
            {
                return false;
            }

            entity.ShipmentId = model.ShipmentId;
            entity.MinTemperatureCelsius = model.MinTemperatureCelsius;
            entity.MaxTemperatureCelsius = model.MaxTemperatureCelsius;
            entity.RequiresTemperatureMonitoring = model.RequiresTemperatureMonitoring;
            entity.TemperatureUnit = string.IsNullOrWhiteSpace(model.TemperatureUnit) ? null : model.TemperatureUnit.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTemperatureRequirementAsync(Guid companyId, Guid requirementId)
        {
            var entity = await _dbContext.TemperatureRequirements
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == requirementId && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ComplianceCheckCreateViewModel> GetCreateComplianceCheckModelAsync(Guid companyId)
        {
            var model = new ComplianceCheckCreateViewModel();
            await PopulateComplianceCheckOptionsAsync(companyId, model);
            return model;
        }

        public async Task<ComplianceCheckEditViewModel?> GetComplianceCheckForEditAsync(Guid companyId, Guid complianceCheckId)
        {
            var model = await _dbContext.ComplianceChecks
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == complianceCheckId && x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new ComplianceCheckEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    CheckType = x.CheckType,
                    Status = x.Status,
                    CheckedAtUtc = x.CheckedAtUtc,
                    CheckedBy = x.CheckedBy,
                    ResultDetails = x.ResultDetails,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateComplianceCheckOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateComplianceCheckAsync(Guid companyId, ComplianceCheckCreateViewModel model)
        {
            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            if (!shipmentExists)
            {
                return null;
            }

            var entity = new ComplianceCheck
            {
                ShipmentId = model.ShipmentId,
                CheckType = model.CheckType.Trim(),
                Status = model.Status,
                CheckedAtUtc = model.CheckedAtUtc,
                CheckedBy = string.IsNullOrWhiteSpace(model.CheckedBy) ? null : model.CheckedBy.Trim(),
                ResultDetails = string.IsNullOrWhiteSpace(model.ResultDetails) ? null : model.ResultDetails.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.ComplianceChecks.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateComplianceCheckAsync(Guid companyId, ComplianceCheckEditViewModel model)
        {
            var entity = await _dbContext.ComplianceChecks
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);

            if (!shipmentExists)
            {
                return false;
            }

            entity.ShipmentId = model.ShipmentId;
            entity.CheckType = model.CheckType.Trim();
            entity.Status = model.Status;
            entity.CheckedAtUtc = model.CheckedAtUtc;
            entity.CheckedBy = string.IsNullOrWhiteSpace(model.CheckedBy) ? null : model.CheckedBy.Trim();
            entity.ResultDetails = string.IsNullOrWhiteSpace(model.ResultDetails) ? null : model.ResultDetails.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteComplianceCheckAsync(Guid companyId, Guid complianceCheckId)
        {
            var entity = await _dbContext.ComplianceChecks
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == complianceCheckId && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<DGDocumentCreateViewModel> GetCreateDGDocumentModelAsync(Guid companyId)
        {
            var model = new DGDocumentCreateViewModel();
            await PopulateDGDocumentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<DGDocumentEditViewModel?> GetDGDocumentForEditAsync(Guid companyId, Guid dgDocumentId)
        {
            var model = await _dbContext.DGDocuments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == dgDocumentId && x.DangerousGoodsDeclaration.Shipment.CustomerCompanyId == companyId)
                .Select(x => new DGDocumentEditViewModel
                {
                    Id = x.Id,
                    DangerousGoodsDeclarationId = x.DangerousGoodsDeclarationId,
                    FileResourceId = x.FileResourceId,
                    DocumentName = x.DocumentName,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateDGDocumentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateDGDocumentAsync(Guid companyId, DGDocumentCreateViewModel model)
        {
            var declarationExists = await _dbContext.DangerousGoodsDeclarations
                .AnyAsync(x => !x.IsDeleted && x.Id == model.DangerousGoodsDeclarationId && x.Shipment.CustomerCompanyId == companyId);

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!declarationExists || !fileExists)
            {
                return null;
            }

            var entity = new DGDocument
            {
                DangerousGoodsDeclarationId = model.DangerousGoodsDeclarationId,
                FileResourceId = model.FileResourceId,
                DocumentName = string.IsNullOrWhiteSpace(model.DocumentName) ? null : model.DocumentName.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.DGDocuments.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateDGDocumentAsync(Guid companyId, DGDocumentEditViewModel model)
        {
            var entity = await _dbContext.DGDocuments
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.DangerousGoodsDeclaration.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var declarationExists = await _dbContext.DangerousGoodsDeclarations
                .AnyAsync(x => !x.IsDeleted && x.Id == model.DangerousGoodsDeclarationId && x.Shipment.CustomerCompanyId == companyId);

            var fileExists = await _dbContext.FileResources
                .AnyAsync(x => !x.IsDeleted && x.Id == model.FileResourceId);

            if (!declarationExists || !fileExists)
            {
                return false;
            }

            entity.DangerousGoodsDeclarationId = model.DangerousGoodsDeclarationId;
            entity.FileResourceId = model.FileResourceId;
            entity.DocumentName = string.IsNullOrWhiteSpace(model.DocumentName) ? null : model.DocumentName.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDGDocumentAsync(Guid companyId, Guid dgDocumentId)
        {
            var entity = await _dbContext.DGDocuments
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == dgDocumentId && x.DangerousGoodsDeclaration.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Document/Compliance option helpers

        private async Task PopulateDocumentOptionsAsync(Guid companyId, DocumentCreateViewModel model)
        {
            model.ShipmentOptions = await GetShipmentSelectOptionsAsync(companyId);
            model.FileResourceOptions = await GetFileResourceSelectOptionsAsync();
        }

        private async Task PopulateDocumentVersionOptionsAsync(Guid companyId, DocumentVersionCreateViewModel model)
        {
            model.DocumentOptions = await _dbContext.Documents
                .AsNoTracking()
                .Where(x => !x.IsDeleted && (x.IssuedByCompanyId == companyId || x.Shipment.CustomerCompanyId == companyId))
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = (x.DocumentNo ?? x.Shipment.ShipmentNo) + " / " + x.DocumentType
                })
                .ToListAsync();

            model.FileResourceOptions = await GetFileResourceSelectOptionsAsync();
        }

        private async Task PopulateDocumentTemplateOptionsAsync(Guid companyId, DocumentTemplateCreateViewModel model)
        {
            model.FileResourceOptions = await GetFileResourceSelectOptionsAsync();
        }

        private async Task PopulateDangerousGoodsDeclarationOptionsAsync(Guid companyId, DangerousGoodsDeclarationCreateViewModel model)
        {
            model.ShipmentOptions = await GetShipmentSelectOptionsAsync(companyId);
            model.PackageOptions = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Shipment.ShipmentNo + " / " + x.PackageNo
                })
                .ToListAsync();
        }

        private async Task PopulateTemperatureRequirementOptionsAsync(Guid companyId, TemperatureRequirementCreateViewModel model) =>
            model.ShipmentOptions = await GetShipmentSelectOptionsAsync(companyId);

        private async Task PopulateComplianceCheckOptionsAsync(Guid companyId, ComplianceCheckCreateViewModel model) =>
            model.ShipmentOptions = await GetShipmentSelectOptionsAsync(companyId);

        private async Task PopulateDGDocumentOptionsAsync(Guid companyId, DGDocumentCreateViewModel model)
        {
            model.DangerousGoodsDeclarationOptions = await _dbContext.DangerousGoodsDeclarations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Shipment.ShipmentNo + " / " + x.UnNumber + " / " + x.ProperShippingName
                })
                .ToListAsync();

            model.FileResourceOptions = await GetFileResourceSelectOptionsAsync();
        }

        private async Task<List<SelectListItem>> GetShipmentSelectOptionsAsync(Guid companyId)
        {
            return await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShipmentNo
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetFileResourceSelectOptionsAsync()
        {
            return await _dbContext.FileResources
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.UploadedAtUtc)
                .Take(500)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FileName + " (" + x.ContentType + ")"
                })
                .ToListAsync();
        }

        #endregion
    }
}