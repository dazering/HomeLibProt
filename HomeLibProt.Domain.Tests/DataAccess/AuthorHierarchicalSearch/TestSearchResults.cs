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
            var (nodeId, author1Id, author2Id) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var nodeId = await SearchNodeUtils.Create(c, "A", 2, null);

                var author1Id = await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");
                var author2Id = await AuthorUtils.Create(connection, fullName: "A B B", lastName: "A", firstName: "B", middleName: "B");

                return (nodeId, author1Id, author2Id);
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await SearchResults.InsertSearchResultAsync(c, new SearchResultParam(NodeId: nodeId, AuthorId: author1Id));
                await SearchResults.InsertSearchResultAsync(c, new SearchResultParam(NodeId: nodeId, AuthorId: author2Id));
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, SearchResultUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
