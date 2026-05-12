using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Rate(long BookId, long AvgRate);

public record RateEntityParam(long BookId, long Rate);

public static class Rates {
    public static async Task<Rate> GetAvgRateByBookIdAsync(DbConnection connection, long bookId) {
        var sql =
            @"
select BookId, cast(round(avg(Rate)) as integer) as AvgRate from Rates
where BookId = @BookId
group by BookId
";

        return await connection.QuerySingleAsync<Rate>(sql, new { BookId = bookId });
    }

    public static async Task InsertRateEntityAsync(DbConnection connection, RateEntityParam rate) {
        var sql =
            @"
insert into
Rates (BookId, Rate)
values (@BookId, @Rate)
";

        var _ = await connection.ExecuteAsync(sql, rate);
    }
}
