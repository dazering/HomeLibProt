using HomeLibServices.FileSystem;
using HomeLibServices.Logger;
using HomeLibServices.Models;
using HomeLibServices.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Xml;

namespace HomeLibServices.Managers
{
    internal class Scanner
    {
        private readonly string pathToLocalRepository;
        private readonly LocalRepositoryReader repositoryReader;
        private readonly FbReader bookReader;
        private CancellationToken token;
        private readonly IServiceProvider provider;
        private ILogger logger;
        private ScannerState scannerState;

        public event EventHandler<ScannerEventArgs> ChangedScanningState;
        public event EventHandler<ScannerEventArgs> ScanningOver;

        public Scanner(string path, IServiceProvider prov)
        {
            pathToLocalRepository = path;
            bookReader = new FbReader();
            scannerState = new ScannerState();
            provider = prov;
            logger = provider.GetRequiredService<ILogger>();
            repositoryReader = new LocalRepositoryReader((archive, s) =>
            {
                string currArchive = s.Replace(pathToLocalRepository, "");

                foreach (var zipArchiveEntry in archive.Entries)
                {
                    string currFileBook = zipArchiveEntry.FullName;
                    try
                    {
                        using (var zipStream = zipArchiveEntry.Open())
                        {
                            token.ThrowIfCancellationRequested();
                            var newBook = ReadArchiveEntity(zipStream);
                            newBook.Path.FbName = currFileBook;
                            newBook.Path.ArchiveName = currArchive;
                            AddBookToRepository(newBook);
                        }
                    }
                    catch (XmlException)
                    {
                        scannerState.BooksNotAddedInDb++;
                        scannerState.CurrentErrorsCount++;
                    }
                    catch (SqlException e)
                    {
                        scannerState.CurrentErrorsCount++;
                        scannerState.BooksNotAddedInDb++;
                        logger.WriteError(e, currArchive, currFileBook);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch (Exception e)
                    {
                        scannerState.CurrentErrorsCount++;
                        logger.WriteError(e, currArchive, currFileBook);
                    }
                }
            });
        }

        public void ReadLocalRepository(CancellationToken tkn)
        {
            token = tkn;
            scannerState.SetStartTime();
            scannerState.IsScanningRun = true;

            try
            {
                repositoryReader.ReadLocalRepository(pathToLocalRepository);
            }
            catch (OperationCanceledException)
            {
                scannerState.SetFinishTime();
                scannerState.IsScanningRun = false;
                ScanningOver?.Invoke(this, new ScannerEventArgs(scannerState));
            }

        }

        private Book ReadArchiveEntity(Stream entityStream)
        {
            return bookReader.ReadBook(entityStream);
        }

        private void AddBookToRepository(Book newBook)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                if (scope.ServiceProvider.GetService<ILibraryRepository>().AddBook(newBook))
                {
                    ChangedScanningState?.Invoke(this, new ScannerEventArgs(scannerState));
                    scannerState.BooksInDataBase++;
                }
            }
        }

        public int CountBooksInLocalRepository()
        {
            int booksCount = 0;
            var localBooksCounter = new LocalRepositoryReader((archive, s) => booksCount += archive.Entries.Count);
            localBooksCounter.ReadLocalRepository(pathToLocalRepository);
            scannerState.BooksInLocalRepository = booksCount;
            return booksCount;
        }

        public int CountBooksInDataBase()
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                scannerState.BooksInDataBase = scope.ServiceProvider.GetService<ILibraryRepository>().CountBooks();
                return scannerState.BooksInDataBase;
            }
        }

        public bool IsScanningRun()
        {
            return scannerState.IsScanningRun;
        }

        public ScannerState GetScannerState()
        {
            scannerState.BooksInDataBase = CountBooksInDataBase();
            scannerState.BooksInLocalRepository = CountBooksInLocalRepository();
            return scannerState;
        }
    }
}
