using Microsoft.Extensions.DependencyInjection;
using Wemogy.Core.Json.Extensions;

namespace Wemogy.AspNetCore.Json
{
    public static class JsonMvcBuilderExtension
    {
        public static IMvcBuilder AddWemogyJsonOptions(this IMvcBuilder builder)
        {
            builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ApplyWemogyJsonOptions();
            });

            return builder;
        }
    }
}
