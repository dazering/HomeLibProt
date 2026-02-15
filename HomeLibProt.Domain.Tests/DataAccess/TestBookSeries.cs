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

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            var (bookId, seriesId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var bookId = await BookUtils.Create(c,
                                                    title: "Title1",
                                                    fileName: "File1",
                                                    size: 1,
                                                    libId: "File1",
                                                    deleted: false,
                                                    extension: "fb2",
                                                    date: "2025-11-07",
                                                    folder: "archive1.zip",
                                                    libRate: 0);

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
}
