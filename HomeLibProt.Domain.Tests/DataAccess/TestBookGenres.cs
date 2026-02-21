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
            var (bookId, genreId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
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

                var genreId = await GenreUtils.Create(c, key: "genre1", name: "Genre 1");

                return (bookId, genreId);
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await BookGenres.InsertBookGenresAsync(c, [new BookGenreParam(BookId: bookId, GenreId: genreId)]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookGenreUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
