using Carma.Domain.Entities;
using Carma.Domain.Enums;
using Carma.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Carma.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<CarmaDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        await context.Database.MigrateAsync();
        if (await userManager.Users.AnyAsync()) return;
        
        var gica = new User
        {
            UserName = "gica", 
            Email = "gica@gica.com", 
            Karma = 0
        };

        var lucianm = new User
        {
            UserName = "lucianm",
            Email = "luci@luci.com",
            Karma = 0
        };
        
        await userManager.CreateAsync(gica, "Gicagica123");
        await userManager.CreateAsync(lucianm, "LucianM123");

        var rides = new List<Ride>
        {
            new Ride
            {
                OrganizerId = lucianm.Id,
                PickupTime = DateTime.UtcNow.AddHours(5),
                Price = 19.40,
                Seats = 3,
                Status = Status.Available,
                PickupLocation = new Location(
                    45.7284636018249,
                    21.233391138813268,
                    "Str. Mureș 116A, Timișoara 300763",
                    "Timișoara",
                    "Romania"
                ),
                DropOffLocation = new Location(
                    45.7690434354139,
                    21.231505963246562,
                    "Strada Divizia 9 Cavalerie 2, Timișoara 300254",
                    "Timișoara",
                    "Romania"
                )
            },
            new Ride
            {
                OrganizerId = gica.Id,
                PickupTime = DateTime.UtcNow.AddHours(10),
                Price = 75.8,
                Seats = 3,
                Status = Status.Available,
                PickupLocation = new Location(
                    45.738195997082414
                    , 21.33070871305078,
                    "Strada Mare 40, Moșnița Veche 307287",
                    "Moșnița Veche",
                    "Romania"
                    ),
                DropOffLocation = new Location(
                    45.70306013819113, 
                    21.238333075284512,
                    "Strada Gloria 5, Giroc 307220",
                    "Giroc",
                    "Romania"
                    )
            }
        };
        
        await context.Rides.AddRangeAsync(rides);
        await context.SaveChangesAsync();
    }
}