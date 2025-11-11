using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.DataAccess.AuthorHierarchicalSearch;
using HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;
using HomeLibProt.Domain.Tests.Utils;
using HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch;

namespace HomeLibProt.Domain.Tests.DataAccess.AuthorHierarchicalSearch;

public class TestSearchNodes {
    [Test]
    public async Task TestInsertSearchNodesAsync() {
        var expected = new[] {
            new TestSearchNode(Id: 1, Letters: "A", AuthorsCount: 2, PreviousId: null),
            new TestSearchNode(Id: 2, Letters: "A ", AuthorsCount: 2, PreviousId: 1),
            new TestSearchNode(Id: 3, Letters: "A A", AuthorsCount: 2, PreviousId: 2)
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var searchNodeId1 = await SearchNodes.InsertSearchNodeAsync(c, new SearchNodeParam(Letters: "A", AuthorsCount: 2, PreviousId: null));
                var searchNodeId2 = await SearchNodes.InsertSearchNodeAsync(c, new SearchNodeParam(Letters: "A ", AuthorsCount: 2, PreviousId: searchNodeId1));
                await SearchNodes.InsertSearchNodeAsync(c, new SearchNodeParam(Letters: "A A", AuthorsCount: 2, PreviousId: searchNodeId2));
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, SearchNodeUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
