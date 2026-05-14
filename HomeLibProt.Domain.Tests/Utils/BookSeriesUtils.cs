using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookSeriesUtils {
    public static async Task<long> Create(DbConnection connection,
                                              long bookId,
                                              long seriesId,
                                              long seriesNumber) {
        var sql = @"
insert into
BookSeries (BookId, SeriesId, SeriesNumber)
values (@BookId, @SeriesId, @SeriesNumber)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            BookId = bookId,
            SeriesId = seriesId,
            SeriesNumber = seriesNumber
        });
    }

    public static async Task<TestBookSeriesEntity[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, SeriesId, SeriesNumber from BookSeries
";

        return await TestUtils.GetFromTestDatabase<TestBookSeriesEntity>(connection, sql);
    }
}
