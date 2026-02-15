using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestSeries {
    [Test]
    public async Task TestGetSeriesByNameAsync() {
        var expected = new[] { new SeriesEntity(Id: 2, Name: "Series 2") };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await SeriesUtils.Create(c, name: "Series 1");
                await SeriesUtils.Create(c, name: "Series 2");
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return await Series.GetSeriesByNameAsync(c, ["Series 2", "Series 3"]);
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestInsertSeriesAsync() {
        var expected = new[] {
            new TestSeriesEntity(Id: 1, Name: "Series 1"),
            new TestSeriesEntity(Id: 2, Name: "Series 2"),
            new TestSeriesEntity(Id: 3, Name: "Series 3"),
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await SeriesUtils.Create(c, name: "Series 1");
                await SeriesUtils.Create(c, name: "Series 2");
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Series.InsertSeriesAsync(c, ["Series 3"]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, SeriesUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected));
    }
}
