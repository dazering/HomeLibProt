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
                new TestBook(Id: 1, Title: "Title 1", FileName: "File1", Size: 1, LibId: "File1", Deleted: 0, Extension: "fb2", Date: "2025-11-07", ArchiveId: 1, LibRate: null, LanguageId: 1),
                new TestBook(Id: 2, Title: "Title 2", FileName: "File2", Size: 2, LibId: "File2", Deleted: 0, Extension: "fb2", Date: "2025-11-07", ArchiveId: 1, LibRate: 1, LanguageId: 1),
                new TestBook(Id: 3, Title: "Title 3", FileName: "File3", Size: 3, LibId: "File3", Deleted: 1, Extension: "fb2", Date: "2025-11-07", ArchiveId: 1, LibRate: null, LanguageId: 1)
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            var (langId, archiveId) = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var langId = await LanguageUtils.Create(c, "Lang 1");
                var archiveId = await ArchiveUtils.Create(c, "archive1.zip");

                return (langId, archiveId);
            });

            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                await Books.InsertBookAsync(c, new BookParam(Title: "Title 1", FileName: "File1", Size: 1, LibId: "File1", Deleted: false, Extension: "fb2", Date: "2025-11-07", ArchiveId: archiveId, LibRate: null, LanguageId: langId));
                await Books.InsertBookAsync(c, new BookParam(Title: "Title 2", FileName: "File2", Size: 2, LibId: "File2", Deleted: false, Extension: "fb2", Date: "2025-11-07", ArchiveId: archiveId, LibRate: 1, LanguageId: langId));
                await Books.InsertBookAsync(c, new BookParam(Title: "Title 3", FileName: "File3", Size: 3, LibId: "File3", Deleted: true, Extension: "fb2", Date: "2025-11-07", ArchiveId: archiveId, LibRate: null, LanguageId: langId));
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, BookUtils.GetTestData);
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public async Task TestGetArchiveEntitiesByArchiveIdAsync() {
        var expected = new[] {
                new ArchiveEntity(FileName: "File1", Extension: "fb2")
            };

        var actual = await TestUtils.UseTestDatabase(async (connection) => {
            var archiveId = await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                var archiveId1 = await ArchiveUtils.Create(c, "archive1.zip");
                var archiveId2 = await ArchiveUtils.Create(c, "archive2.zip");

                var langId = await LanguageUtils.Create(c, "Lang 1");

                await BookUtils.Create(c,
                                       title: "Title1",
                                       fileName: "File1",
                                       size: 1,
                                       libId: "File1",
                                       deleted: false,
                                       extension: "fb2",
                                       date: "2025-11-07",
                                       archiveId: archiveId1,
                                       libRate: 0,
                                       languageId: langId);
                await BookUtils.Create(c,
                                       title: "Title2",
                                       fileName: "File2",
                                       size: 1,
                                       libId: "File2",
                                       deleted: false,
                                       extension: "fb2",
                                       date: "2025-11-07",
                                       archiveId: archiveId2,
                                       libRate: 0,
                                       languageId: langId);

                return archiveId1;
            });

            return await ConnectionUtils.DoInTransactionAsync(connection, async (c) => {
                return Books.GetArchiveEntitiesByArchiveIdAsync(c, archiveId).ToBlockingEnumerable().ToArray();
            });
        });

        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}
