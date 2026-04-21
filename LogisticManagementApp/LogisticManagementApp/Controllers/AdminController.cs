using LogisticManagementApp.Applicationn.Interfaces.AdminPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticManagementApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminCrudService _adminCrudService;

        public AdminController(IAdminCrudService adminCrudService)
        {
            _adminCrudService = adminCrudService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Portal";
            return View("~/Views/Admin/Index.cshtml", _adminCrudService.GetEntityGroups());
        }

        [HttpGet]
        public IActionResult Entities(
            string entity,
            int page = 1,
            string? searchTerm = null,
            string? filterColumn = null,
            string? filterValue = null)
        {
            var model = _adminCrudService.GetEntityList(entity, page, 50, searchTerm, filterColumn, filterValue);
            ViewData["Title"] = $"Admin · {model.DisplayName}";
            return View("~/Views/Admin/Entities.cshtml", model);
        }

        [HttpGet]
        public IActionResult Details(string entity, string key)
        {
            var model = _adminCrudService.GetEntityDetails(entity, key);
            if (model == null) return NotFound();

            ViewData["Title"] = $"Admin · {model.DisplayName}";
            return View("~/Views/Admin/Details.cshtml", model);
        }

        [HttpGet]
        public IActionResult Create(string entity)
        {
            var model = _adminCrudService.GetCreateForm(entity);
            ViewData["Title"] = $"Admin · Нов {model.DisplayName}";
            return View("~/Views/Admin/Form.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string entity, IFormCollection form)
        {
            var model = _adminCrudService.GetCreateForm(entity);
            var requiredSummaryMessages = ValidateRequiredFields(model, form, isEdit: false);
            ViewBag.RequiredSummaryMessages = requiredSummaryMessages;

            if (!ModelState.IsValid)
            {
                ApplyPostedValues(model, form);
                ViewData["Title"] = $"Admin · Нов {model.DisplayName}";
                return View("~/Views/Admin/Form.cshtml", model);
            }

            try
            {
                var key = _adminCrudService.CreateEntity(entity, ExtractValues(form));
                TempData["SuccessMessage"] = "Записът беше създаден успешно.";
                return RedirectToAction(nameof(Details), new { entity, key });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ApplyPostedValues(model, form);
                ViewData["Title"] = $"Admin · Нов {model.DisplayName}";
                return View("~/Views/Admin/Form.cshtml", model);
            }
        }

        [HttpGet]
        public IActionResult Edit(string entity, string key)
        {
            var model = _adminCrudService.GetEditForm(entity, key);
            if (model == null) return NotFound();

            ViewData["Title"] = $"Admin · Редакция {model.DisplayName}";
            return View("~/Views/Admin/Form.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string entity, string key, IFormCollection form)
        {
            var model = _adminCrudService.GetEditForm(entity, key);
            if (model == null) return NotFound();

            var requiredSummaryMessages = ValidateRequiredFields(model, form, isEdit: true);
            ViewBag.RequiredSummaryMessages = requiredSummaryMessages;

            if (!ModelState.IsValid)
            {
                ApplyPostedValues(model, form);
                ViewData["Title"] = $"Admin · Редакция {model.DisplayName}";
                return View("~/Views/Admin/Form.cshtml", model);
            }

            try
            {
                if (!_adminCrudService.UpdateEntity(entity, key, ExtractValues(form)))
                    return NotFound();

                TempData["SuccessMessage"] = "Записът беше редактиран успешно.";
                return RedirectToAction(nameof(Details), new { entity, key });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ApplyPostedValues(model, form);
                ViewData["Title"] = $"Admin · Редакция {model.DisplayName}";
                return View("~/Views/Admin/Form.cshtml", model);
            }
        }

        [HttpGet]
        public IActionResult Delete(string entity, string key)
        {
            var model = _adminCrudService.GetDeleteModel(entity, key);
            if (model == null) return NotFound();

            ViewData["Title"] = $"Admin · Изтриване {model.DisplayName}";
            return View("~/Views/Admin/Delete.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string entity, string key)
        {
            try
            {
                if (!_adminCrudService.DeleteEntity(entity, key))
                    return NotFound();

                TempData["SuccessMessage"] = "Записът беше изтрит успешно.";
                return RedirectToAction(nameof(Entities), new { entity });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { entity, key });
            }
        }


        private List<string> ValidateRequiredFields(LogisticManagementApp.Models.AdminPortal.AdminEntityFormViewModel model, IFormCollection form, bool isEdit)
        {
            var requiredSummaryMessages = new List<string>();

            foreach (var field in model.Fields)
            {
                if (field.IsHidden)
                    continue;

                if (isEdit && field.IsKey)
                    continue;

                if (!isEdit && field.IsReadOnly)
                    continue;

                if (field.IsNullable)
                    continue;

                if (string.Equals(field.DataType, "checkbox", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!form.TryGetValue(field.Name, out var postedValue) || string.IsNullOrWhiteSpace(postedValue.ToString()))
                {
                    var message = $"Полето {field.DisplayName} е задължително.";
                    requiredSummaryMessages.Add(message);
                    ModelState.AddModelError(field.Name, message);
                }
            }

            return requiredSummaryMessages;
        }

        private static Dictionary<string, string?> ExtractValues(IFormCollection form)
        {
            return form.Keys
                .Where(x => !string.Equals(x, "__RequestVerificationToken", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    x => x,
                    x => form[x].Count > 0 ? form[x][form[x].Count - 1]?.ToString() : null,
                    StringComparer.OrdinalIgnoreCase);
        }

        private static void ApplyPostedValues(LogisticManagementApp.Models.AdminPortal.AdminEntityFormViewModel model, IFormCollection form)
        {
            foreach (var field in model.Fields)
            {
                if (field.IsHidden)
                {
                    continue;
                }

                if (field.DataType == "checkbox")
                {
                    field.Value = form.ContainsKey(field.Name) ? "true" : "false";
                    continue;
                }

                if (form.TryGetValue(field.Name, out var postedValue))
                {
                    field.Value = postedValue.ToString();
                }
            }
        }
    }
}
