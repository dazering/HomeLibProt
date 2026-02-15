using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class SeriesUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string name) {
        var sql = @"
insert into
Series (Name)
values (@Name)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            Name = name
        });
    }

    public static async Task<TestSeriesEntity[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Name from Series
";

        return await TestUtils.GetFromTestDatabase<TestSeriesEntity>(connection, sql);
    }
}
