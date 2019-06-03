using HomeLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace HomeLib.Infrostructure.Scanner
{
    public class Downloader
    {
        private string pathToLocalRepository;
        public Downloader(string path)
        {
            pathToLocalRepository = path;
        }

        public byte[] GetBytes(Book book)
        {
            return ScanDirectory(book);
        }

        private byte[] ScanDirectory(Book book)
        {
            DirectoryInfo localRepository = new DirectoryInfo(pathToLocalRepository);
            FileInfo[] zipFiles = localRepository.GetFiles(book.PathArchive, SearchOption.TopDirectoryOnly);
            return ScanArchive(zipFiles[0].Name, book);
        }
        private byte[] ScanArchive(string archiveName, Book book)
        {
            using (FileStream zipStream = File.OpenRead(Path.Combine(pathToLocalRepository, archiveName)))
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = zipArchive.Entries.Where(e => e.Name == book.PathBook).First();
                using (var stream = entry.Open())
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
