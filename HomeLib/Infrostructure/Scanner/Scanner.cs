using HomeLib.Infrostructure;
using HomeLib.Models;
using HomeLib.Models.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        private string pathToLocalRepository; //path to local repository
        private ILibraryRepository libraryRepository; //data base
        private static string currentArchive; // name of the archive with the books
        private static string currentFile; // name of book file
        private Status status;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellationToken;
        private IServiceProvider serviceProvider;

        public Scanner(string path, IServiceProvider provider)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            pathToLocalRepository = path;
            serviceProvider = provider;
            status = new Status() { CountBookInLocalRepository = BooksInLocalRepository(), StateScanner = StateScanner.Unscanned };
        }
        /// <summary>
        /// count books
        /// </summary>
        /// <returns>amount books in local repository</returns>
        private int BooksInLocalRepository()
        {
            int count = 0;
            ScanDirectory(entry => { count++; });
            return count;
        }

        public Status GetStatus()
        {
            libraryRepository = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<LibraryRepository>();
            status.UpdateCountBookInDb(libraryRepository.CountBooks());
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
        public void StartScanAsync(ILibraryRepository repo)
        {
            libraryRepository = repo;
            InitialScan();
        }

        private Task InitialScan()
        {
            if (status.CountBookInLocalRepository > libraryRepository.CountBooks() && !(status.StateScanner == StateScanner.InProgress))
            {
                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
                return Task.Factory.StartNew((_) => { Scan(); }, TaskCreationOptions.LongRunning, cancellationToken);
            }
            return new Task(() => Console.WriteLine("Action "));
        }
        private void Scan()
        {
            try
            {
                Stopwatch timer = Stopwatch.StartNew();
                status.StateScanner = StateScanner.InProgress;
                ScanDirectory(entry =>
                {
                    using (var context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<LibraryRepository>())
                    {
                        currentFile = entry.Name;
                        using (Stream entryStream = entry.Open())
                        {
                            context.AddBook(ReadBook(entryStream));
                        }
                    }
                });
                timer.Stop();
                status.GetTime(timer.Elapsed);
                status.StateScanner = StateScanner.Scanned;
            }
            catch (OperationCanceledException)
            {
                status.StateScanner = StateScanner.Canceled;
            }
        }

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
                currentArchive = archive.Name;
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
            StringBuilder fullName = new StringBuilder();
            while (iterator.MoveNext())
            {
                XPathNodeIterator descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "first-name") { string name = descendants.Current.Value.NameToUpperFirstLiteral(); fullName.Append(name + " "); book.Authtor.FirstName = name; }
                    if (descendants.Current.Name == "middle-name") { string name = descendants.Current.Value.NameToUpperFirstLiteral(); fullName.Append(name); book.Authtor.MiddleName = name; }
                    if (descendants.Current.Name == "last-name") { string name = descendants.Current.Value.NameToUpperFirstLiteral(); fullName.Insert(0, name + " "); book.Authtor.LastName = name; }
                    if (descendants.Current.Name == "book-title") { book.Title = descendants.Current.Value; }
                    if (descendants.Current.Name == "annotation") { book.Annotation = descendants.Current.Value; }
                }
            }
            book.Authtor.FullName = fullName.ToString();
            iterator = navigator.Select("//ns:publish-info", manager);
            while (iterator.MoveNext())
            {
                XPathNodeIterator descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "year") { book.Year = descendants.Current.Value; }
                    if (descendants.Current.Name == "isbn") { book.Isbn = descendants.Current.Value; }
                }
            }
            iterator = navigator.Select("//ns:binary[@id='cover.jpg']", manager);
            if (iterator.MoveNext()) { book.Cover = iterator.Current.Value; }

            book.PathArchive = currentArchive;
            book.PathBook = currentFile;
            return book;
        }
    }
}

