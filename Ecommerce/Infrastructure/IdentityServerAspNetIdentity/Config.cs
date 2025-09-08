using Duende.IdentityServer;
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
        // List of Micro-services can be added here
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
        },
        new Client
        {
            ClientName = "Basket Api Client",
            ClientId = "BasketApiClient",
            AllowedScopes = { BasketApiScope },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("511536CF-F270-4058-80CA-1C89C192F69A".Sha256()) },
        },
        new Client
        {
            ClientName = "EShopping Gateway Client",
            ClientId = "EShoppingGatewayClient",
            AllowedScopes = { EShoppingGateway, BasketApiScope },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("577536CF-F270-4058-80CA-1C89C192F69A".Sha256()) },
        },
        new Client
        {
            // نام نمایشی کلاینت (فقط جهت خوانایی)
            ClientName = "Angular-Client",

            // شناسه یکتای کلاینت (کلیدی برای شناسایی)
            ClientId = "angular-client",

            // نوع Grant Type مجاز: Authorization Code Flow with PKCE
            AllowedGrantTypes = GrantTypes.Code,

            // لیست آدرس‌هایی که بعد از Login موفق کاربر به آنها ریدایرکت می‌شود
            RedirectUris = new List<string>
            {
                "http://localhost:4200/signin-callback",
                "http://localhost:4200/assets/silent-callback.html",
                "https://localhost:9009/signin-oidc"
            },

            // لیست آدرس‌هایی که بعد از Logout موفق کاربر به آنها ریدایرکت می‌شود
            PostLogoutRedirectUris = new List<string>
            {
                "http://localhost:4200/signout-callback",
                "https://localhost:9009/signout-callback-oidc"
            },

            // فعال‌سازی PKCE (برای امنیت بیشتر در SPAها)
            RequirePkce = true,

            // اجازه دریافت Access Token از طریق مرورگر
            AllowAccessTokensViaBrowser = true,

            // فعال بودن کلاینت
            Enabled = true,

            // هنگام Refresh Token، Claims جدید در Access Token اعمال شود
            UpdateAccessTokenClaimsOnRefresh = true,

            // محدوده دسترسی‌هایی که کلاینت می‌تواند دریافت کند (Scopes)
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId, // برای OpenID Connect الزامی است
                IdentityServerConstants.StandardScopes.Profile, // دسترسی به پروفایل کاربر
                EShoppingGateway // یک Scope اختصاصی برای API شما
            },

            // آدرس‌هایی که برای CORS مجاز هستند
            AllowedCorsOrigins = { "http://localhost:4200" },

            // نیاز نداشتن به Client Secret (برای SPAها معمولاً false است)
            RequireClientSecret = false,

            // عدم نمایش صفحه‌ی رضایت (consent) به کاربر
            AllowRememberConsent = false,
            RequireConsent = false,

            // مدت اعتبار Access Token به ثانیه (اینجا 1 ساعت)
            AccessTokenLifetime = 3600,

            // Secret کلاینت (برای اپلیکیشن‌های Server-side مورد استفاده است)
            // اینجا تعریف شده ولی چون RequireClientSecret = false است، در SPA استفاده نمی‌شود
            ClientSecrets = new List<Secret>
            {
                new("5c6eb3b4-61a7-4668-ac57-2b4591ec26d2".Sha256())
            }
        }
    ];
}