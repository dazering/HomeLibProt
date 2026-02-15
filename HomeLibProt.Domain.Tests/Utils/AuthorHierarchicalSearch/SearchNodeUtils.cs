using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;

namespace HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch;

public static class SearchNodeUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string letters,
                                          long authorsCount,
                                          long? previousId) {
        var sql = @"
insert into
AuthorHierarchicalSearchNodes (Letters, AuthorsCount, PreviousId)
values (@Letters, @AuthorsCount, @PreviousId)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            Letters = letters,
            AuthorsCount = authorsCount,
            PreviousId = previousId
        });
    }
    public static async Task<TestSearchNode[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Letters, AuthorsCount, PreviousId from AuthorHierarchicalSearchNodes
";

        return await TestUtils.GetFromTestDatabase<TestSearchNode>(connection, sql);
    }
}
