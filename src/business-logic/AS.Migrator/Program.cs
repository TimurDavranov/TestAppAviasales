using AS.Migrator;

var factory = new ApplicationDbContextFactory();

await using (var db = factory.CreateDbContext(args))
{
    if (await db.Database.CanConnectAsync())
        await db.Database.EnsureCreatedAsync();

    await db.SaveChangesAsync();
}