using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Dot.Net.WebApi.Domain;
using Dot.Net.WebApi.Controllers.Domain;
using Dot.Net.WebApi.Controllers;

namespace Dot.Net.WebApi.Data
{
    public class LocalDbContext : IdentityDbContext<User>
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public new DbSet<User> Users { get; set;}
        public DbSet<BidList> Bids { get; set;}
        public DbSet<CurvePoint> CurvePoints { get; set;}
        public DbSet<Rating> Rating { get; set;}
        public DbSet<RuleName> RuleNames { get; set;}
        public DbSet<Trade> Trades { get; set;}
    }
}