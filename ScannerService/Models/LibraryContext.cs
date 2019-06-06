using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ScannerService.Models
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Authtor> Authtors { get; set; }
        public virtual DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Library;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authtor>().HasKey(a => a.Id);
            modelBuilder.Entity<Authtor>().HasAlternateKey(a => new { a.FirstName, a.MiddleName, a.LastName });
            modelBuilder.Entity<Authtor>().Property(a => a.FirstName).HasDefaultValue("");
            modelBuilder.Entity<Authtor>().Property(a => a.MiddleName).HasDefaultValue("");
            modelBuilder.Entity<Authtor>().Property(a => a.LastName).HasDefaultValue("");
            modelBuilder.Entity<Authtor>().HasIndex(a => a.FullName);

            modelBuilder.Entity<Book>().HasOne(b => b.Authtor).WithMany(a => a.Books).HasPrincipalKey(a => new { a.FirstName, a.MiddleName, a.LastName });
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);
        }
    }
}
