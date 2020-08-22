using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NanoShop.Core.Common;
using NanoShop.Core.Entities;
using System;
using System.Linq;
using System.Reflection;

namespace NanoShop.Core
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public IConfiguration Configuration { get; }
        public virtual DbSet<Product> Product { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("NanoShop");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //Seed
/*            builder.Entity<ApplicationRole>().HasData(
                new { Id = 1, Name = StaticRoles.Admin, NormalizedName = StaticRoles.Admin.ToUpper() },
                new { Id = 2, Name = StaticRoles.Customer, NormalizedName = StaticRoles.Customer.ToUpper() }
                );*/
            base.OnModelCreating(builder);
        }
    }
}
