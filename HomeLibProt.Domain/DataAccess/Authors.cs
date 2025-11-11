using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Author(long Id, string Name);

public record AuthorParam(string FullName, string LastName, string FirstName, string MiddleName);

public static class Authors {
    public static async Task<Author[]> GetAuthorsByNameAsync(DbConnection connection, string[] authorNames) {
        var sql =
            @"
select Id, FullName as Name from Authors
where FullName in @Names
";

        var authors = await connection.QueryAsync<Author>(sql, new { Names = authorNames });

        return authors.ToArray();
    }

    public static async Task InsertAuthorsAsync(DbConnection connection, AuthorParam[] authorNames) {
        var sql =
            @"
insert into
Authors (FullName, FirstName, MiddleName, LastName)
values (@FullName, @FirstName, @MiddleName, @LastName)
";

        foreach (var a in authorNames) {
            var _ = await connection.ExecuteAsync(sql, a);
        }
    }

    public static IAsyncEnumerable<Author> GetAuthorsAsync(DbConnection connection) {
        var sql =
            @"
select Id, FullName as Name from Authors
order by FullName
";

        return connection.QueryUnbufferedAsync<Author>(sql);
    }
}
