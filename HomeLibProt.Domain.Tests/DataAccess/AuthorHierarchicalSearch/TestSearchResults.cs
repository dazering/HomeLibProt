using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.DataAccess.AuthorHierarchicalSearch;
using HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;
using HomeLibProt.Domain.Tests.Utils;
using HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch;

namespace HomeLibProt.Domain.Tests.DataAccess.AuthorHierarchicalSearch;

public class TestSearchResults {
    [Test]
    public async Task TestInsertSearchNodesAsync() {
        var expected = new[] {
            new TestSearchResult(Id: 1, NodeId: 1, AuthorId: 1),
            new TestSearchResult(Id: 2, NodeId: 1, AuthorId: 2)
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, SearchResultUtils.SetUpTestData);

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await SearchResults.InsertSearchResultAsync(c, new SearchResultParam(NodeId: 1, AuthorId: 1));
                await SearchResults.InsertSearchResultAsync(c, new SearchResultParam(NodeId: 1, AuthorId: 2));
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, SearchResultUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
