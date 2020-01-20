using HomeLibServices.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibServices.DataBase
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> cntx) : base(cntx)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasAlternateKey(a => new { a.FirstName, a.MiddleName, a.LastName });
            modelBuilder.Entity<Author>().Property(a => a.MiddleName).HasDefaultValue("");
            modelBuilder.Entity<Author>().HasIndex(a => a.FullName);

            modelBuilder.Entity<Authorship>().HasOne(b => b.Book).WithMany(b => b.Authorships)
                .HasForeignKey(b => b.BookId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Authorship>().HasOne(a => a.Author).WithMany(a => a.Authorships)
                .HasForeignKey(a => a.AuthorId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Authorship>().Property(a => a.AuthorId).HasColumnName("Id_Author");
            modelBuilder.Entity<Authorship>().Property(a => a.BookId).HasColumnName("Id_Book");

            modelBuilder.Entity<Book>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);
            modelBuilder.Entity<LocalPath>().HasIndex(p => p.FbName);
            modelBuilder.Entity<LocalPath>().HasIndex(p => p.ArchiveName);
            modelBuilder.Entity<Book>().Ignore(b => b.Cover);
            modelBuilder.Entity<Book>().OwnsOne<LocalPath>(l => l.Path);
        }
    }
}
