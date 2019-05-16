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
            modelBuilder.Entity<Authtor>().HasKey(a => new { a.FirstName, a.LastName });
        }
    }
}
