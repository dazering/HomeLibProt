using HomeLibServices.Models;
using HomeLibServices.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HomeLibServices.Managers
{
    public class BookManager
    {
        private readonly Downloader downloader;
        private readonly IServiceProvider serviceProvider;

        public BookManager(string path, IServiceProvider provider)
        {
            downloader = new Downloader(path);
            serviceProvider = provider;
        }

        #region serviceLocator

        private IServiceScope getScope()
        {
            return serviceProvider.CreateScope();
        }

        private ILibraryRepository getLibraryRepository(IServiceScope scope)
        {
            return scope.ServiceProvider.GetRequiredService<ILibraryRepository>();
        }

        #endregion

        #region LibaryRepository
        /// <inheritdoc cref="LibraryRepository.GetBook"/>
        public Book GetBook(long id)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetBook(id);
            }
        }

        /// <inheritdoc cref="LibraryRepository.GetAuthor"/>
        public Author GetAuthor(long id)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetAuthor(id);
            }
        }

        /// <inheritdoc cref="LibraryRepository.GetAuthorsFirstLiteral"/>
        public IEnumerable<(string, int)> GetAuthorsFirstLiteral(string firstLiterals)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetAuthorsFirstLiteral(firstLiterals);
            }
        }

        /// <inheritdoc cref="LibraryRepository.SearchBookByTitle"/>
        public IEnumerable<Book> SearchBooksByTitle(string searchTerm)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).SearchBookByTitle(searchTerm);
            }
        }

        /// <inheritdoc cref="LibraryRepository.SearchAuthorByName"/>
        public IEnumerable<Author> SearchAuthorsByName(string searchTertm)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).SearchAuthorByName(searchTertm);
            }
        }

        /// <inheritdoc cref="LibraryRepository.SearchAuthorsByFirstLiteral"/>
        public IEnumerable<Author> SearchAuthorsByFirstLiteral(string searchTertm)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).SearchAuthorsByFirstLiteral(searchTertm);
            }
        }

        #endregion

        #region DownloadBook
        /// <summary>
        /// Return bytes of .fb2 file
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        /// <exception cref="System.Data.SqlClient.SqlException">Thrown when database is not access.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when file no found of physical driver.</exception>
        public byte[] GetBookBytes(Book book)
        {
            return GetBookBytes(book.Path.ArchiveName, book.Path.FbName);
        }
        
        /// <inheritdoc cref="GetBookBytes(HomeLibServices.Models.Book)"/>
        /// <param name="archiveName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] GetBookBytes(string archiveName, string fileName)
        {
            return downloader.ReadArchive(archiveName, fileName);
        }

        #endregion
    }
}
