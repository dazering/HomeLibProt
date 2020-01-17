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
            scannerState = new ScannerState(CountBooksInDataBase(), CountBooksInLocalRepository());
            provider = prov;
            logger = provider.GetService<ILogger>();
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
                            var newBook = ReadArchiveEntity(zipStream);
                            newBook.Path.FbName = currFileBook;
                            newBook.Path.ArchiveName = currArchive;
                            AddBookToRepository(newBook);
                            token.ThrowIfCancellationRequested();
                        }
                    }
                    catch (XmlException)
                    {
                        scannerState.BooksNotAddedInDb++;
                        scannerState.CurrentErrorsCount++;
                        scannerState.BooksInLocalRepository--;
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
            scannerState.StartTime = DateTime.Now;
            scannerState.IsScanningRun = true;

            try
            {
                repositoryReader.ReadLocalRepository(pathToLocalRepository);
            }
            catch (OperationCanceledException)
            {
                scannerState.FinishTime = DateTime.Now;
                scannerState.ElapsedTime = scannerState.FinishTime - scannerState.StartTime;
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
                scope.ServiceProvider.GetService<ILibraryRepository>().AddBook(newBook);
                ChangedScanningState?.Invoke(this, new ScannerEventArgs(scannerState));
                scannerState.BooksInDataBase++;
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

        public ScannerState GetScannerState()
        {
            return scannerState;
        }
    }
}
