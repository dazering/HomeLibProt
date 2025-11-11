using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess.AuthorHierarchicalSearch;

public record SearchResultParam(long NodeId, long AuthorId);

public static class SearchResults {
    public static async Task InsertSearchResultAsync(DbConnection connection, SearchResultParam searchResultParam) {
        var sql =
            @"
insert into
AuthorHierarchicalSearchResults (NodeId, AuthorId)
values (@NodeId, @AuthorId)
";

        await connection.ExecuteAsync(sql, searchResultParam);
    }
}
