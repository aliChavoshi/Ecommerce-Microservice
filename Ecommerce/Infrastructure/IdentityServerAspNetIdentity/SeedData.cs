using System.Security.Claims;
using Duende.IdentityModel;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServerAspNetIdentity;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = userMgr.FindByNameAsync("alichavoshi").Result;
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = "alichavoshi",
                Email = "alichavoshi@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                PhoneNumber = "09130242780"
            };
            var result = userMgr.CreateAsync(user, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(user, [
                new Claim(JwtClaimTypes.Name, "Ali Chavoshi"),
                new Claim(JwtClaimTypes.GivenName, "alichavoshi"),
                new Claim(JwtClaimTypes.FamilyName, "Chavoshi"),
                new Claim(JwtClaimTypes.WebSite, "https://www.daneshjooyar.com/teacher/alichavoshi/")
            ]).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("alichavoshi created");
        }
        else
        {
            Log.Debug("alichavoshi already exists");
        }

        // var bob = userMgr.FindByNameAsync("bob").Result;
        // if (bob == null)
        // {
        //     bob = new ApplicationUser
        //     {
        //         UserName = "bob",
        //         Email = "BobSmith@email.com",
        //         EmailConfirmed = true
        //     };
        //     var result = userMgr.CreateAsync(bob, "Pass123$").Result;
        //     if (!result.Succeeded)
        //     {
        //         throw new Exception(result.Errors.First().Description);
        //     }
        //
        //     result = userMgr.AddClaimsAsync(bob, new Claim[]
        //     {
        //         new Claim(JwtClaimTypes.Name, "Bob Smith"),
        //         new Claim(JwtClaimTypes.GivenName, "Bob"),
        //         new Claim(JwtClaimTypes.FamilyName, "Smith"),
        //         new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
        //         new Claim("location", "somewhere")
        //     }).Result;
        //     if (!result.Succeeded)
        //     {
        //         throw new Exception(result.Errors.First().Description);
        //     }
        //
        //     Log.Debug("bob created");
        // }
        // else
        // {
        //     Log.Debug("bob already exists");
        // }
    }
}