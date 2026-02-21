using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class LanguageUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string name,
                                          bool include = true) {
        var sql = @"
insert into
Languages (Name, Include)
values (@Name, @Include)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            Name = name,
            Include = include
        });
    }

    public static async Task<TestLanguage[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Name, Include from Languages
";

        return await TestUtils.GetFromTestDatabase<TestLanguage>(connection, sql);
    }
}
