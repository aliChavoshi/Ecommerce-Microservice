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
        new ApiScope("catalogapi","Catalog Scope")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        //List of Microservices can be added here
        new ApiResource("Catalog", "Catalog.API")
        {
            Scopes = { "catalogapi" }
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
        // m2m client credentials flow client
        new Client
        {
            ClientId = "CatalogApiClient",
            ClientName = "Catalog API Client",

            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

            AllowedScopes = { "catalogapi" }
        },

        // interactive client using code flow + pkce
        new Client
        {
            ClientId = "interactive",
            ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,

            RedirectUris = { "https://localhost:44300/signin-oidc" },
            FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
            PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

            AllowOfflineAccess = true,
            AllowedScopes = { "openid", "profile", "scope2" }
        }
    ];
}