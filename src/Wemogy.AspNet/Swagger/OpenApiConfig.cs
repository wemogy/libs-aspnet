using Microsoft.OpenApi.Models;

namespace Wemogy.AspNet.Swagger;

public class OpenApiConfig
{
    public OpenApiInfo OpenApiInfo { get; }

    /// <summary>
    /// Determines, if the OpenAPI specification should be published to the UI.
    /// </summary>
    public bool Publish { get; }

    public OpenApiConfig(OpenApiInfo openApiInfo, bool publish)
    {
        OpenApiInfo = openApiInfo;
        Publish = publish;
    }
}
