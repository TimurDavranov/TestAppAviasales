using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AS.Identity.Api.Domain
{
    public sealed class IDDbContext : IdentityDbContext<IdentityUser>
    {
        public IDDbContext(DbContextOptions<IDDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }
    }
}
