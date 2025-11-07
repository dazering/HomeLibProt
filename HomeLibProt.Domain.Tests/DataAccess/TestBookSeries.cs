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
            await ConnectionUtils.DoInTransactionAsync(connection, BookSeriesUtils.SetUpTestData);

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await BookSeries.InsertBookSeriesAsync(c, new BookSeriesParam(BookId: 1, SeriesId: 1, SeriesNumber: 1));
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookSeriesUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
