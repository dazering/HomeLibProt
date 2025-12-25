namespace HomeLibProt.Domain.Utils;

using System;
using System.IO.Compression;
using System.Threading.Tasks;

public static class ArchiveUtils {
    public static async Task DoWithArchiveAsync(string pathToZipArchive, Func<ZipArchive, Task> action) {
        using var zipArchive = ZipFile.OpenRead(pathToZipArchive);
        await action(zipArchive);
    }

    public static async Task<T> DoWithArchiveAsync<T>(string pathToZipArchive, Func<ZipArchive, Task<T>> action) {
        using var zipArchive = ZipFile.OpenRead(pathToZipArchive);
        return await action(zipArchive);
    }
}
