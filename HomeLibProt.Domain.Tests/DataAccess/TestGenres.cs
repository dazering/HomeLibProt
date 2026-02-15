using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestGenres {
    [Test]
    public async Task TestGetGenresByKeyAsync() {
        var expected = new[] { new Genre(Id: 2, Key: "genre2") };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await GenreUtils.Create(connection, key: "genre1", name: "Genre 1");
                await GenreUtils.Create(connection, key: "genre2", name: "Genre 2");
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return await Genres.GetGenresByKeyAsync(c, ["genre2", "genre3"]);
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestInsertGenresAsync() {
        var expected = new[] {
            new TestGenre(Id: 1, Key: "genre1", Name: "Genre 1"),
            new TestGenre(Id: 2, Key: "genre2", Name: "Genre 2"),
            new TestGenre(Id: 3, Key: "genre3", Name: null),
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await GenreUtils.Create(connection, key: "genre1", name: "Genre 1");
                await GenreUtils.Create(connection, key: "genre2", name: "Genre 2");
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Genres.InsertGenresAsync(c, ["genre3"]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, GenreUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected));
    }
}
