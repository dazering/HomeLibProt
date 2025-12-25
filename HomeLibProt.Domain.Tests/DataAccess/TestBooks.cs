using System.Linq;
using System.Threading.Tasks;
using HomeLibProt.Domain.DataAccess;
using HomeLibProt.Domain.Tests.Entities;
using HomeLibProt.Domain.Tests.Utils;

namespace HomeLibProt.Domain.Tests.DataAccess;

public class TestBooks {
    [Test]
    public async Task TestInsertBookAsync() {
        var expected = new[] {
                new TestBook(Id: 1, Title: "Title 1", FileName: "File1", Size: 1, LibId: "File1", Deleted: 0, Extension: "fb2", Date: "2025-11-07", Folder: "archive1.zip", LibRate: null),
                new TestBook(Id: 2, Title: "Title 2", FileName: "File2", Size: 2, LibId: "File2", Deleted: 0, Extension: "fb2", Date: "2025-11-07", Folder: "archive1.zip", LibRate: 1),
                new TestBook(Id: 3, Title: "Title 3", FileName: "File3", Size: 3, LibId: "File3", Deleted: 1, Extension: "fb2", Date: "2025-11-07", Folder: "archive1.zip", LibRate: null)
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Books.InsertBookAsync(c, new BookParam(Title: "Title 1", FileName: "File1", Size: 1, LibId: "File1", Deleted: false, Extension: "fb2", Date: "2025-11-07", Folder: "archive1.zip", LibRate: null));
                await Books.InsertBookAsync(c, new BookParam(Title: "Title 2", FileName: "File2", Size: 2, LibId: "File2", Deleted: false, Extension: "fb2", Date: "2025-11-07", Folder: "archive1.zip", LibRate: 1));
                await Books.InsertBookAsync(c, new BookParam(Title: "Title 3", FileName: "File3", Size: 3, LibId: "File3", Deleted: true, Extension: "fb2", Date: "2025-11-07", Folder: "archive1.zip", LibRate: null));
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestGetFoldersAsync() {
        var expected = new[] {
                "archive1.zip",
                "archive2.zip"
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, BookUtils.SetUpTestData);

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return Books.GetFoldersAsync(c).ToBlockingEnumerable().ToArray();
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestGetFolderEntitiesByFolderAsync() {
        var expected = new[] {
                new FolderEntity(FileName: "File1", Extension: "fb2")
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, BookUtils.SetUpTestData);

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return Books.GetFolderEntitiesByFolderAsync(c, "archive1.zip").ToBlockingEnumerable().ToArray();
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
