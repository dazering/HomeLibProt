using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookParam(string Title, string FileName, long Size, string LibId, bool Deleted, string Extension, string Date, long ArchiveId, int? LibRate, long LanguageId);

public record ArchiveEntity(string FileName, string Extension);

public record BookEntityParam(long Id, string Title, string FileName, long Size, string LibId, bool Deleted, string Extension, string Date, long ArchiveId, int? LibRate, long LanguageId);

public record BookInpxInfo(string Title, string? Series, long? SeriesNumber, long Deleted, string Extension, string Date, string Language);

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

    public static async Task InsertBookEntityAsync(DbConnection connection, BookEntityParam bookEntity) {
        var sql =
            @"
insert into
Books
    (Id,
    Title,
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
    (@Id,
    @Title,
    @FileName,
    @Size,
    @LibId,
    @Deleted,
    @Extension,
    @Date,
    @ArchiveId,
    @LibRate,
    @LanguageId)
";

        await connection.ExecuteAsync(sql, bookEntity);
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

    public static async Task<BookInpxInfo[]> GetBookInpxInfosByIdAsync(DbConnection connection, long bookId) {
        var sql =
            @"
select
  b.Title,
  s.Name as Series,
  bs.SeriesNumber,
  b.Deleted,
  b.Extension,
  b.Date,
  l.Name as Language
from Books b
left join BookSeries bs on bs.BookId = b.Id
left join Series s on s.Id = bs.SeriesId
inner join Languages l on l.Id = b.LanguageId
where b.Id = @BookId
";

        var bookInpxInfos = await connection.QueryAsync<BookInpxInfo>(sql, new { BookId = bookId });

        return bookInpxInfos.ToArray();
    }
}
