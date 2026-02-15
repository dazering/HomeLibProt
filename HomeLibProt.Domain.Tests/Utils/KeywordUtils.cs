using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class KeywordUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string name) {
        var sql = @"
insert into
Keywords (Name)
values (@Name)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            Name = name
        });
    }

    public static async Task<TestKeyword[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Name from Keywords
";

        return await TestUtils.GetFromTestDatabase<TestKeyword>(connection, sql);
    }
}
