# Sitecore-Identity-AzureADB2C
Plugin for Sitecore Identity Server adding Azure B2C support
Populate the following config values:
- ClientId: Application ID from B2C application
- Tenant: xxxx.onmicrosoft.com
- Policy: User flow name

Create a custom user attribute in B2C called "SitecoreAdmin" of type boolean.  If this is set to "true" the user is logged in as Sitecore admin.
