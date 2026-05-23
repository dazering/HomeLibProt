using System;
using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestKeywords {
    [Test]
    public async Task TestGetKeywordsByNameAsync() {
        var expected = new[] { new Keyword(Id: 2, Name: "Keyword 2") };

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await KeywordUtils.Create(c, name: "Keyword 1");
                    await KeywordUtils.Create(c, name: "Keyword 2");
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await Keywords.GetKeywordsByNameAsync(c, ["Keyword 2", "Keyword 3"]);
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestInsertKeywordsAsync() {
        var expected = new[] {
            new TestKeyword(Id: 1, Name: "Keyword 1"),
            new TestKeyword(Id: 2, Name: "Keyword 2"),
            new TestKeyword(Id: 3, Name: "Keyword 3"),
        };

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await KeywordUtils.Create(c, name: "Keyword 1");
                    await KeywordUtils.Create(c, name: "Keyword 2");
                });

                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await Keywords.InsertKeywordsAsync(c, ["Keyword 3"]);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, KeywordUtils.GetTestData);
            });

        Assert.That(actual, Is.EqualTo(expected));
    }

    public static TestCaseData[] GetKeywordsByBookIdAsyncTestCases = {
        new(1, new string[] { "Keyword 1" }),
        new(-1, Array.Empty<string>()),
    };

    [Test]
    [TestCaseSource(nameof(GetKeywordsByBookIdAsyncTestCases))]
    public async Task TestGetKeywordsByBookIdAsync(long bookId, string[] expected) {
        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    var archiveId = await ArchiveUtils.Create(c, "archive1.zip");

                    var langId = await LanguageUtils.Create(c, "Lang 1");

                    var bookId = await BookUtils.Create(c,
                                                        title: "Title1",
                                                        fileName: "File1",
                                                        size: 1,
                                                        libId: "File1",
                                                        deleted: false,
                                                        extension: "fb2",
                                                        date: "2025-11-07",
                                                        archiveId: archiveId,
                                                        libRate: 0,
                                                        languageId: langId);

                    var keywordId = await KeywordUtils.Create(c, name: "Keyword 1");

                    await BookKeywordUtils.Create(c, bookId: bookId, keywordId: keywordId);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await Keywords.GetKeywordsByBookIdAsync(c, bookId: bookId);
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
