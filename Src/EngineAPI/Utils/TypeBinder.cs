using EngineAPI.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EngineAPI.Utils
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(propertyName);
            if (value == ValueProviderResult.None)
                return Task.CompletedTask;

            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(value.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(propertyName, Resource.ValueIsNotOfCorrectType );
            }

            return Task.CompletedTask;

        }
    }
}
