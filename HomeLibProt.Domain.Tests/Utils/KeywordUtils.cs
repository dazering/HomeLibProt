using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class KeywordUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Keywords (Id, Name)
values
(1, 'Keyword 1'),
(2, 'Keyword 2')
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestKeyword[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Name from Keywords
";

        return await TestUtils.GetFromTestDatabase<TestKeyword>(connection, sql);
    }
}
