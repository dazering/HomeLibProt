using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScannerService.Models;

namespace ScannerService.Repository
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext context;
        public LibraryRepository(LibraryContext cntx) => context = cntx;

        public void AddBook(Book newBook)
        {
            
            if (IsDatabaseContainsBook(newBook))
            {
                return;
            }
            if (IsDatabaseContainsAuthtor(newBook))
            {
                context.Authtors.Where(a => a.FirstName == newBook.Authtor.FirstName && a.MiddleName == newBook.Authtor.MiddleName && a.LastName == newBook.Authtor.LastName).Load();
                context.Update(newBook);
            }
            else
            {
                context.Add(newBook);
            }
            context.SaveChanges();
        }

        public Book GetBook(long id)
        {
            return context.Books.Include(b=>b.Authtor).Where(b=>b.Id==id).FirstOrDefault();
        }

        public IEnumerable<Book> GetAllBook()
        {
            return context.Books.Include(b => b.Authtor).ToList();
        }

        private bool IsDatabaseContainsBook(Book newBook)
        {
            if (context.Books.Count(b => b.Title == newBook.Title && b.Authtor.FirstName == newBook.Authtor.FirstName && b.Authtor.MiddleName == newBook.Authtor.MiddleName && b.Authtor.LastName == newBook.Authtor.LastName) == 0)
            {
                return false;
            }
            return true;
        }

        private bool IsDatabaseContainsAuthtor(Book newBook)
        {
            if (context.Authtors.Count(a => (a.FirstName == newBook.Authtor.FirstName) && (a.MiddleName == newBook.Authtor.MiddleName) && (a.LastName == newBook.Authtor.LastName)) == 0)
            {
                return false;
            }
            return true;
        }

        public int CountBooks()
        {
            IQueryable<Book> books = context.Books;
            return books.Count();
        }

        public void Dispose()
        {
        }
    }
}
