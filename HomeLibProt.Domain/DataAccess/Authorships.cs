using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record AuthorshipParam(long BookId, long AuthorId);

public static class Authorships {
    public static async Task InsertAuthorshipsAsync(DbConnection connection, AuthorshipParam[] authorshipParams) {
        var sql =
            @"
insert into
Authorships (BookId, AuthorId)
values (@BookId, @AuthorId)
";

        foreach (var asp in authorshipParams) {
            var _ = await connection.ExecuteAsync(sql, asp);
        }
    }
}
