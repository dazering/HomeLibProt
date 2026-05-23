using System;
using System.Linq;
using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestGenres {
    [Test]
    public async Task TestGetGenresByKeyAsync() {
        var expected = new[] { new Genre(Id: 2, Key: "genre2") };

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
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

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
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

    [Test]
    public async Task TestInsertGenreEntityAsync() {
        var expected = new[] {
            new TestGenre(Id: 1, Key: "genre1", Name: "Genre 1"),
            new TestGenre(Id: 2, Key: "genre2", Name: "Genre 2"),
            new TestGenre(Id: 30, Key: "genre30", Name: "Genre 30"),
        };

        var actual = await TestUtils.UseTestDatabase(
            DbStructure.CreateImportSqlDumpStructure,
            async (connection) => {
                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await GenreUtils.Create(connection, key: "genre1", name: "Genre 1");
                    await GenreUtils.Create(connection, key: "genre2", name: "Genre 2");
                });

                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await Genres.InsertGenreEntityAsync(c, new GenreEntityParam(Id: 30, Key: "genre30", Name: "Genre 30"));
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, GenreUtils.GetTestData);
            });

        Assert.That(actual, Is.EqualTo(expected));
    }

    public static TestCaseData[] GetGenreKeysByBookIdAsyncTestCases = {
        new(1, new string[] { "genre1" }),
        new(-1, Array.Empty<string>()),
    };

    [Test]
    [TestCaseSource(nameof(GetGenreKeysByBookIdAsyncTestCases))]
    public async Task TestGetGenreKeysByBookIdAsync(long bookId, string[] expected) {
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

                    var genreId = await GenreUtils.Create(c, key: "genre1", name: "Genre 1");

                    await BookGenreUtils.Create(c, bookId: bookId, genreId: genreId);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await Genres.GetGenreKeysByBookIdAsync(c, bookId: bookId);
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestGetGenresInpxEntitiesAsync() {
        var expected = new[] {
            new GenreInpxEntity(Key: "genre1" , Name: "Genre 1"),
            new GenreInpxEntity(Key: "genre2" , Name: "Genre 2"),
         };

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await GenreUtils.Create(connection, key: "genre1", name: "Genre 1");
                    await GenreUtils.Create(connection, key: "genre2", name: "Genre 2");
                });
                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return Genres.GetGenresInpxEntitiesAsync(c).ToBlockingEnumerable().ToArray();
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
