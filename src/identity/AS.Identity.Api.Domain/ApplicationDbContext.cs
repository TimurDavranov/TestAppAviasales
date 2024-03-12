using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AS.Identity.Api.Domain
{
    public interface IApplicationDbContext
    {

    }

    public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
