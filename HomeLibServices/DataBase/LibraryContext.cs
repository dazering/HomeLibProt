using System;
using System.Collections.Generic;
using System.Text;
using HomeLibServices.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibServices.DataBase
{
   public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> cntx):base(cntx)
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasAlternateKey(a => new {a.FirstName, a.MiddleName, a.LastName});
            modelBuilder.Entity<Author>().Property(a => a.MiddleName).HasDefaultValue("");
            modelBuilder.Entity<Author>().HasIndex(a => a.FullName);

            modelBuilder.Entity<Book>().HasOne(b => b.Author).WithMany(a => a.Books).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);
            modelBuilder.Entity<Book>().Ignore(b => b.Cover);
            modelBuilder.Entity<Book>().OwnsOne<LocalPath>(l=>l.Path);
        }
    }
}
