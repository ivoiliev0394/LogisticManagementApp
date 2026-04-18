using LogisticManagementApp.Applicationn.Interfaces.AdminPortal;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Models.AdminPortal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace LogisticManagementApp.Applicationn.Services.AdminPortal
{
    public class AdminCrudService : IAdminCrudService
    {
        private static readonly HashSet<string> ExcludedEntities = new(StringComparer.OrdinalIgnoreCase);
        private readonly LogisticAppDbContext _dbContext;

        public AdminCrudService(LogisticAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IReadOnlyList<AdminEntityGroupViewModel> GetEntityGroups()
        {
            return GetEntityTypes()
                .Select(BuildDescriptor)
                .GroupBy(x => x.Group)
                .OrderBy(x => x.Key)
                .Select(g => new AdminEntityGroupViewModel
                {
                    Name = g.Key,
                    Entities = g.OrderBy(x => x.DisplayName).ToList()
                })
                .ToList();
        }

        public AdminEntityListViewModel GetEntityList(string entityName, int page = 1, int pageSize = 50)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var descriptor = BuildDescriptor(entityType);
            var propertyInfos = GetBrowsableProperties(entityType).Take(8).ToList();
            var pageIndex = Math.Max(1, page);
            var take = pageSize <= 0 ? 50 : Math.Min(pageSize, 200);

            var allItems = QueryEntityObjects(entityType).ToList();
            var paged = allItems.Skip((pageIndex - 1) * take).Take(take).ToList();

            return new AdminEntityListViewModel
            {
                EntityName = descriptor.Name,
                DisplayName = descriptor.DisplayName,
                GroupName = descriptor.Group,
                Columns = propertyInfos.Select(x => x.Name).ToList(),
                TotalCount = allItems.Count,
                Page = pageIndex,
                PageSize = take,
                Rows = paged.Select(item => new AdminEntityRowViewModel
                {
                    Key = BuildKey(entityType, item),
                    Values = propertyInfos.ToDictionary(
                        x => x.Name,
                        x => FormatValue(GetPropertyValue(item, x.Name)),
                        StringComparer.OrdinalIgnoreCase)
                }).ToList()
            };
        }

        public AdminEntityDetailsViewModel? GetEntityDetails(string entityName, string key)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var entity = FindEntity(entityType, key);
            if (entity == null) return null;

            var descriptor = BuildDescriptor(entityType);

            return new AdminEntityDetailsViewModel
            {
                EntityName = descriptor.Name,
                DisplayName = descriptor.DisplayName,
                GroupName = descriptor.Group,
                Key = key,
                Fields = GetBrowsableProperties(entityType)
                    .Select(p => BuildFieldViewModel(entityType, p, GetPropertyValue(entity, p.Name), isEdit: false))
                    .ToList()
            };
        }

        public AdminEntityFormViewModel GetCreateForm(string entityName)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var descriptor = BuildDescriptor(entityType);

            return new AdminEntityFormViewModel
            {
                EntityName = descriptor.Name,
                DisplayName = descriptor.DisplayName,
                GroupName = descriptor.Group,
                IsEdit = false,
                Fields = GetBrowsableProperties(entityType)
                    .Select(p => BuildFieldViewModel(entityType, p, null, isEdit: false))
                    .ToList()
            };
        }

        public AdminEntityFormViewModel? GetEditForm(string entityName, string key)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var entity = FindEntity(entityType, key);
            if (entity == null) return null;

            var descriptor = BuildDescriptor(entityType);

            return new AdminEntityFormViewModel
            {
                EntityName = descriptor.Name,
                DisplayName = descriptor.DisplayName,
                GroupName = descriptor.Group,
                Key = key,
                IsEdit = true,
                Fields = GetBrowsableProperties(entityType)
                    .Select(p => BuildFieldViewModel(entityType, p, GetPropertyValue(entity, p.Name), isEdit: true))
                    .ToList()
            };
        }

        public AdminEntityDeleteViewModel? GetDeleteModel(string entityName, string key)
        {
            var details = GetEntityDetails(entityName, key);
            if (details == null) return null;

            return new AdminEntityDeleteViewModel
            {
                EntityName = details.EntityName,
                DisplayName = details.DisplayName,
                GroupName = details.GroupName,
                Key = details.Key,
                Fields = details.Fields
            };
        }

        public string CreateEntity(string entityName, IDictionary<string, string?> values)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var entity = Activator.CreateInstance(entityType.ClrType)
                ?? throw new InvalidOperationException($"Неуспешно създаване на entity {entityType.ClrType.Name}.");

            ApplyValues(entityType, entity, values, isEdit: false);
            _dbContext.Add(entity);
            _dbContext.SaveChanges();
            return BuildKey(entityType, entity);
        }

        public bool UpdateEntity(string entityName, string key, IDictionary<string, string?> values)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var entity = FindEntity(entityType, key);
            if (entity == null) return false;

            ApplyValues(entityType, entity, values, isEdit: true);
            _dbContext.Update(entity);
            _dbContext.SaveChanges();
            return true;
        }

        public bool DeleteEntity(string entityName, string key)
        {
            var entityType = GetEntityTypeOrThrow(entityName);
            var entity = FindEntity(entityType, key);
            if (entity == null) return false;

            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
            return true;
        }

        private IEnumerable<object> QueryEntityObjects(IEntityType entityType)
        {
            var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!;
            var genericSetMethod = setMethod.MakeGenericMethod(entityType.ClrType);
            var set = genericSetMethod.Invoke(_dbContext, null)!;

            var query = (IQueryable)set;

            query = query.Provider.CreateQuery(
                Expression.Call(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.IgnoreQueryFilters),
                    new[] { entityType.ClrType },
                    query.Expression
                )
            );

            query = query.Provider.CreateQuery(
                Expression.Call(
                    typeof(EntityFrameworkQueryableExtensions),
                    nameof(EntityFrameworkQueryableExtensions.AsNoTracking),
                    new[] { entityType.ClrType },
                    query.Expression
                )
            );

            return query.Cast<object>().ToList();
        }

        private object? FindEntity(IEntityType entityType, string key)
        {
            var keyMap = ParseKey(key);
            if (keyMap.Count == 0) return null;

            return QueryEntityObjects(entityType)
                .FirstOrDefault(entity => KeyMatches(entityType, entity, keyMap));
        }

        private bool KeyMatches(IEntityType entityType, object entity, IDictionary<string, string> keyMap)
        {
            var key = entityType.FindPrimaryKey();
            if (key == null) return false;

            foreach (var keyProperty in key.Properties)
            {
                if (!keyMap.TryGetValue(keyProperty.Name, out var rawExpected))
                    return false;

                var actual = GetPropertyValue(entity, keyProperty.Name);
                var actualText = KeyValueToString(actual);

                if (!string.Equals(actualText, rawExpected, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        private void ApplyValues(IEntityType entityType, object entity, IDictionary<string, string?> values, bool isEdit)
        {
            var keyPropertyNames = entityType.FindPrimaryKey()?.Properties.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase)
                ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in GetBrowsableProperties(entityType))
            {
                if (isEdit && keyPropertyNames.Contains(property.Name))
                    continue;

                if (!values.TryGetValue(property.Name, out var rawValue))
                    continue;

                var propertyInfo = entityType.ClrType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propertyInfo == null || !propertyInfo.CanWrite)
                    continue;

                var convertedValue = ConvertFromString(rawValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(entity, convertedValue);
            }
        }

        private static object? GetPropertyValue(object entity, string propertyName)
        {
            return entity.GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                ?.GetValue(entity);
        }

        private List<IProperty> GetBrowsableProperties(IEntityType entityType)
        {
            return entityType.GetProperties()
                .Where(p => !p.IsShadowProperty())
                .OrderBy(p => p.IsPrimaryKey() ? 0 : 1)
                .ThenBy(p => p.Name)
                .ToList();
        }

        private AdminEntityDescriptorViewModel BuildDescriptor(IEntityType entityType)
        {
            var clrType = entityType.ClrType;
            return new AdminEntityDescriptorViewModel
            {
                Name = clrType.Name,
                DisplayName = SplitPascalCase(clrType.Name),
                Group = GetGroupName(clrType),
                PropertyCount = GetBrowsableProperties(entityType).Count
            };
        }

        private AdminEntityFieldViewModel BuildFieldViewModel(IEntityType entityType, IProperty property, object? value, bool isEdit)
        {
            var clrType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
            var foreignKeys = entityType.GetForeignKeys()
                .SelectMany(x => x.Properties)
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return new AdminEntityFieldViewModel
            {
                Name = property.Name,
                DisplayName = GetDisplayName(entityType.ClrType, property.Name),
                DataType = GetInputType(clrType),
                Value = FormatEditorValue(value, clrType),
                IsKey = property.IsPrimaryKey(),
                IsNullable = property.IsNullable,
                IsReadOnly = property.ValueGenerated != ValueGenerated.Never && !isEdit,
                IsForeignKey = foreignKeys.Contains(property.Name)
            };
        }

        private IEntityType GetEntityTypeOrThrow(string entityName)
        {
            var entityType = GetEntityTypes().FirstOrDefault(x => string.Equals(x.ClrType.Name, entityName, StringComparison.OrdinalIgnoreCase));

            if (entityType == null)
                throw new InvalidOperationException($"Entity '{entityName}' не е намерено в DbContext.");

            return entityType;
        }

        private IEnumerable<IEntityType> GetEntityTypes()
        {
            return _dbContext.Model.GetEntityTypes()
                .Where(x => x.ClrType != null)
                .Where(x => !ExcludedEntities.Contains(x.ClrType.Name))
                .OrderBy(x => GetGroupName(x.ClrType))
                .ThenBy(x => x.ClrType.Name);
        }

        private static string GetGroupName(Type clrType)
        {
            var namespaceValue = clrType.Namespace ?? string.Empty;
            var prefix = "LogisticManagementApp.Domain.";
            if (namespaceValue.StartsWith(prefix, StringComparison.Ordinal))
            {
                namespaceValue = namespaceValue[prefix.Length..];
            }

            var parts = namespaceValue.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "Other";
            if (parts.Length == 1) return SplitPascalCase(parts[0]);

            return string.Join(" / ", parts.Take(2).Select(SplitPascalCase));
        }

        private static string BuildKey(IEntityType entityType, object entity)
        {
            var primaryKey = entityType.FindPrimaryKey()
                ?? throw new InvalidOperationException($"Entity {entityType.ClrType.Name} няма primary key.");

            return string.Join(";", primaryKey.Properties.Select(p => $"{p.Name}={Uri.EscapeDataString(KeyValueToString(GetPropertyValue(entity, p.Name)))}"));
        }

        private static Dictionary<string, string> ParseKey(string key)
        {
            return key.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => Uri.UnescapeDataString(parts[1]), StringComparer.OrdinalIgnoreCase);
        }

        private static string KeyValueToString(object? value)
        {
            if (value == null) return string.Empty;

            return value switch
            {
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dto => dto.ToString("O", CultureInfo.InvariantCulture),
                DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                TimeOnly timeOnly => timeOnly.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture),
                bool boolValue => boolValue ? "true" : "false",
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            };
        }

        private static string FormatValue(object? value)
        {
            if (value == null) return "—";
            if (value is string stringValue) return string.IsNullOrWhiteSpace(stringValue) ? "—" : stringValue;
            if (value is bool boolValue) return boolValue ? "Да" : "Не";
            if (value is DateTime dateTime) return dateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            if (value is DateTimeOffset dateTimeOffset) return dateTimeOffset.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            if (value is DateOnly dateOnly) return dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (value is decimal decimalValue) return decimalValue.ToString("0.##", CultureInfo.InvariantCulture);
            if (value is byte[] bytes) return $"[{bytes.Length} bytes]";
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "—";
        }

        private static string? FormatEditorValue(object? value, Type clrType)
        {
            if (value == null) return null;

            if (clrType == typeof(DateTime))
                return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            if (clrType == typeof(DateTimeOffset))
                return ((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            if (clrType == typeof(DateOnly))
                return ((DateOnly)value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (clrType == typeof(TimeOnly))
                return ((TimeOnly)value).ToString("HH:mm", CultureInfo.InvariantCulture);
            if (clrType == typeof(bool))
                return (bool)value ? "true" : "false";
            if (clrType.IsEnum)
                return value.ToString();
            if (clrType == typeof(byte[]))
                return Convert.ToBase64String((byte[])value);

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static object? ConvertFromString(string? rawValue, Type targetType)
        {
            if (targetType == typeof(string))
                return rawValue;

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            var effectiveType = underlyingType ?? targetType;

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (underlyingType != null || !effectiveType.IsValueType)
                    return null;

                return Activator.CreateInstance(effectiveType);
            }

            if (effectiveType == typeof(Guid)) return Guid.Parse(rawValue);
            if (effectiveType == typeof(int)) return int.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(long)) return long.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(short)) return short.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(byte)) return byte.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(decimal)) return decimal.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(double)) return double.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(float)) return float.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(bool)) return rawValue.Equals("true", StringComparison.OrdinalIgnoreCase) || rawValue == "on" || rawValue == "1";
            if (effectiveType == typeof(DateTime)) return DateTime.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(DateTimeOffset)) return DateTimeOffset.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(DateOnly)) return DateOnly.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(TimeOnly)) return TimeOnly.Parse(rawValue, CultureInfo.InvariantCulture);
            if (effectiveType == typeof(byte[])) return Convert.FromBase64String(rawValue);
            if (effectiveType.IsEnum) return Enum.Parse(effectiveType, rawValue, ignoreCase: true);

            var converter = TypeDescriptor.GetConverter(effectiveType);
            if (converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromInvariantString(rawValue);

            return Convert.ChangeType(rawValue, effectiveType, CultureInfo.InvariantCulture);
        }

        private static string GetDisplayName(Type clrType, string propertyName)
        {
            var propertyInfo = clrType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var displayNameAttribute = propertyInfo?.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttribute != null)
                return displayNameAttribute.DisplayName;

            return SplitPascalCase(propertyName);
        }

        private static string GetInputType(Type clrType)
        {
            if (clrType == typeof(bool)) return "checkbox";
            if (clrType == typeof(DateTime) || clrType == typeof(DateTimeOffset)) return "datetime-local";
            if (clrType == typeof(DateOnly)) return "date";
            if (clrType == typeof(TimeOnly)) return "time";
            if (clrType == typeof(int) || clrType == typeof(long) || clrType == typeof(short) || clrType == typeof(decimal) || clrType == typeof(double) || clrType == typeof(float)) return "number";
            if (clrType == typeof(Guid)) return "text";
            if (clrType.IsEnum) return "text";
            if (clrType == typeof(byte[])) return "textarea";
            return "text";
        }

        private static string SplitPascalCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            return string.Concat(value.Select((c, i) => i > 0 && char.IsUpper(c) && !char.IsUpper(value[i - 1]) ? $" {c}" : c.ToString()));
        }
    }
}
