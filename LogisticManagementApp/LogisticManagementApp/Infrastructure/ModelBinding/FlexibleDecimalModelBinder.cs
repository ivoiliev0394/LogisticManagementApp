using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace LogisticManagementApp.Infrastructure.ModelBinding
{
    public class FlexibleDecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            var rawValue = valueResult.FirstValue;

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (bindingContext.ModelMetadata.IsReferenceOrNullableType)
                {
                    bindingContext.Result = ModelBindingResult.Success(null);
                    return Task.CompletedTask;
                }

                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Полето {bindingContext.ModelMetadata.GetDisplayName()} е задължително.");
                return Task.CompletedTask;
            }

            var normalizedValue = rawValue.Trim().Replace(" ", string.Empty);

            if (TryParseDecimal(normalizedValue, out var parsedDecimal))
            {
                bindingContext.Result = ModelBindingResult.Success(parsedDecimal);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Полето {bindingContext.ModelMetadata.GetDisplayName()} трябва да е валидно число.");
            return Task.CompletedTask;
        }

        private static bool TryParseDecimal(string input, out decimal value)
        {
            var styles = NumberStyles.Number | NumberStyles.AllowLeadingSign;
            var candidates = new[]
            {
                input,
                input.Replace(",", "."),
                input.Replace(".", ",")
            }.Distinct();

            foreach (var candidate in candidates)
            {
                if (decimal.TryParse(candidate, styles, CultureInfo.InvariantCulture, out value) ||
                    decimal.TryParse(candidate, styles, new CultureInfo("bg-BG"), out value) ||
                    decimal.TryParse(candidate, styles, CultureInfo.CurrentCulture, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
