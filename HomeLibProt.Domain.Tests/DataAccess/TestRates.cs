using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestRates {
    [Test]
    public async Task TestInsertRateEntityAsync() {
        var expected = new[] {
            new TestRate(Id: 1, BookId: 1, Rate: 1),
            new TestRate(Id: 2, BookId: 1, Rate: 2),
            new TestRate(Id: 3, BookId: 1, Rate: 3),
        };

        var actual = await TestUtils.UseTestDatabase(
            DbStructure.CreateImportSqlDumpStructure,
            async (connection) => {
                var bookId = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    var langId = await LanguageUtils.Create(c, "Lang 1");
                    var archiveId = await ArchiveUtils.Create(c, "archive1.zip");
                    return await BookUtils.Create(c,
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
                });

                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await Rates.InsertRateEntityAsync(c, new RateEntityParam(BookId: bookId, Rate: 1));
                    await Rates.InsertRateEntityAsync(c, new RateEntityParam(BookId: bookId, Rate: 2));
                    await Rates.InsertRateEntityAsync(c, new RateEntityParam(BookId: bookId, Rate: 3));
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, RateUtils.GetTestData);
            });

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestGetAvgRateByBookIdAsync() {
        var expected = new Rate(BookId: 1, AvgRate: 2);

        var actual = await TestUtils.UseTestDatabase(
            DbStructure.CreateImportSqlDumpStructure,
            async (connection) => {
                var bookId = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    var langId = await LanguageUtils.Create(c, "Lang 1");
                    var archiveId = await ArchiveUtils.Create(c, "archive1.zip");
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
                    await RateUtils.Create(c, bookId: bookId, rate: 1);
                    await RateUtils.Create(c, bookId: bookId, rate: 1);
                    await RateUtils.Create(c, bookId: bookId, rate: 3);

                    return bookId;
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await Rates.GetAvgRateByBookIdAsync(c, bookId: bookId);
                });
            });

        Assert.That(actual, Is.EqualTo(expected));
    }
}
