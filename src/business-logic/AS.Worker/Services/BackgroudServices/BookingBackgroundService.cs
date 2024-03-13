using AS.Core.Factories;
using AS.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AS.Worker.Services.BackgroudServices
{
    public class BookingBackgroundService(DatabaseContextFactory<ApplicationDbContext> _contextFactory) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                    await DoWork(stoppingToken);
            }, stoppingToken);

            return Task.CompletedTask;
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var context = _contextFactory.CreateContext();
            var bookings = await context.Bookings
                .Where(s => s.Status == Core.Enums.BookingStatus.New && s.ExpiresDate < DateTime.Now)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(s => s.Status, Core.Enums.BookingStatus.Failed)
                    .SetProperty(s => s.Error, "Истек срок оплаты авиабилета"));
            await context.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
