using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestBookKeywords {
    [Test]
    public async Task TestInsertBookKeywordsAsync() {
        var expected = new[] {
                new TestBookKeyword(BookId: 1, KeywordId: 1)
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, BookKeywordUtils.SetUpTestData);

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await BookKeywords.InsertBookKeywordsAsync(c, [new BookKeywordParam(BookId: 1, KeywordId: 1)]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookKeywordUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
