using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HomeLib.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> contextOptions) : base(contextOptions)
        {
            
        }

        public DbSet<Authtor> Authtors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authtor>().HasKey(a => a.Id);
            modelBuilder.Entity<Authtor>().HasAlternateKey(a => new { a.FirstName, a.MiddleName, a.LastName });
            modelBuilder.Entity<Authtor>().Property(a => a.FullName).HasComputedColumnSql("[LastName]+' ' +[FirstName]+' '+[MiddleName] PERSISTED");

            modelBuilder.Entity<Book>().HasOne(b => b.Authtor).WithMany(a => a.Books).HasPrincipalKey(a => new { a.FirstName, a.MiddleName, a.LastName });
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);
        }
    }
}
