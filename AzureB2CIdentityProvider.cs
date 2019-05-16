namespace Sitecore.Plugin.IdentityProvider.AzureB2C
{
    public class AzureB2CIdentityProvider : IdentityProviders.IdentityProvider
    {
        public string ClientId { get; set; }

        public string Tenant { get; set; }

        public string Policy { get; set; }
    }
}
