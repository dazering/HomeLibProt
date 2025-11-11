using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;

namespace HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch;

public static class SearchNodeUtils {
    public static async Task<TestSearchNode[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Letters, AuthorsCount, PreviousId from AuthorHierarchicalSearchNodes
";

        return await TestUtils.GetFromTestDatabase<TestSearchNode>(connection, sql);
    }
}
