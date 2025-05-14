using Duende.IdentityServer.Models;

namespace IdentityServerAspNetIdentity;

public static class Config
{
    //properties
    private const string CatalogApiScope = "catalogapi";
    private const string CatalogApiReadScope = "catalogapi.read";
    private const string CatalogApiWriteScope = "catalogapi.write";
    private const string BasketApiScope = "basketapi";
    private const string EShoppingGateway = "eshoppinggateway";

    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(), // this is a special identity resource that contains the sub claim
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope(CatalogApiScope, "Catalog Scope"),
        new ApiScope(CatalogApiReadScope, "Catalog Read Scope"),
        new ApiScope(CatalogApiWriteScope, "Catalog Write Scope"),
        new ApiScope(BasketApiScope, "Basket Scope"),
        new ApiScope(EShoppingGateway, "EShopping Gateway"),
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        // Catalog == Audience in the program.cs of the Catalog.API
        // List of Microservices can be added here
        new ApiResource("Catalog", "Catalog.API")
        {
            Scopes = { CatalogApiReadScope, CatalogApiWriteScope }
        },
        new ApiResource("Basket", "Basket.API")
        {
            Scopes = { BasketApiScope }
        },
        new ApiResource("EShoppingGateway", "EShopping Gateway")
        {
            Scopes = { EShoppingGateway }
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
        //m2m ClientCredentials 
        new Client
        {
            ClientName = "Catalog Api Client",
            ClientId = "CatalogApiClient",
            AllowedScopes = { CatalogApiReadScope, CatalogApiWriteScope, EShoppingGateway },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
            // AccessTokenType = AccessTokenType.Jwt,
            // AccessTokenLifetime = 36000
        },
        new Client
        {
            ClientName = "Basket Api Client",
            ClientId = "BasketApiClient",
            AllowedScopes = { BasketApiScope },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("511536CF-F270-4058-80CA-1C89C192F69A".Sha256()) },
            // AccessTokenType = AccessTokenType.Jwt,
            // AccessTokenLifetime = 36000
        },
        new Client
        {
            ClientName = "EShopping Gateway Client",
            ClientId = "EShoppingGatewayClient",
            AllowedScopes = { EShoppingGateway, BasketApiScope },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("577536CF-F270-4058-80CA-1C89C192F69A".Sha256()) },
            // AccessTokenType = AccessTokenType.Jwt,
            // AccessTokenLifetime = 36000
        }
    ];
}