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
            var (bookId, keywordId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var langId = await LanguageUtils.Create(c, "Lang 1");

                var bookId = await BookUtils.Create(c,
                                                    title: "Title1",
                                                    fileName: "File1",
                                                    size: 1,
                                                    libId: "File1",
                                                    deleted: false,
                                                    extension: "fb2",
                                                    date: "2025-11-07",
                                                    folder: "archive1.zip",
                                                    libRate: 0,
                                                    languageId: langId);
                var keywordId = await KeywordUtils.Create(c, name: "Keyword 1");

                return (bookId, keywordId);
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await BookKeywords.InsertBookKeywordsAsync(c, [new BookKeywordParam(BookId: bookId, KeywordId: keywordId)]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookKeywordUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
