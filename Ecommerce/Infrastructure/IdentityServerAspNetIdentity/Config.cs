﻿using Duende.IdentityServer.Models;

namespace IdentityServerAspNetIdentity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(), // this is a special identity resource that contains the sub claim
        new IdentityResources.Profile(),
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("catalogapi","Catalog Scope"),
        new ApiScope("basketapi","Basket Scope"),
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        //List of Microservices can be added here
        // Catalog == Audience in the program.cs of the Catalog.API
        new ApiResource("Catalog", "Catalog.API")
        {
            Scopes = { "catalogapi" }
        },
        new ApiResource("Basket", "Basket.API")
        {
            Scopes = { "basketapi" }
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
        // m2m client credentials flow client
        new Client
        {
            ClientId = "CatalogApiClient",
            ClientName = "Catalog API Client",

            AllowedGrantTypes = GrantTypes.ClientCredentials, // this type for machine to machine communication
            ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

            AllowedScopes = { "catalogapi" }
        },
        // m2m client credentials flow client
        new Client
        {
            ClientId = "BasketApiClient",
            ClientName = "Basket API Client",

            AllowedGrantTypes = GrantTypes.ClientCredentials, // this type for machine to machine communication
            ClientSecrets = { new Secret("E6CA9A3E-E61B-4F2A-A271-C3FB38FE933E".Sha256()) },

            AllowedScopes = { "basketapi" }
        },
    ];
}