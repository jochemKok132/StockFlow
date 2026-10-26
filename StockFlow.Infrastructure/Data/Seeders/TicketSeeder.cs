using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Support;
using StockFlow.Domain.Enums;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class TicketSeeder
    {
        public static void UseTicketSeeder(this DbContext context)
        {
            if (context.Set<Ticket>().Any())
                return;

            context.Set<Ticket>().AddRange(
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    TicketNumber = 1001,
                    Type = TicketType.Support,
                    Status = TicketStatus.Done,
                    Title = "Can't log in on register 2",
                    Description = "The cash register says my session expired every few minutes.",
                    Response = "The clock of register 2 was wrong. It is fixed now.",
                    CreatedById = SeedIds.CashierOne,
                    CreatedAt = SeedIds.DaysAgo(6, 10),
                    UpdatedAt = SeedIds.DaysAgo(5, 9),
                    HandledById = SeedIds.Support,
                    HandledAt = SeedIds.DaysAgo(5, 9),
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    TicketNumber = 1002,
                    Type = TicketType.Support,
                    Status = TicketStatus.Open,
                    Title = "Receipt printer needs paper",
                    Description = "Where do we keep the spare rolls for the receipt printer?",
                    CreatedById = SeedIds.CashierTwo,
                    CreatedAt = SeedIds.DaysAgo(0, 9, 30),
                    UpdatedAt = SeedIds.DaysAgo(0, 9, 30),
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    TicketNumber = 1003,
                    Type = TicketType.Bug,
                    Status = TicketStatus.InProgress,
                    Title = "Stock count shows the wrong location",
                    Description = "The location of a beam on the count list doesn't match the label on the shelf.",
                    Response = "Looking into it.",
                    CreatedById = SeedIds.FloorWorkerOne,
                    CreatedAt = SeedIds.DaysAgo(2, 14),
                    UpdatedAt = SeedIds.DaysAgo(1, 10),
                    HandledById = SeedIds.Admin,
                    HandledAt = SeedIds.DaysAgo(1, 10),
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    TicketNumber = 1004,
                    Type = TicketType.Feature,
                    Status = TicketStatus.Open,
                    Title = "Email the order to the supplier",
                    Description = "It would save time if a sent order was emailed to the supplier automatically.",
                    CreatedById = SeedIds.Manager,
                    CreatedAt = SeedIds.DaysAgo(3, 16),
                    UpdatedAt = SeedIds.DaysAgo(3, 16),
                });
            context.SaveChanges();
        }
    }
}
