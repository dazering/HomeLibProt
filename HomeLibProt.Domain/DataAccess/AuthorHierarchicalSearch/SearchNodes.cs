using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess.AuthorHierarchicalSearch;

public record SearchNodeParam(string Letters, long AuthorsCount, long? PreviousId);

public static class SearchNodes {
    public static async Task<long> InsertSearchNodeAsync(DbConnection connection, SearchNodeParam searchNodeParam) {
        var sql =
            @"
insert into
AuthorHierarchicalSearchNodes (Letters, AuthorsCount, PreviousId)
values (@Letters, @AuthorsCount, @PreviousId)
returning Id
";

        return await connection.QuerySingleAsync<long>(sql, searchNodeParam);
    }
}
