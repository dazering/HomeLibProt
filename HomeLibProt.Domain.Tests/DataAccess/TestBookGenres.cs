using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestBookGenres {
    [Test]
    public async Task TestInsertBookGenresAsync() {
        var expected = new[] {
                new TestBookGenre(BookId: 1, GenreId: 1)
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, BookGenreUtils.SetUpTestData);

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await BookGenres.InsertBookGenresAsync(c, [new BookGenreParam(BookId: 1, GenreId: 1)]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookGenreUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
