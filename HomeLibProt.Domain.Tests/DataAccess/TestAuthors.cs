using System.Linq;
using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestAuthors {
    [Test]
    public async Task TestGetAuthorsByNameAsync() {
        var expected = new[] { new Author(Id: 2, Name: "B B B") };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");
                await AuthorUtils.Create(connection, fullName: "B B B", lastName: "B", firstName: "B", middleName: "B");
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return await Authors.GetAuthorsByNameAsync(c, ["B B B", "C C C"]);
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestInsertAuthorsAsync() {
        var expected = new[] {
            new TestAuthor(Id: 1, FullName: "A A A", LastName: "A", FirstName: "A", MiddleName: "A"),
            new TestAuthor(Id: 2, FullName: "B B B", LastName: "B", FirstName: "B", MiddleName: "B"),
            new TestAuthor(Id: 3, FullName: "C C C", LastName: "C", FirstName: "C", MiddleName: "C")
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");
                await AuthorUtils.Create(connection, fullName: "B B B", lastName: "B", firstName: "B", middleName: "B");
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Authors.InsertAuthorsAsync(c, [new AuthorParam(FullName: "C C C", LastName: "C", FirstName: "C", MiddleName: "C")]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, AuthorUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestGetAuthorsFilterByIncludedLanguageAsync() {
        var expected = new[] {
            new Author(Id: 1, Name: "A A A"),
            new Author(Id: 2, Name: "B B B")
         };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var archiveId = await ArchiveUtils.Create(c, "archive1.zip");

                var langId1 = await LanguageUtils.Create(c, "Lang1", true);
                var langId2 = await LanguageUtils.Create(c, "Lang2", false);

                var bookId1 = await BookUtils.Create(c,
                                                     title: "Title1",
                                                     fileName: "File1",
                                                     size: 1,
                                                     libId: "File1",
                                                     deleted: false,
                                                     extension: "fb2",
                                                     date: "2025-11-07",
                                                     archiveId: archiveId,
                                                     libRate: 0,
                                                     languageId: langId1);
                var bookId2 = await BookUtils.Create(c,
                                                     title: "Title2",
                                                     fileName: "File2",
                                                     size: 1,
                                                     libId: "File2",
                                                     deleted: false,
                                                     extension: "fb2",
                                                     date: "2025-11-07",
                                                     archiveId: archiveId,
                                                     libRate: 0,
                                                     languageId: langId2);
                var bookId3 = await BookUtils.Create(c,
                                                     title: "Title3",
                                                     fileName: "File3",
                                                     size: 1,
                                                     libId: "File3",
                                                     deleted: false,
                                                     extension: "fb2",
                                                     date: "2025-11-07",
                                                     archiveId: archiveId,
                                                     libRate: 0,
                                                     languageId: langId2);

                var authorId1 = await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");
                var authorId2 = await AuthorUtils.Create(connection, fullName: "B B B", lastName: "B", firstName: "B", middleName: "B");
                var authorId3 = await AuthorUtils.Create(connection, fullName: "C C C", lastName: "C", firstName: "C", middleName: "C");

                await AuthorshipUtils.Create(c, bookId: bookId1, authorId: authorId1);
                await AuthorshipUtils.Create(c, bookId: bookId1, authorId: authorId2);
                await AuthorshipUtils.Create(c, bookId: bookId2, authorId: authorId2);
                await AuthorshipUtils.Create(c, bookId: bookId2, authorId: authorId3);
            });
            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return Authors.GetAuthorsFilterByIncludedLanguageAsync(c).ToBlockingEnumerable().ToArray();
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
