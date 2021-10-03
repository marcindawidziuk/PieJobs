using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PieJobs.Data;

namespace PieJobs.Api.Authentication
{
    public class ApiAuthenticationHandler : AuthenticationHandler<ApiAuthenticationSchemeOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiAuthenticationHandler(IOptionsMonitor<ApiAuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceScopeFactory serviceScopeFactory,
            IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request is not null && !request.Path.StartsWithSegments("/api"))
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey(ApiAuthenticationSchemeOptions.TokenHeaderName))
                return AuthenticateResult.Fail("Missing authentication header");

            var token = Request.Headers[ApiAuthenticationSchemeOptions.TokenHeaderName].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
                return AuthenticateResult.Fail("Missing authentication header");

            if (string.IsNullOrWhiteSpace(token))
                return AuthenticateResult.Fail("Missing token");

            using var scope = _serviceScopeFactory.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IContextFactory>();

            await using var db = service.Create();

            var user = await db.Users
                .Where(x => x.ApiToken == token)
                .Select(x => new AuthenticatedUserResponse(x.Id, x.UserName))
                .SingleOrDefaultAsync();

            if (user is null)
            {
                return AuthenticateResult.Fail("User not found");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new(ClaimTypes.Name, user.Name)
            };

            var id = new ClaimsIdentity(claims, Scheme.Name);

            var principal = new ClaimsPrincipal(id);

            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        public record AuthenticatedUserResponse(int Id, string Name);
    }
}