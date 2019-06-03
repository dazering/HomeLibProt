using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HomeLib.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace HomeLib.Models.Repository
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

        public IEnumerable<MetaInfo> SearchAuthorByFirstLiterals(QueryOptions options)
        {
            var names = new List<MetaInfo>();
            var authors = context.Authtors.Where(a=>a.FullName.StartsWith(options.SearchFirstLiterals)).OrderBy(a=>a.FullName).ToList();
            names = authors.GroupBy(a => a.FullName.Substring(0, options.SearchFirstLiterals.Length + 1))
                .Select(a => new MetaInfo { Name = a.Key, Count = authors.Where(an=>an.FullName.StartsWith(a.Key)).Count() }).ToList();

            return names;
        }

        public IEnumerable<Book> SearchBooksByAuthor(QueryOptions options)
        {
            IEnumerable<Book> books = new List<Book>();
            var authors = context.Authtors.Where(a=>a.FullName == options.AuthorName).Include(a=>a.Books).OrderBy(a=>a.FullName).ToList();
            books = authors.Where(a => a.FullName == options.AuthorName).Select(a => a.Books).FirstOrDefault();

            return books;
        }

        public IEnumerable<MetaInfo> SearchAuthors(QueryOptions options)
        {
            var authors = new List<MetaInfo>();
            var authorhosInData = context.Authtors.Where(a => a.FullName.StartsWith(options.SearchFirstLiterals)).Include(a => a.Books).OrderBy(a=>a.FullName).ToList();
            authors = authorhosInData.GroupBy(a => a.FullName)
                .Select(a => new MetaInfo { Name = a.Key, Count = authorhosInData.Where(aid => aid.FullName == a.Key).First().Books.Count() }).ToList();

            return authors; 
        }

        public IEnumerable<MetaInfo> GetAuthtors(SearchOptions options)
        {
            var authors = new List<MetaInfo>();
            var list = new SearchResult<Authtor>(context.Authtors.Include(a=>a.Books), options);
            authors = list.OrderBy(a=>a.FullName).Select(a => new MetaInfo { Name = a.FullName, Count = a.Books.Count() }).ToList();
            return authors;
        }

        public SearchResult<Book> GetBooks(SearchOptions options)
        {
            return new SearchResult<Book>(context.Books.OrderBy(a=>a.Title), options);
        }
    }
}
