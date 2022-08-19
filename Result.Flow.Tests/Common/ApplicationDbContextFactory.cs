using System;
using Microsoft.EntityFrameworkCore;
using Result.Flow.Persistence;

namespace Result.Flow.Tests.Common;

public class ApplicationDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        context.Database.EnsureCreated();

        DataSeed(context);
        context.SaveChangesAsync();

        return context;
    }

    private static void DataSeed(ApplicationDbContext context)
    {
        var user = new User
        {
            Id = 1,
            CreditCard = new CreditCard
            {
                Number = "1234-5678-1234-5678",
                Expiry = new DateOnly(2023, 03, 01),
                Cvv = 123
            }
        };
        context.Add(user);
    }
}