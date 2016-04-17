using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using eTransfert.Models;

namespace eTransfert.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

       

        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Comptes> Comptes { get; set; }
        public DbSet<Trace> Trace { get; set; }
        public DbSet<RechargeCptePrincTrace> RechargeCptePrincTrace { get; set; }
        public DbSet<Promotion> Promotion { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
