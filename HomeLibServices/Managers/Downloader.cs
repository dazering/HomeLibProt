using System.IO;
using System.IO.Compression;
using System.Linq;

namespace HomeLibServices.Managers
{
    internal class Downloader
    {
        private readonly string pathToLocalRepository;

        public Downloader(string path)
        {
            pathToLocalRepository = path;
        }

        internal byte[] ReadArchive(string archiveName, string fileName)
        {
            using (FileStream zipStream = File.OpenRead(Path.Combine(pathToLocalRepository, archiveName)))
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault(e => e.Name == fileName);
                using (var stream = zipArchiveEntry.Open())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    byte[] bytesOfBook = ms.ToArray();
                    return bytesOfBook;
                }
            }
        }
    }
}
