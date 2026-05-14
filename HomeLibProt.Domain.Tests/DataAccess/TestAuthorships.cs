using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestAuthorships {
    [Test]
    public async Task TestInsertAuthorshipsAsync() {
        var expected = new[] {
                new TestAuthorship(BookId: 1, AuthorId: 1)
            };

        var actual = await TestUtils.UseTestDatabase(
            async (connection) => await DbStructure.CreateImportInpxStructure(connection, true),
            async (connection) => {
                var (bookId, authorId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
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
                    var authorId = await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");

                    return (bookId, authorId);
                });

                await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    await Authorships.InsertAuthorshipsAsync(c, [new AuthorshipParam(BookId: bookId, AuthorId: authorId)]);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, AuthorshipUtils.GetTestData);
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    [TestCase(1, 2, AuthorshipsCheckResult.Duplicate)]
    [TestCase(1, 1, AuthorshipsCheckResult.AllExsists)]
    [TestCase(1, 0, AuthorshipsCheckResult.OnlyBook)]
    [TestCase(0, 1, AuthorshipsCheckResult.OnlyAuthor)]
    [TestCase(0, 0, AuthorshipsCheckResult.NoRecords)]
    public async Task TestCheckIfAuthorshipsExistsAsync(long bookId, long authorId, AuthorshipsCheckResult expected) {
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
                    await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");
                    var authorId = await AuthorUtils.Create(connection, fullName: "B B B", lastName: "B", firstName: "B", middleName: "B");

                    await AuthorshipUtils.Create(connection, bookId: bookId, authorId: authorId);
                });

                return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                    return await Authorships.CheckIfAuthorshipsExistsAsync(c, new AuthorshipParam(BookId: bookId, AuthorId: authorId));
                });
            });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
