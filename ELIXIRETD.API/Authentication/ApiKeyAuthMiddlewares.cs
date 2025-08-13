//using ELIXIRETD.API.Authentication;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace ELIXIRETD.API.Authentication
//{
//    public class KeyApiAttribute : Attribute, IAuthorizationFilter
//    {

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

//            if (configuration == null)
//            {
//                context.Result = new StatusCodeResult(500);
//                return;
//            }

//            var apiKey = configuration.GetValue<string>(AuthConstant.ApiKeySectionNameOne);

//            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstant.ApiKeyHeaderNameOne, out var extractedApiKey) ||
//                string.IsNullOrEmpty(apiKey) ||
//                !apiKey.Equals(extractedApiKey))
//            {
//                context.Result = new UnauthorizedObjectResult("Invalid or missing API Key");
//            }
//        }
//    }
//}
