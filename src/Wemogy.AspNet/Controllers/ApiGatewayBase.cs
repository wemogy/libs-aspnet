using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using RestSharp;
using RestSharp.Serializers.Json;
using Wemogy.AspNet.Exceptions;
using Wemogy.Core.Extensions;
using Wemogy.Core.Json;

namespace Wemogy.AspNet.Controllers
{
    public abstract class ApiGatewayBase : ControllerBase
    {
        private const string _subjectIdHeaderKey = "x-subject-id";
        private const string _tenantIdHeaderKey = "x-tenant-id";
        private readonly RestClient _restClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ApiGatewayBase(string baseUrl, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _restClient = new RestClient(baseUrl).UseSystemTextJson(WemogyJson.Options);
        }

        private ICollection<KeyValuePair<string, string>> GetHeadersMapped()
        {
            var headers = new Dictionary<string, string>();

            if (this._httpContextAccessor.HttpContext == null)
            {
                return headers;
            }

            if (this._httpContextAccessor.HttpContext.User.Identity != null && this._httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var claimsIdentity = this._httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                var nameIdentifier = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (nameIdentifier != null)
                {
                    headers.Add(_subjectIdHeaderKey, nameIdentifier);
                }
            }

            if (this._httpContextAccessor.HttpContext.Request.Headers.ContainsKey(_tenantIdHeaderKey))
            {
                var tenantIdHeader = this._httpContextAccessor.HttpContext.Request.Headers[_tenantIdHeaderKey].FirstOrDefault();
                // ensure that tenant id is a GUID
                if (Guid.TryParse(tenantIdHeader, out Guid tenantId))
                {
                    headers.Add(_tenantIdHeaderKey, tenantId.ToString());
                }
                else
                {
                    throw new InvalidHeaderException(_tenantIdHeaderKey, tenantIdHeader);
                }
            }

            return headers;
        }

        private async Task<ActionResult> JsonGatewayAsync(string resource, Method method, Func<RestRequest, RestRequest> restRequestOperation, CancellationToken cancellationToken, bool deserializeContent = false)
        {
            RestRequest restRequest = new RestRequest(resource, method);

            try
            {
                restRequest = restRequest.AddHeaders(this.GetHeadersMapped());
                restRequest = restRequestOperation(restRequest);
            }
            catch (InvalidHeaderException exception)
            {
                return new BadRequestObjectResult(exception);
            }

            var result = await this._restClient.ExecuteAsync(restRequest, cancellationToken);

            if (!result.IsSuccessful)
            {
                return new StatusCodeResult((int)result.StatusCode);
            }

            try
            {
                // ToDo: remove empty properties
                /*var resultJToken = JToken.Parse(result.Content)
                    .RemoveEmptyProperties();
                var resultJson = resultJToken.ToString();*/

                if (deserializeContent)
                {
                    return Ok(result.Content.FromJson<string>());
                }

                return Content(result.Content, System.Net.Mime.MediaTypeNames.Application.Json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                // ToDo: decide if we throw an exception here, because in this case the JSON could be invalid
                return new OkObjectResult(result.Content);
            }
        }

        protected Task<ActionResult> JsonPostGatewayAsync<TRequest>(string resource, TRequest request, CancellationToken cancellationToken, bool deserializeContent = false)
            where TRequest : class
        {
            return this.JsonGatewayAsync(resource, Method.Post, req => req.AddJsonBody(request), cancellationToken, deserializeContent);
        }

        protected Task<ActionResult> JsonPostGatewayAsync(string resource, CancellationToken cancellationToken)
        {
            return this.JsonGatewayAsync(resource, Method.Post, req => req, cancellationToken);
        }

        protected Task<ActionResult> JsonPutGatewayAsync<TRequest>(string resource, TRequest request, CancellationToken cancellationToken)
            where TRequest : class
        {
            return this.JsonGatewayAsync(resource, Method.Put, req => req.AddJsonBody(request), cancellationToken);
        }

        protected Task<ActionResult> JsonDeleteGatewayAsync<TRequest>(string resource, TRequest request, CancellationToken cancellationToken)
            where TRequest : class
        {
            return this.JsonGatewayAsync(resource, Method.Delete, req => req.AddJsonBody(request), cancellationToken);
        }

        protected Task<ActionResult> JsonDeleteGatewayAsync(string resource, CancellationToken cancellationToken)
        {
            return this.JsonGatewayAsync(resource, Method.Delete, req => req, cancellationToken);
        }

        protected Task<ActionResult> JsonGetGatewayAsync(string resource, CancellationToken cancellationToken)
        {
            return this.JsonGatewayAsync(resource, Method.Get, req => req, cancellationToken);
        }

        protected Task<ActionResult> JsonPatchGatewayAsync<TRequest>(string resource, TRequest request, CancellationToken cancellationToken)
            where TRequest : class
        {
            return this.JsonGatewayAsync(resource, Method.Patch, req => req.AddJsonBody(request), cancellationToken);
        }

        protected Task<ActionResult> GraphQlGatewayAsync(string graphQlEndpoint, JsonDocument graphQlRequest, CancellationToken cancellationToken)
        {
            return this.JsonPostGatewayAsync(graphQlEndpoint, graphQlRequest, cancellationToken);
        }

        protected Task<ActionResult> GraphQlGatewayAsync(JsonDocument graphQlRequest, CancellationToken cancellationToken)
        {
            return this.JsonPostGatewayAsync("graphql", graphQlRequest, cancellationToken);
        }

        protected Task<ActionResult> FilePostGatewayAsync(string resource, IFormFile formFile, CancellationToken cancellationToken)
        {
            return this.JsonGatewayAsync(resource, Method.Post, req => req.AddFile(formFile.Name, formFile.OpenReadStream, formFile.Name, formFile.ContentType), cancellationToken);
        }
    }
}
