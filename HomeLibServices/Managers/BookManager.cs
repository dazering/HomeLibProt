﻿using HomeLibServices.Models;
using HomeLibServices.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HomeLibServices.Managers
{
    internal class BookManager
    {
        private readonly ScannerTasker scanner;
        private readonly Downloader downloader;
        private readonly IServiceProvider serviceProvider;
        public event EventHandler<ScannerEventArgs> ScannerMessage;

        public BookManager(string path, IServiceProvider provider)
        {
            scanner = new ScannerTasker(path, provider);
            scanner.ScnanningStateChanged += SendMessage;
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

        public Book GetBook(long id)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetBook(id);
            }
        }

        public Author GetAuthor(long id)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetAuthor(id);
            }
        }

        public IEnumerable<Book> GetAllBooks()
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetAllBooks();
            }
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetAllAuthors();
            }
        }

        public IEnumerable<(string, int)> GetAuthorsByFirstLiteral(string firstLiterals)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).GetAuthorsByFirstLiteral(firstLiterals);
            }
        }

        public IEnumerable<Book> SearchBooksByTitle(string searchTerm)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).SearchBookByTitle(searchTerm);
            }
        }

        public IEnumerable<Author> SearchAuthorsByName(string searchTertm)
        {
            using (var scope = getScope())
            {
                return getLibraryRepository(scope).SearchAuthorByName(searchTertm);
            }
        }

        #endregion

        public void StartUpdateDbRepository()
        {
            scanner.TryStartScanning();
        }

        #region DownloadBook

        public byte[] GetBookBytes(Book book)
        {
            return GetBookBytes(book.Path.ArchiveName, book.Path.FbName);
        }

        public byte[] GetBookBytes(string archiveName, string fileName)
        {
            return downloader.ReadArchive(archiveName, fileName);
        }

        #endregion

        private void SendMessage(object obj, ScannerEventArgs e)
        {
            ScannerMessage?.Invoke(this, e);
        }
    }
}