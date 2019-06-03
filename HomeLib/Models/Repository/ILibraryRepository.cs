using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeLib.Models.Pages;
namespace HomeLib.Models.Repository
{
    public interface ILibraryRepository : IDisposable
    {
        void AddBook(Book newBook);
        Book GetBook(long id);
        IEnumerable<Book> GetAllBook();
        IEnumerable<MetaInfo> SearchAuthors(QueryOptions options);
        IEnumerable<Book> SearchBooksByAuthor(QueryOptions options);
        IEnumerable<MetaInfo> SearchAuthorByFirstLiterals(QueryOptions options);
        IEnumerable<MetaInfo> GetAuthtors(SearchOptions options);
        SearchResult<Book> GetBooks(SearchOptions options);
        int CountBooks();
    }
}
