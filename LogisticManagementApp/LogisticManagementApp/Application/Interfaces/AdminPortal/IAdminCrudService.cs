using LogisticManagementApp.Models.AdminPortal;

namespace LogisticManagementApp.Applicationn.Interfaces.AdminPortal
{
    public interface IAdminCrudService
    {
        IReadOnlyList<AdminEntityGroupViewModel> GetEntityGroups();
        AdminEntityListViewModel GetEntityList(string entityName, int page = 1, int pageSize = 50, string? searchTerm = null, string? filterColumn = null, string? filterValue = null);
        AdminEntityDetailsViewModel? GetEntityDetails(string entityName, string key);
        AdminEntityFormViewModel GetCreateForm(string entityName);
        AdminEntityFormViewModel? GetEditForm(string entityName, string key);
        AdminEntityDeleteViewModel? GetDeleteModel(string entityName, string key);
        string CreateEntity(string entityName, IDictionary<string, string?> values);
        bool UpdateEntity(string entityName, string key, IDictionary<string, string?> values);
        bool DeleteEntity(string entityName, string key);
    }
}
