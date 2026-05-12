using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookSeriesParam(long BookId, long SeriesId, long SeriesNumber);

public enum BookSeriesCheckResult {
    AllExsists = 1,
    OnlyBook,
    OnlySeries,
    NoRecords
}

public static class BookSeries {
    public static async Task InsertBookSeriesAsync(DbConnection connection, BookSeriesParam bookSeriesParam) {
        var sql =
            @"
insert into
BookSeries (BookId, SeriesId, SeriesNumber)
values (@BookId, @SeriesId, @SeriesNumber)
";
        await connection.ExecuteAsync(sql, bookSeriesParam);
    }

    public static async Task<BookSeriesCheckResult> CheckIfBookSeriesExistsAsync(DbConnection connection, BookSeriesParam bookSeriesParam) {
        var sql =
            @"
with
    Book as (select max(1) as BookExsists from Books where Id = @BookId),
    SeriesResult as (select max(1) as SeriesExsists from Series where Id = @SeriesId)
select
    case
        when Book.BookExsists and SeriesResult.SeriesExsists then 1
        when Book.BookExsists then 2
        when SeriesResult.SeriesExsists then 3
        else 4
    end as Result
from Book, SeriesResult
";

        return await connection.QuerySingleAsync<BookSeriesCheckResult>(sql, new { BookId = bookSeriesParam.BookId, SeriesId = bookSeriesParam.SeriesId });
    }
}
