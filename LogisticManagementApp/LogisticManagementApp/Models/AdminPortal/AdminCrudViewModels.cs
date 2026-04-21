namespace LogisticManagementApp.Models.AdminPortal
{
    public class AdminEntityGroupViewModel
    {
        public string Name { get; set; } = string.Empty;
        public List<AdminEntityDescriptorViewModel> Entities { get; set; } = new();
    }

    public class AdminEntityDescriptorViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public int PropertyCount { get; set; }
    }

    public class AdminEntityListViewModel
    {
        public string EntityName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public IReadOnlyList<string> Columns { get; set; } = Array.Empty<string>();
        public IReadOnlyList<AdminEntityRowViewModel> Rows { get; set; } = Array.Empty<AdminEntityRowViewModel>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }
        public string? FilterColumn { get; set; }
        public string? FilterValue { get; set; }
        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class AdminEntityRowViewModel
    {
        public string Key { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool CanDelete { get; set; } = true;
        public Dictionary<string, string> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class AdminEntityDetailsViewModel
    {
        public string EntityName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool CanDelete { get; set; } = true;
        public List<AdminEntityFieldViewModel> Fields { get; set; } = new();
    }

    public class AdminEntityDeleteViewModel : AdminEntityDetailsViewModel
    {
    }

    public class AdminEntityFormViewModel
    {
        public string EntityName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? Key { get; set; }
        public bool IsEdit { get; set; }
        public List<AdminEntityFieldViewModel> Fields { get; set; } = new();
    }

    public class AdminEntityFieldViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = "string";
        public string? Value { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsHidden { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsEnum { get; set; }
        public bool IsDecimalNumber { get; set; }
        public bool UseForeignKeyDropdown { get; set; }
        public bool UseEnumDropdown { get; set; }
        public string? ForeignEntityName { get; set; }
        public string? EnumTypeName { get; set; }
        public List<AdminForeignKeyOptionViewModel> ForeignKeyOptions { get; set; } = new();
        public List<AdminForeignKeyOptionViewModel> EnumOptions { get; set; } = new();
    }

    public class AdminForeignKeyOptionViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
