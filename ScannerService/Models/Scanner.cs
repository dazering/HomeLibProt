using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScannerService.Infrostructure;
using ScannerService.Repository;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace ScannerService.Models
{
    /// <summary>
    /// Service for scanning local library
    /// </summary>
    public class Scanner
    {
        private string pathToLocalRepository; //path to local repository
        private string currentArchive; // name of the archive with the books
        private string currentFile; // name of book file
        private Status status;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancellationToken;
        private IServiceProvider serviceProvider;

        public Scanner(IServiceProvider provider)
        {
            serviceProvider = provider ?? throw new ArgumentNullException("provider");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            pathToLocalRepository = Configuration["LocalRepository:Path"];
            status = new Status() { CountBookInLocalRepository = BooksInLocalRepository(), StateScanner = StateScanner.Unscanned };
        }

        private IConfiguration Configuration { get; }

        private ILibraryRepository GetLibrary()
        {
            return serviceProvider.GetService<ILibraryRepository>();
        }

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

        public Status GetStatus()
        {
            status.UpdateCountBookInDb(GetLibrary().CountBooks());
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
            InitialScan();
        }

        private void InitialScan()
        {
            if (status.CountBookInLocalRepository > GetLibrary().CountBooks() && !(status.StateScanner == StateScanner.InProgress))
            {
                tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
                Task.Factory.StartNew((_) => { Scan(); }, TaskCreationOptions.LongRunning, cancellationToken);
            }
        }
        private void Scan()
        {
            try
            {
                Stopwatch timer = Stopwatch.StartNew();
                status.StateScanner = StateScanner.InProgress;
                ScanDirectory((entry) =>
                {
                    SetFileName(entry.Name);
                    using (Stream entryStream = entry.Open())
                    using (var context = GetLibrary())
                    {
                        try
                        {
                            Console.WriteLine();
                            Stopwatch timerFor = Stopwatch.StartNew();
                            Console.Write(currentFile + " in " + currentArchive);
                            context.AddBook(ReadBook(entryStream));
                            timerFor.Stop();
                            Console.Write(" for: " + timerFor.Elapsed + " total: " + timer.Elapsed);
                        }
                        catch (XmlException) { }
                        catch (Exception e) { Console.Beep(415, 200); Console.WriteLine(e.InnerException + " " + e.Message + " " + currentFile + " in " + currentArchive); }
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
            bool firstAuthor = true;
            while (iterator.MoveNext())
            {
                XPathNodeIterator descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "author" && firstAuthor)
                    {
                        XPathNodeIterator authorsDescendants = descendants.Current.SelectDescendants(XPathNodeType.Element, false);
                        while (authorsDescendants.MoveNext())
                        {
                            if (authorsDescendants.Current.Name == "first-name") { string name = authorsDescendants.Current.Value.NameToUpperFirstLiteral(); fullName.Append(name); book.Authtor.FirstName = name; }
                            if (authorsDescendants.Current.Name == "middle-name") { string name = authorsDescendants.Current.Value.NameToUpperFirstLiteral(); fullName.Append(" " + name); book.Authtor.MiddleName = name; }
                            if (authorsDescendants.Current.Name == "last-name") { string name = authorsDescendants.Current.Value.NameToUpperFirstLiteral(); fullName.Insert(0, name + " "); book.Authtor.LastName = name; }
                        }
                        firstAuthor = false;
                    }
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

            book.PathArchive = GetArchiveName();
            book.PathBook = GetFileName();
            return book;
        }
    }
}