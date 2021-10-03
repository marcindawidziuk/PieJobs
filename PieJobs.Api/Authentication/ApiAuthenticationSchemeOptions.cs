using Microsoft.AspNetCore.Authentication;

namespace PieJobs.Api.Authentication
{
    public class ApiAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string DefaultSchemeName = "DefaultApiAuthenticationScheme";
        public const string TokenHeaderName = "AUTH-TOKEN";
    }
}