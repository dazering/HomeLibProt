using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookSeriesParam(long BookId, long SeriesId, long SeriesNumber);

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
}
