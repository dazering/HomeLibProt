using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookParam(string Title, string FileName, long Size, string LibId, bool Deleted, string Extension, string Date, string Folder, int? LibRate, long LanguageId);

public record FolderEntity(string FileName, string Extension);

public static class Books {
    public static async Task<long> InsertBookAsync(DbConnection connection, BookParam bookParam) {
        var sql =
            @"
insert into
Books
    (Title,
    FileName,
    Size,
    LibId,
    Deleted,
    Extension,
    Date,
    Folder,
    LibRate,
    LanguageId)
values
    (@Title,
    @FileName,
    @Size,
    @LibId,
    @Deleted,
    @Extension,
    @Date,
    @Folder,
    @LibRate,
    @LanguageId)
returning Id
";

        return await connection.QuerySingleAsync<long>(sql, bookParam);
    }

    public static IAsyncEnumerable<string> GetFoldersAsync(DbConnection connection) {
        var sql =
            @"
select distinct Folder from Books
order by Folder
";

        return connection.QueryUnbufferedAsync<string>(sql);
    }

    public static IAsyncEnumerable<FolderEntity> GetFolderEntitiesByFolderAsync(DbConnection connection, string folder) {
        var sql =
            @"
select FileName, Extension from Books
where Folder = @Folder
order by FileName
";

        return connection.QueryUnbufferedAsync<FolderEntity>(sql, new { Folder = folder });
    }
}
