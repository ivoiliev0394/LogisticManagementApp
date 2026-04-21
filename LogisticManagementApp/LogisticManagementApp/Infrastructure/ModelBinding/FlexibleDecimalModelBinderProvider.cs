using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LogisticManagementApp.Infrastructure.ModelBinding
{
    public class FlexibleDecimalModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _binder = new FlexibleDecimalModelBinder();

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var modelType = context.Metadata.UnderlyingOrModelType;
            return modelType == typeof(decimal) ? _binder : null;
        }
    }
}
