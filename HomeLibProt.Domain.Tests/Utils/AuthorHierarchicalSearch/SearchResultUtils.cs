using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;

namespace HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch;

public static class SearchResultUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Authors (Id, FullName, FirstName, MiddleName, LastName)
values
(1, 'A A A', 'A', 'A', 'A'),
(2, 'A B B', 'A', 'B', 'B');

insert into
AuthorHierarchicalSearchNodes (Id, Letters, AuthorsCount, PreviousId)
values
(1, 'A', 2, null)
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestSearchResult[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, NodeId, AuthorId from AuthorHierarchicalSearchResults
";

        return await TestUtils.GetFromTestDatabase<TestSearchResult>(connection, sql);
    }
}
