using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookParam(string Title, string FileName, long Size, string LibId, bool Deleted, string Extension, string Date, long ArchiveId, int? LibRate, long LanguageId);

public record ArchiveEntity(string FileName, string Extension);

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
    ArchiveId,
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
    @ArchiveId,
    @LibRate,
    @LanguageId)
returning Id
";

        return await connection.QuerySingleAsync<long>(sql, bookParam);
    }

    public static IAsyncEnumerable<ArchiveEntity> GetArchiveEntitiesByArchiveIdAsync(DbConnection connection, long archiveId) {
        var sql =
            @"
select FileName, Extension from Books
where ArchiveId = @ArchiveId
order by FileName
";

        return connection.QueryUnbufferedAsync<ArchiveEntity>(sql, new { ArchiveId = archiveId });
    }
}
