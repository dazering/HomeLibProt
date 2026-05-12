using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class RateUtils {
    public static async Task<long> Create(DbConnection connection,
                                          long bookId,
                                          long rate) {
        var sql = @"
insert into
Rates (BookId, Rate)
values (@BookId, @Rate)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            BookId = bookId,
            Rate = rate
        });
    }

    public static async Task<TestRate[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, BookId, Rate from Rates
";

        return await TestUtils.GetFromTestDatabase<TestRate>(connection, sql);
    }
}
