using LogisticManagementApp.Models.CompanyPortal.Compliance;
using LogisticManagementApp.Models.CompanyPortal.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Company")]
    public partial class CompanyController : Controller
    {
        #region Documents

        [HttpGet]
        public async Task<IActionResult> FileResources()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetFileResourcesAsync(companyId.Value);
            return View("~/Views/CompanyPortal/Documents/FileResources.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Documents()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDocumentsAsync(companyId.Value);
            return View("~/Views/CompanyPortal/Documents/Documents.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> DocumentVersions()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDocumentVersionsAsync(companyId.Value);
            return View("~/Views/CompanyPortal/Documents/DocumentVersions.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> DocumentTemplates()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDocumentTemplatesAsync(companyId.Value);
            return View("~/Views/CompanyPortal/Documents/DocumentTemplates.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDocument()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetCreateDocumentModelAsync(companyId.Value);
            return View("~/Views/CompanyPortal/Documents/CreateDocument.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDocument(DocumentCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateDocumentModelAsync(companyId.Value);
                CopyDocument(model, reload);

                return View("~/Views/CompanyPortal/Documents/CreateDocument.cshtml", reload);
            }

            var id = await _companyPortalService.CreateDocumentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на document.";

                var reload = await _companyPortalService.GetCreateDocumentModelAsync(companyId.Value);
                CopyDocument(model, reload);

                return View("~/Views/CompanyPortal/Documents/CreateDocument.cshtml", reload);
            }

            TempData["SuccessMessage"] = "Document беше добавен успешно.";
            return RedirectToAction(nameof(Documents));
        }

        [HttpGet]
        public async Task<IActionResult> EditDocument(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDocumentForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Documents/EditDocument.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocument(DocumentEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetDocumentForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyDocument(model, reload);
                    model = reload;
                }

                return View("~/Views/CompanyPortal/Documents/EditDocument.cshtml", model);
            }

            var success = await _companyPortalService.UpdateDocumentAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на document.";
                return View("~/Views/CompanyPortal/Documents/EditDocument.cshtml", model);
            }

            TempData["SuccessMessage"] = "Document беше обновен успешно.";
            return RedirectToAction(nameof(Documents));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteDocumentAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Document беше изтрит успешно." : "Неуспешно изтриване на document.";

            return RedirectToAction(nameof(Documents));
        }

        [HttpGet]
        public async Task<IActionResult> CreateDocumentVersion()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetCreateDocumentVersionModelAsync(companyId.Value);
            return View("~/Views/CompanyPortal/Documents/CreateDocumentVersion.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDocumentVersion(DocumentVersionCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateDocumentVersionModelAsync(
                    companyId.Value
                );
                CopyDocumentVersion(model, reload);

                return View("~/Views/CompanyPortal/Documents/CreateDocumentVersion.cshtml", reload);
            }

            var id = await _companyPortalService.CreateDocumentVersionAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на document version.";

                var reload = await _companyPortalService.GetCreateDocumentVersionModelAsync(
                    companyId.Value
                );
                CopyDocumentVersion(model, reload);

                return View("~/Views/CompanyPortal/Documents/CreateDocumentVersion.cshtml", reload);
            }

            TempData["SuccessMessage"] = "Document version беше добавена успешно.";
            return RedirectToAction(nameof(DocumentVersions));
        }

        [HttpGet]
        public async Task<IActionResult> EditDocumentVersion(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDocumentVersionForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Documents/EditDocumentVersion.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocumentVersion(DocumentVersionEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetDocumentVersionForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyDocumentVersion(model, reload);
                    model = reload;
                }

                return View("~/Views/CompanyPortal/Documents/EditDocumentVersion.cshtml", model);
            }

            var success = await _companyPortalService.UpdateDocumentVersionAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на document version.";
                return View("~/Views/CompanyPortal/Documents/EditDocumentVersion.cshtml", model);
            }

            TempData["SuccessMessage"] = "Document version беше обновена успешно.";
            return RedirectToAction(nameof(DocumentVersions));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocumentVersion(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteDocumentVersionAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Document version беше изтрита успешно."
                    : "Неуспешно изтриване на document version.";

            return RedirectToAction(nameof(DocumentVersions));
        }

        [HttpGet]
        public async Task<IActionResult> CreateDocumentTemplate()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetCreateDocumentTemplateModelAsync(
                companyId.Value
            );
            return View("~/Views/CompanyPortal/Documents/CreateDocumentTemplate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDocumentTemplate(DocumentTemplateCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateDocumentTemplateModelAsync(
                    companyId.Value
                );
                CopyDocumentTemplate(model, reload);

                return View("~/Views/CompanyPortal/Documents/CreateDocumentTemplate.cshtml", reload);
            }

            var id = await _companyPortalService.CreateDocumentTemplateAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на template.";

                var reload = await _companyPortalService.GetCreateDocumentTemplateModelAsync(
                    companyId.Value
                );
                CopyDocumentTemplate(model, reload);

                return View("~/Views/CompanyPortal/Documents/CreateDocumentTemplate.cshtml", reload);
            }

            TempData["SuccessMessage"] = "Document template беше добавен успешно.";
            return RedirectToAction(nameof(DocumentTemplates));
        }

        [HttpGet]
        public async Task<IActionResult> EditDocumentTemplate(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDocumentTemplateForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Documents/EditDocumentTemplate.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocumentTemplate(DocumentTemplateEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetDocumentTemplateForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyDocumentTemplate(model, reload);
                    model = reload;
                }

                return View("~/Views/CompanyPortal/Documents/EditDocumentTemplate.cshtml", model);
            }

            var success = await _companyPortalService.UpdateDocumentTemplateAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на template.";
                return View("~/Views/CompanyPortal/Documents/EditDocumentTemplate.cshtml", model);
            }

            TempData["SuccessMessage"] = "Document template беше обновен успешно.";
            return RedirectToAction(nameof(DocumentTemplates));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocumentTemplate(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteDocumentTemplateAsync(
                companyId.Value,
                id
            );
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Document template беше изтрит успешно."
                    : "Неуспешно изтриване на template.";

            return RedirectToAction(nameof(DocumentTemplates));
        }

        #endregion

        #region Compliance

        [HttpGet]
        public async Task<IActionResult> DangerousGoodsDeclarations()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/DangerousGoodsDeclarations.cshtml",
                await _companyPortalService.GetDangerousGoodsDeclarationsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> TemperatureRequirements()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/TemperatureRequirements.cshtml",
                await _companyPortalService.GetTemperatureRequirementsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> ComplianceChecks()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/ComplianceChecks.cshtml",
                await _companyPortalService.GetComplianceChecksAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> DGDocuments()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/DGDocuments.cshtml",
                await _companyPortalService.GetDGDocumentsAsync(companyId.Value)
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreateDangerousGoodsDeclaration()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/CreateDangerousGoodsDeclaration.cshtml",
                await _companyPortalService.GetCreateDangerousGoodsDeclarationModelAsync(
                    companyId.Value
                )
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDangerousGoodsDeclaration(
            DangerousGoodsDeclarationCreateViewModel model
        )
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload =
                    await _companyPortalService.GetCreateDangerousGoodsDeclarationModelAsync(
                        companyId.Value
                    );
                CopyDangerousGoodsDeclaration(model, reload);

                return View(
                    "~/Views/CompanyPortal/Compliance/CreateDangerousGoodsDeclaration.cshtml",
                    reload
                );
            }

            var id = await _companyPortalService.CreateDangerousGoodsDeclarationAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на DG declaration.";

                var reload =
                    await _companyPortalService.GetCreateDangerousGoodsDeclarationModelAsync(
                        companyId.Value
                    );
                CopyDangerousGoodsDeclaration(model, reload);

                return View(
                    "~/Views/CompanyPortal/Compliance/CreateDangerousGoodsDeclaration.cshtml",
                    reload
                );
            }

            TempData["SuccessMessage"] = "DG declaration беше добавена успешно.";
            return RedirectToAction(nameof(DangerousGoodsDeclarations));
        }

        [HttpGet]
        public async Task<IActionResult> EditDangerousGoodsDeclaration(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDangerousGoodsDeclarationForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Compliance/EditDangerousGoodsDeclaration.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDangerousGoodsDeclaration(
            DangerousGoodsDeclarationEditViewModel model
        )
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetDangerousGoodsDeclarationForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyDangerousGoodsDeclaration(model, reload);
                    model = reload;
                }

                return View(
                    "~/Views/CompanyPortal/Compliance/EditDangerousGoodsDeclaration.cshtml",
                    model
                );
            }

            var success = await _companyPortalService.UpdateDangerousGoodsDeclarationAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на DG declaration.";
                return View(
                    "~/Views/CompanyPortal/Compliance/EditDangerousGoodsDeclaration.cshtml",
                    model
                );
            }

            TempData["SuccessMessage"] = "DG declaration беше обновена успешно.";
            return RedirectToAction(nameof(DangerousGoodsDeclarations));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDangerousGoodsDeclaration(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteDangerousGoodsDeclarationAsync(
                companyId.Value,
                id
            );
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "DG declaration беше изтрита успешно."
                    : "Неуспешно изтриване на DG declaration.";

            return RedirectToAction(nameof(DangerousGoodsDeclarations));
        }

        [HttpGet]
        public async Task<IActionResult> CreateTemperatureRequirement()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/CreateTemperatureRequirement.cshtml",
                await _companyPortalService.GetCreateTemperatureRequirementModelAsync(
                    companyId.Value
                )
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemperatureRequirement(
            TemperatureRequirementCreateViewModel model
        )
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateTemperatureRequirementModelAsync(
                    companyId.Value
                );
                CopyTemperatureRequirement(model, reload);

                return View(
                    "~/Views/CompanyPortal/Compliance/CreateTemperatureRequirement.cshtml",
                    reload
                );
            }

            var id = await _companyPortalService.CreateTemperatureRequirementAsync(
                companyId.Value,
                model
            );

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на temperature requirement.";

                var reload = await _companyPortalService.GetCreateTemperatureRequirementModelAsync(
                    companyId.Value
                );
                CopyTemperatureRequirement(model, reload);

                return View(
                    "~/Views/CompanyPortal/Compliance/CreateTemperatureRequirement.cshtml",
                    reload
                );
            }

            TempData["SuccessMessage"] = "Temperature requirement беше добавен успешно.";
            return RedirectToAction(nameof(TemperatureRequirements));
        }

        [HttpGet]
        public async Task<IActionResult> EditTemperatureRequirement(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetTemperatureRequirementForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Compliance/EditTemperatureRequirement.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTemperatureRequirement(
            TemperatureRequirementEditViewModel model
        )
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetTemperatureRequirementForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyTemperatureRequirement(model, reload);
                    model = reload;
                }

                return View("~/Views/CompanyPortal/Compliance/EditTemperatureRequirement.cshtml", model);
            }

            var success = await _companyPortalService.UpdateTemperatureRequirementAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на temperature requirement.";
                return View("~/Views/CompanyPortal/Compliance/EditTemperatureRequirement.cshtml", model);
            }

            TempData["SuccessMessage"] = "Temperature requirement беше обновен успешно.";
            return RedirectToAction(nameof(TemperatureRequirements));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTemperatureRequirement(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteTemperatureRequirementAsync(
                companyId.Value,
                id
            );
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Temperature requirement беше изтрит успешно."
                    : "Неуспешно изтриване на temperature requirement.";

            return RedirectToAction(nameof(TemperatureRequirements));
        }

        [HttpGet]
        public async Task<IActionResult> CreateComplianceCheck()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/CreateComplianceCheck.cshtml",
                await _companyPortalService.GetCreateComplianceCheckModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComplianceCheck(ComplianceCheckCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateComplianceCheckModelAsync(
                    companyId.Value
                );
                CopyComplianceCheck(model, reload);

                return View("~/Views/CompanyPortal/Compliance/CreateComplianceCheck.cshtml", reload);
            }

            var id = await _companyPortalService.CreateComplianceCheckAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на compliance check.";

                var reload = await _companyPortalService.GetCreateComplianceCheckModelAsync(
                    companyId.Value
                );
                CopyComplianceCheck(model, reload);

                return View("~/Views/CompanyPortal/Compliance/CreateComplianceCheck.cshtml", reload);
            }

            TempData["SuccessMessage"] = "Compliance check беше добавен успешно.";
            return RedirectToAction(nameof(ComplianceChecks));
        }

        [HttpGet]
        public async Task<IActionResult> EditComplianceCheck(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetComplianceCheckForEditAsync(
                companyId.Value,
                id
            );
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Compliance/EditComplianceCheck.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComplianceCheck(ComplianceCheckEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetComplianceCheckForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyComplianceCheck(model, reload);
                    model = reload;
                }

                return View("~/Views/CompanyPortal/Compliance/EditComplianceCheck.cshtml", model);
            }

            var success = await _companyPortalService.UpdateComplianceCheckAsync(
                companyId.Value,
                model
            );

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на compliance check.";
                return View("~/Views/CompanyPortal/Compliance/EditComplianceCheck.cshtml", model);
            }

            TempData["SuccessMessage"] = "Compliance check беше обновен успешно.";
            return RedirectToAction(nameof(ComplianceChecks));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComplianceCheck(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteComplianceCheckAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "Compliance check беше изтрит успешно."
                    : "Неуспешно изтриване на compliance check.";

            return RedirectToAction(nameof(ComplianceChecks));
        }

        [HttpGet]
        public async Task<IActionResult> CreateDGDocument()
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            return View(
                "~/Views/CompanyPortal/Compliance/CreateDGDocument.cshtml",
                await _companyPortalService.GetCreateDGDocumentModelAsync(companyId.Value)
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDGDocument(DGDocumentCreateViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetCreateDGDocumentModelAsync(
                    companyId.Value
                );
                CopyDGDocument(model, reload);

                return View("~/Views/CompanyPortal/Compliance/CreateDGDocument.cshtml", reload);
            }

            var id = await _companyPortalService.CreateDGDocumentAsync(companyId.Value, model);

            if (id == null)
            {
                TempData["ErrorMessage"] = "Неуспешно добавяне на DG document.";

                var reload = await _companyPortalService.GetCreateDGDocumentModelAsync(
                    companyId.Value
                );
                CopyDGDocument(model, reload);

                return View("~/Views/CompanyPortal/Compliance/CreateDGDocument.cshtml", reload);
            }

            TempData["SuccessMessage"] = "DG document беше добавен успешно.";
            return RedirectToAction(nameof(DGDocuments));
        }

        [HttpGet]
        public async Task<IActionResult> EditDGDocument(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var model = await _companyPortalService.GetDGDocumentForEditAsync(companyId.Value, id);
            if (model == null) return NotFound();

            return View("~/Views/CompanyPortal/Compliance/EditDGDocument.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDGDocument(DGDocumentEditViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                var reload = await _companyPortalService.GetDGDocumentForEditAsync(
                    companyId.Value,
                    model.Id
                );

                if (reload != null)
                {
                    CopyDGDocument(model, reload);
                    model = reload;
                }

                return View("~/Views/CompanyPortal/Compliance/EditDGDocument.cshtml", model);
            }

            var success = await _companyPortalService.UpdateDGDocumentAsync(companyId.Value, model);

            if (!success)
            {
                TempData["ErrorMessage"] = "Неуспешно обновяване на DG document.";
                return View("~/Views/CompanyPortal/Compliance/EditDGDocument.cshtml", model);
            }

            TempData["SuccessMessage"] = "DG document беше обновен успешно.";
            return RedirectToAction(nameof(DGDocuments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDGDocument(Guid id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var success = await _companyPortalService.DeleteDGDocumentAsync(companyId.Value, id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "DG document беше изтрит успешно." : "Неуспешно изтриване на DG document.";

            return RedirectToAction(nameof(DGDocuments));
        }

        #endregion

        #region Copy helpers

        private static void CopyDocument( DocumentCreateViewModel source, DocumentCreateViewModel target)
        {
            target.ShipmentId = source.ShipmentId;
            target.DocumentType = source.DocumentType;
            target.FileResourceId = source.FileResourceId;
            target.DocumentNo = source.DocumentNo;
            target.IssuedAtUtc = source.IssuedAtUtc;
            target.Notes = source.Notes;
        }

        private static void CopyDocumentVersion( DocumentVersionCreateViewModel source, DocumentVersionCreateViewModel target)
        {
            target.DocumentId = source.DocumentId;
            target.FileResourceId = source.FileResourceId;
            target.VersionNo = source.VersionNo;
            target.CreatedOnUtc = source.CreatedOnUtc;
            target.ChangeDescription = source.ChangeDescription;
        }

        private static void CopyDocumentTemplate( DocumentTemplateCreateViewModel source,DocumentTemplateCreateViewModel target)
        {
            target.Name = source.Name;
            target.TemplateType = source.TemplateType;
            target.FileResourceId = source.FileResourceId;
            target.IsDefault = source.IsDefault;
            target.IsActive = source.IsActive;
            target.Notes = source.Notes;
        }

        private static void CopyDangerousGoodsDeclaration( DangerousGoodsDeclarationCreateViewModel source,DangerousGoodsDeclarationCreateViewModel target  )
        {
            target.ShipmentId = source.ShipmentId;
            target.PackageId = source.PackageId;
            target.UnNumber = source.UnNumber;
            target.ProperShippingName = source.ProperShippingName;
            target.HazardClass = source.HazardClass;
            target.PackingGroup = source.PackingGroup;
            target.NetQuantity = source.NetQuantity;
            target.QuantityUnit = source.QuantityUnit;
            target.HandlingInstructions = source.HandlingInstructions;
            target.RequiresSpecialHandling = source.RequiresSpecialHandling;
            target.Notes = source.Notes;
        }

        private static void CopyTemperatureRequirement( TemperatureRequirementCreateViewModel source, TemperatureRequirementCreateViewModel target )
        {
            target.ShipmentId = source.ShipmentId;
            target.MinTemperatureCelsius = source.MinTemperatureCelsius;
            target.MaxTemperatureCelsius = source.MaxTemperatureCelsius;
            target.RequiresTemperatureMonitoring = source.RequiresTemperatureMonitoring;
            target.TemperatureUnit = source.TemperatureUnit;
            target.Notes = source.Notes;
        }

        private static void CopyComplianceCheck( ComplianceCheckCreateViewModel source,ComplianceCheckCreateViewModel target )
        {
            target.ShipmentId = source.ShipmentId;
            target.CheckType = source.CheckType;
            target.Status = source.Status;
            target.CheckedAtUtc = source.CheckedAtUtc;
            target.CheckedBy = source.CheckedBy;
            target.ResultDetails = source.ResultDetails;
            target.Notes = source.Notes;
        }

        private static void CopyDGDocument(DGDocumentCreateViewModel source, DGDocumentCreateViewModel target)
        {
            target.DangerousGoodsDeclarationId = source.DangerousGoodsDeclarationId;
            target.FileResourceId = source.FileResourceId;
            target.DocumentName = source.DocumentName;
            target.Notes = source.Notes;
        }

        #endregion
    }
}