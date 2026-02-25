using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class AuthorshipUtils {
    public static async Task<long> Create(DbConnection connection,
                                          long bookId,
                                          long authorId) {
        var sql = @"
insert into
Authorships (BookId, AuthorId)
values (@BookId, @AuthorId)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            BookId = bookId,
            AuthorId = authorId
        });
    }
    public static async Task<TestAuthorship[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, AuthorId from Authorships
";

        return await TestUtils.GetFromTestDatabase<TestAuthorship>(connection, sql);
    }
}
