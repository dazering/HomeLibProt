using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;

namespace HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch;

public static class SearchResultUtils {
    public static async Task<TestSearchResult[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, NodeId, AuthorId from AuthorHierarchicalSearchResults
";

        return await TestUtils.GetFromTestDatabase<TestSearchResult>(connection, sql);
    }
}
