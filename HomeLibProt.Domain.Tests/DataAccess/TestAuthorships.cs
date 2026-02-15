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

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            var (bookId, authorId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var bookId = await BookUtils.Create(c,
                                                    title: "Title1",
                                                    fileName: "File1",
                                                    size: 1,
                                                    libId: "File1",
                                                    deleted: false,
                                                    extension: "fb2",
                                                    date: "2025-11-07",
                                                    folder: "archive1.zip",
                                                    libRate: 0);
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
}
