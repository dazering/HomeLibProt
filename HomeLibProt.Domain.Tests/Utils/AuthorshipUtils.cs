using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class AuthorshipUtils {
    public static async Task<TestAuthorship[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, AuthorId from Authorships
";

        return await TestUtils.GetFromTestDatabase<TestAuthorship>(connection, sql);
    }
}
