module HomeLibProt.CollectionImporter.Tests.Utils.CollectionValidatorUtils

open System.Data.Common

open HomeLibProt.Domain.Tests.Utils

let setUpData (connection: DbConnection) =
    task {
        let sql =
            @"
insert into
Books
    (Id,
    Title,
    FileName,
    Size,
    LibId,
    Deleted,
    Extension,
    Date,
    Folder,
    LibRate)
values
    (1,
    'Title1',
    '1',
    1,
    '1',
    0,
    'fb2',
    '2025-11-7',
    '000001-000001.zip',
    0),
    (2,
    'Title2',
    '2',
    1,
    '2',
    0,
    'fb2',
    '2025-11-7',
    '000001-000001.zip',
    0),
    (3,
    'Title3',
    '3',
    1,
    '3',
    0,
    'fb2',
    '2025-11-7',
    '000002-000003.zip',
    0);
"

        do! TestUtils.InsertIntoTestDatabase(connection, sql)
    }
