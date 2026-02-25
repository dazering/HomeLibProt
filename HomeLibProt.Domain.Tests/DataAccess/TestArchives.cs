using System.Linq;
using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestArchives {
    [Test]
    public async Task TestGetArchivesByNameAsync() {
        var expected = new[] { new Archive(Id: 2, Name: "Archive 2") };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await ArchiveUtils.Create(c, name: "Archive 1");
                await ArchiveUtils.Create(c, name: "Archive 2");
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return await Archives.GetArchivesByNameAsync(c, ["Archive 2", "Archive 3"]);
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestInsertArchivesAsync() {
        var expected = new[] {
            new TestArchive(Id: 1, Name: "Archive 1"),
            new TestArchive(Id: 2, Name: "Archive 2"),
            new TestArchive(Id: 3, Name: "Archive 3"),
        };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await ArchiveUtils.Create(c, name: "Archive 1");
                await ArchiveUtils.Create(c, name: "Archive 2");
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Archives.InsertArchivesAsync(c, ["Archive 3"]);
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, ArchiveUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestGetArchivesAsync() {
        var expected = new[] {
                new Archive(Id: 1, Name: "archive1.zip"),
                new Archive(Id: 2, Name: "archive2.zip")
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await ArchiveUtils.Create(c, "archive1.zip");
                await ArchiveUtils.Create(c, "archive2.zip");
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return Archives.GetArchivesAsync(c).ToBlockingEnumerable().ToArray();
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
