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
            await ConnectionUtils.DoInTransactionAsync(connection, AuthorshipUtils.SetUpTestData);

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Authorships.InsertAuthorshipsAsync(c, [new AuthorshipParam(BookId: 1, AuthorId: 1)]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, AuthorshipUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
