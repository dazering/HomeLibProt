using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestLanguages {
    [Test]
    public async Task TestGetLanguagesByNameAsync() {
        var expected = new[] { new Language(Id: 2, Name: "Lang 2") };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await LanguageUtils.Create(c, name: "Lang 1");
                await LanguageUtils.Create(c, name: "Lang 2");
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return await Languages.GetLanguagesByNameAsync(c, ["Lang 2", "Lang 3"]);
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestInsertSeriesAsync() {
        var expected = new[] {
            new TestLanguage(Id: 1, Name: "Lang 1", Include: 1),
            new TestLanguage(Id: 2, Name: "Lang 2", Include: 1),
            new TestLanguage(Id: 3, Name: "Lang 3", Include: 1),
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await LanguageUtils.Create(c, name: "Lang 1");
                await LanguageUtils.Create(c, name: "Lang 2");
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Languages.InsertLanguagesAsync(c, ["Lang 3"]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, LanguageUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected));
    }
}
