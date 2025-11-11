module HomeLibProt.CollectionImporter.Tests.Utils.AuthorHierarchicalSearchImporterUtils

open System.Data.Common

open HomeLibProt.Domain.Tests.Utils

let setUpData (connection: DbConnection) =
    task {
        let sql =
            @"
insert into
Authors (Id, FullName, FirstName, MiddleName, LastName)
values
(1, 'B B B', 'B', 'B', 'B'),
(2, 'Baa Baa Baa', 'Baa', 'Baa', 'Baa'),
(3, 'Bab Bab Bab', 'Bab', 'Bab', 'Bab')
"

        do! TestUtils.InsertIntoTestDatabase(connection, sql)
    }
