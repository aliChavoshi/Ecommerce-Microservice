using Duende.IdentityServer.Models;

namespace IdentityServerAspNetIdentity;

public static class Config
{
    //properties
    private const string CatalogApiScope = "catalogapi";
    private const string BasketApiScope = "basketapi";

    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(), // this is a special identity resource that contains the sub claim
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope(CatalogApiScope, "Catalog Scope"),
        // new ApiScope(BasketApiScope, "Basket Scope")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        // Catalog == Audience in the program.cs of the Catalog.API
        // List of Microservices can be added here
        new ApiResource("Catalog", "Catalog.API")
        {
            Scopes = { CatalogApiScope }
        },
        // new ApiResource("Basket", "Basket.API")
        // {
        //     Scopes = { BasketApiScope }
        // }
    ];

    public static IEnumerable<Client> Clients =>
    [
        //m2m ClientCredentials
        new Client
        {
            ClientId = "CatalogApiClient",
            ClientName = "Catalog Api Client",
            AllowedScopes = { CatalogApiScope },
            ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AccessTokenType = AccessTokenType.Jwt,
            AccessTokenLifetime = 36000
        }
    ];
}