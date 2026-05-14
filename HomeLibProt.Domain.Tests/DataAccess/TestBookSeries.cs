using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestBookSeries {
    [Test]
    public async Task TestInsertBookSeriesAsync() {
        var expected = new[] {
                new TestBookSeriesEntity(BookId: 1, SeriesId: 1, SeriesNumber: 1)
            };

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                var (bookId, seriesId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
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

                    var seriesId = await SeriesUtils.Create(c, name: "Series 1");

                    return (bookId, seriesId);
                });

                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await BookSeries.InsertBookSeriesAsync(c, new BookSeriesParam(BookId: bookId, SeriesId: seriesId, SeriesNumber: 1));
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, BookSeriesUtils.GetTestData);
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    [TestCase(1, 2, BookSeriesCheckResult.Duplicate)]
    [TestCase(1, 1, BookSeriesCheckResult.AllExsists)]
    [TestCase(1, 0, BookSeriesCheckResult.OnlyBook)]
    [TestCase(0, 1, BookSeriesCheckResult.OnlySeries)]
    [TestCase(0, 0, BookSeriesCheckResult.NoRecords)]
    public async Task TestCheckIfCheckIfBookSeriesExistsAsync(long bookId, long seriesId, BookSeriesCheckResult expected) {
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
                    await SeriesUtils.Create(c, name: "Series 1");
                    var seriesId = await SeriesUtils.Create(c, name: "Series 2");
                    await BookSeriesUtils.Create(c, bookId: bookId, seriesId: seriesId, seriesNumber: 1);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await BookSeries.CheckIfBookSeriesExistsAsync(c, new BookSeriesParam(BookId: bookId, SeriesId: seriesId, 0));
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
