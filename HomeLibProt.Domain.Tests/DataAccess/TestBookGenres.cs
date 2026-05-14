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

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                var (bookId, genreId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
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

    [Test]
    [TestCase(1, 2, BookGenreCheckResult.Duplicate)]
    [TestCase(1, 1, BookGenreCheckResult.AllExsists)]
    [TestCase(1, 0, BookGenreCheckResult.OnlyBook)]
    [TestCase(0, 1, BookGenreCheckResult.OnlyGenre)]
    [TestCase(0, 0, BookGenreCheckResult.NoRecords)]
    public async Task TestCheckIfBookGenreExistsAsync(long bookId, long genreId, BookGenreCheckResult expected) {
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
                    await GenreUtils.Create(c, key: "genre1", name: "Genre 1");
                    var genreId = await GenreUtils.Create(c, key: "genre2", name: "Genre 2");

                    await BookGenreUtils.Create(c, bookId: bookId, genreId: genreId);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await BookGenres.CheckIfBookGenreExistsAsync(c, new BookGenreParam(BookId: bookId, GenreId: genreId));
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
