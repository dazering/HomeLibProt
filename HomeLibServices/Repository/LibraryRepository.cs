using HomeLibServices.DataBase;
using HomeLibServices.Models;
using HomeLibServices.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HomeLibServices.Repository
{
    public class LibraryRepository : ILibraryRepository
    {
        public LibraryRepository(LibraryContext cntx)
        {
            context = cntx;
        }

        private LibraryContext context;

        public bool AddBook(Book book)
        {
            if (IsDbContainingBook(book))
            {
                return false;
            }

            var authorsFromDb = getAuthorsByName(book);
            if (authorsFromDb.Any())
            {
                foreach (var authorship in book.Authorships)
                {
                    authorship.AuthorId = authorsFromDb.FirstOrDefault(a => a.FullName == authorship.Author.FullName)
                        ?.AuthorId ?? 0;
                }
            }
            context.Add(book);
            context.SaveChanges();
            return true;
        }

        private string GetAuthorsForQuery(Book book)
        {
            StringBuilder query = new StringBuilder();
            query.AppendJoin(", ", Enumerable.Range(1, book.Authorships.Count).Select((a) => $"@{a}"));

            return query.ToString();
        }

        private List<SqlParameter> GetAuthorsSqlParameters(List<SqlParameter> parameters, Book book)
        {
            parameters.AddRange(book.Authorships.Select((a, i) => new SqlParameter($"@{i + 1}", a.Author.FullName)));
            return parameters;
        }

        private bool IsDbContainingBook(Book book)
        {
            return context.Books.Any(b => b.Title == book.Title && b.Path.FbName == book.Path.FbName && b.Path.ArchiveName == book.Path.ArchiveName);
        }

        private IEnumerable<Author> getAuthorsByName(Book book)
        {
            return context.Authors
                .FromSql($"SELECT * FROM Authors a WHERE a.FullName IN({GetAuthorsForQuery(book)})", GetAuthorsSqlParameters(new List<SqlParameter>(), book).ToArray()).ToList();
        }

        public Book GetBook(long id)
        {
            return context.Books.Include(b => b.Authorships).ThenInclude(a => a.Author).FirstOrDefault(b => b.BookId == id);
        }

        public Author GetAuthor(long id)
        {
            return context.Authors.Include(a => a.Authorships).ThenInclude(b => b.Book).FirstOrDefault(a => a.AuthorId == id);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            context.Books.Load();
            return context.Books;
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            context.Authors.Load();
            return context.Authors;
        }

        public int CountBooks()
        {
            context.Books.Load();
            return context.Books.Count();
        }

        public IEnumerable<(string, int)> GetAuthorsByFirstLiteral(string firstLiterals)
        {
            return context.Authors.Where(a => a.FullName.StartsWith(firstLiterals))
                .GroupBy(a => a.FullName.Substring(0, firstLiterals.Length + 1)).Select(a =>
                    new System.Tuple<string, int>(a.Key, a.Count()).ToValueTuple())
                .ToList();
        }

        public IEnumerable<Book> SearchBookByTitle(string searchTerm)
        {
            return new SearchResult<Book>(context.Books, "Title", searchTerm);
        }

        public IEnumerable<Author> SearchAuthorByName(string searchTerm)
        {
            var result = new SearchResult<Author>(context.Authors.Include(a=>a.Authorships), "FullName", searchTerm);
            return result;
        }
    }
}
