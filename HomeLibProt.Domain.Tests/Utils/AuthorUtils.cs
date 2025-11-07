using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class AuthorUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Authors (Id, FullName, FirstName, MiddleName, LastName)
values
(1, 'A A A', 'A', 'A', 'A'),
(2, 'B B B', 'B', 'B', 'B')
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestAuthor[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, FullName, FirstName, MiddleName, LastName from Authors
";

        return await TestUtils.GetFromTestDatabase<TestAuthor>(connection, sql);
    }
}
