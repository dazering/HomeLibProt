using HomeLib.Infrostructure;
using HomeLib.Models;
using HomeLib.Models.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace HomeLib.Infrostructure.Scanner
{
    /// <summary>
    /// Service for scanning local library
    /// </summary>
    public class Scanner
    {
        private string pathToLocalRepository; //path to local repository - r
        private string currentArchive; // name of the archive with the books - r
        private string currentFile; // name of book file - r
        private Status status; // - r
        private CancellationTokenSource tokenSource; // -r
        private CancellationToken cancellationToken;// -r
        private IServiceProvider serviceProvider;// -r

        public Scanner(IServiceProvider provider)
        {
            serviceProvider = provider ?? throw new ArgumentNullException("provider");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            pathToLocalRepository = Configuration["LocalRepository:Path"];
            EnsureLocalRepositoryExist(pathToLocalRepository);
            status = new Status() { CountBookInLocalRepository = BooksInLocalRepository() };
        }

        /// <summary>
        /// Check paht to local repository. If directory not exist - create
        /// </summary>
        /// <param name="path"></param>

        private void EnsureLocalRepositoryExist(string path)
        {
            if (Directory.Exists(path))
            {
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        #region Service locator

        private IConfiguration Configuration { get; }

        private ILibraryRepository GetLibrary()
        {
            return serviceProvider.GetService<ILibraryRepository>();
        }

        #endregion

        #region Working with file names

        private string GetArchiveName()
        {
            return currentArchive;
        }

        private void SetArchiveName(string archiveName)
        {
            currentArchive = archiveName;
        }

        private string GetFileName()
        {
            return currentFile;
        }

        private void SetFileName(string fileName)
        {
            currentFile = fileName;
        }

        #endregion

        #region Count Books in Repository and in File System

        /// <summary>
        /// count books
        /// </summary>
        /// <returns>amount books in local repository</returns>
        private int BooksInLocalRepository()
        {
            int count = 0;
            ScanDirectory((entry) => { count++; });
            return count;
        }

        private int BooksInDataBase()
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ILibraryRepository>();
                return context.CountBooks();
            }
        }

        #endregion

        public Status GetStatus()
        {
            status.CountBookInLocalRepository = BooksInLocalRepository();
            status.CountBookInDataBase = BooksInDataBase();
            return status;
        }
        public void CancelScanning()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
        }

        /// <summary>
        ///initial scanning 
        /// </summary>

        public void StartScanAsync()
        {
            if (status.CountBookInLocalRepository > BooksInDataBase() && !(status.IsScannerInProgress()))
            {
                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
                Task.Factory.StartNew((_) => { Scan(); }, cancellationToken);
            }
        }

        private void Scan()
        {
            Stopwatch timer = Stopwatch.StartNew();
            status.SetStatusInProgress();
            try
            {
                ScanDirectory((entry) =>
                {
                    SetFileName(entry.Name);
                    using (Stream entryStream = entry.Open())
                    using (IServiceScope scope = serviceProvider.CreateScope())
                    {
                        try
                        {
                            var context = scope.ServiceProvider.GetService<ILibraryRepository>();
                            context.AddBook(ReadBook(entryStream));
                        }
                        catch (XmlException) { status.IncreaseNotAddedBooks(); }
                        catch (SqlException e)
                        {
                            string message = string.Format("{0} , {1} in {2}", e.Message, GetFileName(), GetArchiveName());
                            status.AddErrorMessage(message); status.IncreaseNotAddedBooks();
                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException();
                        }
                        catch (Exception e) { string message = string.Format("{0} - {1}, {2} in {3}", e.Message, e.InnerException, GetFileName(), GetArchiveName()); status.AddErrorMessage(message); status.IncreaseNotAddedBooks(); }
                    }

                });
                timer.Stop();
                status.SetTime(timer.Elapsed);
                status.SetStatusScanned();
            }
            catch (OperationCanceledException)
            {
                status.SetStatusCanceled();
                tokenSource.Dispose();
                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
            }
        }

        #region Read File System

        /// <summary>
        /// scan local directory and record name of archive
        /// </summary>
        /// <param name="action">action with entry</param>
        private void ScanDirectory(Action<ZipArchiveEntry> action)
        {
            DirectoryInfo localRepository = new DirectoryInfo(pathToLocalRepository);
            FileInfo[] zipFiles = localRepository.GetFiles("*.zip", SearchOption.TopDirectoryOnly);
            foreach (FileInfo archive in zipFiles)
            {
                SetArchiveName(archive.Name);
                ScanArchive(archive.Name, action);
            }
        }
        /// <summary>
        /// scaning archive, creating entity of book and adding to the database
        /// </summary>
        /// <param name="archiveName"></param>
        private void ScanArchive(string archiveName, Action<ZipArchiveEntry> action)
        {
            using (FileStream zipStream = File.OpenRead(Path.Combine(pathToLocalRepository, archiveName)))
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    action(entry);
                }
            }
        }

        #endregion

        #region Read book And Return object Book

        /// <summary>
        /// create entity book
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private Book ReadBook(Stream inputStream)
        {

            Book book = new Book();
            XPathDocument doc = new XPathDocument(inputStream);
            XPathNavigator navigator = doc.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("ns", "http://www.gribuser.ru/xml/fictionbook/2.0");
            XPathNodeIterator iterator = navigator.Select("//ns:title-info", manager);
            XPathNodeIterator descendants;
            bool firstAuthor = true;
            while (iterator.MoveNext())
            {
                descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "author" && firstAuthor)
                    {
                        XPathNodeIterator authorsDescendants = descendants.Current.SelectDescendants(XPathNodeType.Element, false);
                        while (authorsDescendants.MoveNext())
                        {
                            if (authorsDescendants.Current.Name == "first-name") { string name = authorsDescendants.Current.Value.FormatName(); book.Authtor.FirstName = name; }
                            if (authorsDescendants.Current.Name == "middle-name") { string name = authorsDescendants.Current.Value.FormatName(); book.Authtor.MiddleName = name; }
                            if (authorsDescendants.Current.Name == "last-name") { string name = authorsDescendants.Current.Value.FormatName(); book.Authtor.LastName = name; }
                        }
                        firstAuthor = false;
                    }
                    if (descendants.Current.Name == "book-title") { book.Title = descendants.Current.Value; }
                }
            }

            iterator = navigator.Select("//ns:isbn", manager);

            while (iterator.MoveNext())
            {
                descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "isbn")
                    {
                        book.Isbn = descendants.Current.Value;
                    }
                }
            }
            book.PathArchive = GetArchiveName();
            book.PathBook = GetFileName();
            return book;
        }

        #endregion
    }
}

