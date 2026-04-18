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
        public IActionResult Entities(string entity, int page = 1)
        {
            var model = _adminCrudService.GetEntityList(entity, page);
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
            try
            {
                var key = _adminCrudService.CreateEntity(entity, ExtractValues(form));
                TempData["SuccessMessage"] = "Записът беше създаден успешно.";
                return RedirectToAction(nameof(Details), new { entity, key });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var model = _adminCrudService.GetCreateForm(entity);
                ApplyPostedValues(model, form);
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
                var model = _adminCrudService.GetEditForm(entity, key);
                if (model == null) return NotFound();

                ApplyPostedValues(model, form);
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

        private static Dictionary<string, string?> ExtractValues(IFormCollection form)
        {
            return form.Keys
                .Where(x => !string.Equals(x, "__RequestVerificationToken", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x, x => form[x].ToString(), StringComparer.OrdinalIgnoreCase);
        }

        private static void ApplyPostedValues(LogisticManagementApp.Models.AdminPortal.AdminEntityFormViewModel model, IFormCollection form)
        {
            foreach (var field in model.Fields)
            {
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
