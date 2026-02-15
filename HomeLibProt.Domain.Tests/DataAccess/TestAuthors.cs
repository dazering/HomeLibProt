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
    public async Task TestGetAuthorsAsync() {
        var expected = new[] {
            new Author(Id: 1, Name: "A A A"),
            new Author(Id: 2, Name: "B B B")
         };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await AuthorUtils.Create(connection, fullName: "A A A", lastName: "A", firstName: "A", middleName: "A");
                await AuthorUtils.Create(connection, fullName: "B B B", lastName: "B", firstName: "B", middleName: "B");
            });
            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return Authors.GetAuthorsAsync(c).ToBlockingEnumerable().ToArray();
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
