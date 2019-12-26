using System;
using System.IO;
using System.IO.Compression;

namespace HomeLibServices.FileSystem
{
    /// <summary>
    /// Class contains functionality for reading Directory, Archives and Files in Archives
    /// </summary>
    internal class LocalRepositoryReader
    {
        /// <summary>
        /// Delegate containing action with archives entities
        /// </summary>
        private Action<ZipArchive> action;
        /// <summary>
        /// Constructor accept delegate 
        /// </summary>
        /// <param name="act">Action with archives entities</param>
        internal LocalRepositoryReader(Action<ZipArchive> act)
        {
            action = act;
        }
        /// <summary>
        /// Initiate reading repository
        /// </summary>
        /// <param name="pathToRepository">Path to repository</param>
        internal void ReadLocalRepository(string pathToRepository)
        {
            ReadDirectory(pathToRepository);
        }
        /// <summary>
        /// Get full path to archive in directory
        /// </summary>
        /// <param name="pathToRepository">Path to repository</param>
        private void ReadDirectory(string pathToRepository)
        {
            var zipFiles = Directory.GetFiles(pathToRepository, "*.zip", SearchOption.TopDirectoryOnly);
            foreach (var zipFile in zipFiles)
            {
                ReadArchive(zipFile);
            }
        }
        /// <summary>
        /// Open archive and processing(action) each archived file  
        /// </summary>
        /// <param name="pathToArchive">Path to archive</param>
        private void ReadArchive(string pathToArchive)
        {
            using (FileStream zipStream = File.OpenRead(pathToArchive))
            using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                action(zipArchive);
            }
        }
    }
}
