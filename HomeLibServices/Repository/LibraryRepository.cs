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

        /// <summary>
        /// Add <paramref name="book"/> to database
        /// </summary>
        /// <param name="book"></param>
        /// <returns>If adding was successful return true, else false</returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public bool AddBook(Book book)
        {
            if (IsDbContainingBook(book))
            {
                return false;
            }

            if (book.Authorships.Any(a => a.Author.FullName.Length < 4))
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

        /// <summary>
        /// Forming string for sql query from <paramref name="book"/> authors
        /// </summary>
        /// <param name="book"></param>
        /// <returns>String with sql query</returns>
        private string GetAuthorsForQuery(Book book)
        {
            StringBuilder query = new StringBuilder();
            query.AppendJoin(", ", Enumerable.Range(1, book.Authorships.Count).Select((a) => $"@{a}"));

            return query.ToString();
        }

        /// <summary>
        /// Forming <paramref name="parameters"/> from authors in <paramref name="book"/>
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="book"></param>
        /// <returns>Fulfilled <paramref name="parameters"/></returns>
        private List<SqlParameter> GetAuthorsSqlParameters(List<SqlParameter> parameters, Book book)
        {
            parameters.AddRange(book.Authorships.Select((a, i) => new SqlParameter($"@{i + 1}", a.Author.FullName)));
            return parameters;
        }

        /// <summary>
        /// Checks is <paramref name="book"/> exists in database
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        private bool IsDbContainingBook(Book book)
        {
            return context.Books.Any(b => b.Title == book.Title && b.Path.FbName == book.Path.FbName && b.Path.ArchiveName == book.Path.ArchiveName);
        }

        private IEnumerable<Author> getAuthorsByName(Book book)
        {
            return context.Authors
                .FromSql($"SELECT * FROM Authors a WHERE a.FullName IN({GetAuthorsForQuery(book)})", GetAuthorsSqlParameters(new List<SqlParameter>(), book).ToArray()).ToList();
        }

        /// <summary>
        /// Get Book from database by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public Book GetBook(long id)
        {
            return context.Books.Include(b => b.Authorships).ThenInclude(a => a.Author).FirstOrDefault(b => b.BookId == id);
        }

        /// <summary>
        /// Get Author from database by <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public Author GetAuthor(long id)
        {
            return context.Authors.Include(a => a.Authorships).ThenInclude(b => b.Book).FirstOrDefault(a => a.AuthorId == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public int CountBooks()
        {
            return context.Books.Count();
        }

        /// <summary>
        /// Get first literals and count authors names
        /// </summary>
        /// <param name="firstLiterals"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public IEnumerable<AlphabetSearchResult> GetAuthorsFirstLiteral(string firstLiterals)
        {
            return context.Authors.Where(a => a.FullName.StartsWith(firstLiterals))
                .GroupBy(a => a.FullName.Substring(0, firstLiterals.Length + 1)).Select(a =>
                    new AlphabetSearchResult { Alphabets = a.Key, Count = a.Count() })
                .ToList();
        }

        /// <summary>
        /// Search book by title
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public IEnumerable<Book> SearchBookByTitle(string searchTerm)
        {
            return new SearchResult<Book>(context.Books, "Title", searchTerm);
        }

        /// <summary>
        /// Fulltext search author by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public IEnumerable<Author> SearchAuthorByName(string searchTerm)
        {
            var result = new SearchResult<Author>(context.Authors.Include(a => a.Authorships).Select(a => new Author() { AuthorId = a.AuthorId, FullName = a.FullName }), "FullName", searchTerm);
            return result;
        }

        /// <summary>
        /// Search by first literals author name
        /// </summary>
        /// <param name="firstLiterals"></param>
        /// <returns></returns>
        /// <exception cref="SqlException">Thrown when database is not access.</exception>
        public IEnumerable<Author> SearchAuthorsByFirstLiteral(string firstLiterals)
        {
            return context.Authors.Include(a => a.Authorships).Where(a => a.FullName.StartsWith(firstLiterals)).Select(a => new Author() { FullName = a.FullName, AuthorId = a.AuthorId, Authorships = a.Authorships }).ToList();
        }
    }
}
