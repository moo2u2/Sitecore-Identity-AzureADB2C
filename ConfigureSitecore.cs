using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Sitecore.Framework.Runtime.Configuration;
using System;

namespace Sitecore.Plugin.IdentityProvider.AzureB2C
{
    using Configuration;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ConfigureSitecore
    {
        private readonly ILogger<ConfigureSitecore> _logger;
        private readonly AppSettings _appSettings;

        public ConfigureSitecore(ISitecoreConfiguration scConfig, ILogger<ConfigureSitecore> logger)
        {
            _logger = logger;
            _appSettings = new AppSettings();
            scConfig.GetSection(AppSettings.SectionName);
            scConfig.GetSection(AppSettings.SectionName).Bind(_appSettings.AzureB2CIdentityProvider);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            AzureB2CIdentityProvider azureB2Cprovider = _appSettings.AzureB2CIdentityProvider;
            if (!azureB2Cprovider.Enabled)
                return;

            _logger.LogDebug("Configure '" + azureB2Cprovider.DisplayName + "'. AuthenticationScheme = " + azureB2Cprovider.AuthenticationScheme + ", ClientId = " + azureB2Cprovider.ClientId, Array.Empty<object>());
            new AuthenticationBuilder(services).AddOpenIdConnect(azureB2Cprovider.AuthenticationScheme, azureB2Cprovider.DisplayName, options =>
            {
                options.SignInScheme = "idsrv.external";
                options.CallbackPath = new PathString("/signin-idsrv");
                options.ClientId = azureB2Cprovider.ClientId;
                options.Authority = $"https://login.microsoftonline.com/tfp/{azureB2Cprovider.Tenant}/{azureB2Cprovider.Policy}/v2.0";
                options.MetadataAddress = $"https://login.microsoftonline.com/{azureB2Cprovider.Tenant}/v2.0/.well-known/openid-configuration?p={azureB2Cprovider.Policy}";
                options.UseTokenLifetime = true;
                options.SignedOutCallbackPath = new PathString($"/signout/{azureB2Cprovider.Policy}");
                options.SignedOutCallbackPath = new PathString($"/signout-callback-{azureB2Cprovider.Policy}");
                options.RemoteSignOutPath = new PathString($"/signout-{azureB2Cprovider.Policy}");
                options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "name" };
                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = (context) =>
                    {
                        var debugClaims = context.Principal?.Claims;
                        return Task.CompletedTask; // break here to see claims values
                    },
                };
            });
        }
    }
}
