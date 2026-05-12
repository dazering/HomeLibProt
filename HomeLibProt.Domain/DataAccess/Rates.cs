using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Rate(long BookId, long AvgRate);

public record RateEntityParam(long BookId, long Rate);

public enum RatesCheckResult {
    BookExsists = 1,
    NoRecord
}

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

    public static async Task<RatesCheckResult> CheckIfBookExistsAsync(DbConnection connection, RateEntityParam rateEntity) {
        var sql =
            @"
with
    Book as (select max(1) as BookExsists from Books where Id = @BookId)
select
    case
        when Book.BookExsists then 1
        else 2
    end as Result
from Book
";

        return await connection.QuerySingleAsync<RatesCheckResult>(sql, new { BookId = rateEntity.BookId });
    }
}
