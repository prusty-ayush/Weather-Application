using Authorisation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Azure.Core.HttpHeader;

namespace Authorisation.Infrastructure.Data
{
    ///AppDbContext is your database bridge between:
    ///Your C# domain entities (User) 
    ///Your actual database(SQL Server, PostgreSQL, etc.)
    ///Entity Framework Core(EF Core)
    public class AppDbContext :DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        /// This represents a table in the database
        public DbSet<User> users { get; set; }


        /// This method is used to configure:
        /// Relationships
        /// Constraints
        /// Indexes
        /// Table names
        /// Keys
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
