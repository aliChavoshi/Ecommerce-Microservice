using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServerAspNetIdentity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // builder.WebHost.ConfigureKestrel(options =>
        // {
        //     // اجازه بده روی همه IPها گوش بده (نه فقط localhost)
        //     options.ListenAnyIP(9011);
        //     options.ListenAnyIP(9009, listenOptions =>
        //     {
        //         listenOptions.UseHttps(); // اگر https می‌خوای
        //     });
        // });

        builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
                options.IssuerUri = "http://identityserveraspnetidentity:8080"; //aud in jwt token : ocelot
                // options.IssuerUri = "https://id-local.eshopping.com:44344"; //aud in jwt token : nxing
                // options.IssuerUri = "http://host.docker.internal:9011"; //localhost
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential();

        //this is for Google authentication
        // builder.Services.AddAuthentication()
        //     .AddGoogle(options =>
        //     {
        //         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //
        //         // register your IdentityServer with Google at https://console.developers.google.com
        //         // enable the Google+ API
        //         // set the redirect URI to https://localhost:5001/signin-google
        //         options.ClientId = "copy client ID from Google here";
        //         options.ClientSecret = "copy client secret from Google here";
        //     });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        //for Nginx reverse proxy
        var forwardedHeaderOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        forwardedHeaderOptions.KnownNetworks.Clear();
        forwardedHeaderOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardedHeaderOptions);
        //
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();

        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}