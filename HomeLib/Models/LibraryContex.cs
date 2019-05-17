using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HomeLib.Models
{
    public class LibraryContex : DbContext
    {
        public LibraryContex(DbContextOptions<LibraryContex> contextOptions) : base(contextOptions)
        {
            
        }

        public DbSet<Authtor> Authtors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authtor>().HasKey(a => new { a.FirstName,a.MiddleName, a.LastName });
            modelBuilder.Entity<Authtor>().Property(a => a.MiddleName).HasDefaultValue("none");

            modelBuilder.Entity<Book>().HasOne(b => b.Authtor).WithMany(b => b.Books).IsRequired();
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);
        }
    }
}
