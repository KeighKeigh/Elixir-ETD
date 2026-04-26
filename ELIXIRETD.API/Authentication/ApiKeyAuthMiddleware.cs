using ELIXIRETD.API.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RDF.Arcana.API.Features.Authenticate.AuthXApi
{
    public class ApiKeyAuthAttribute : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

            if (configuration == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            var apiKey = configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

            string[] headerNames = { "X-Api-Key", "Api-Key" };

            string extractedKey = null;

            foreach (var header in headerNames)
            {
                if (context.HttpContext.Request.Headers.TryGetValue(header, out var value))
                {
                    extractedKey = value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(extractedKey) || !apiKey.Equals(extractedKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid or missing API Key");
            }

            //var apiKey = configuration.GetValue<string>(AuthConstants.ApiKeySectionName);

            //if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey) ||
            //    string.IsNullOrEmpty(apiKey) ||
            //    !apiKey.Equals(extractedApiKey))
            //{
            //    context.Result = new UnauthorizedObjectResult("Invalid or missing API Key");
            //}
        }
    }
}