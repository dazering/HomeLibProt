module HomeLibProt.CollectionManager.SqlDumps.SqlDumpImporter

open System.Data.Common
open System.IO
open System.IO.Compression
open System.Text.RegularExpressions
open System.Threading.Tasks

open HomeLibProt.Domain.DataAccess
open HomeLibProt.CollectionManager.RegEx.RegExResults
open HomeLibProt.CollectionManager.SqlDumps.SqlDumpParser

type SqlDumpImporterParameters =
    { PathToSqlDumps: string
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

let private authorResultToEntityParam (author: AuthorResult) : AuthorEntityParam =
    AuthorEntityParam(
        Id = author.Id,
        FirstName = author.FirstName,
        MiddleName = author.MiddleName,
        LastName = author.LastName,
        FullName = author.FullName
    )

let private importAuthorResult (connection: DbConnection) (result: AuthorResult) : Task<unit> =
    task {
        let entityParam = result |> authorResultToEntityParam

        do! Authors.InsertAuthorEntityAsync(connection, entityParam)
    }

let private importFromGZip
    (archive: string)
    (regExPattern: string)
    (importResult: DbConnection -> 'T -> Task<unit>)
    (getResult: (GroupCollection -> 'T))
    (connection: DbConnection)
    : Task<unit> =
    task {
        use fs = new FileStream(archive, FileMode.Open, FileAccess.Read)
        use gzip = new GZipStream(fs, CompressionMode.Decompress)
        use sr = new StreamReader(gzip)

        let results = sr |> parseSqlDump getResult regExPattern

        for result in results do
            do! importResult connection result
    }

let importSqlDumpsFlibustaAsync (parameters: SqlDumpImporterParameters) (connection: DbConnection) : Task<unit> =
    task {
        do! parameters.DoInTransactionAsync(connection, DbStructure.CreateImportSqlDumpStructure)

        let authors = Path.Combine(parameters.PathToSqlDumps, Flibusta.authors)

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                authors
                                HomeLibProt.CollectionManager.RegEx.Flibusta.authors
                                importAuthorResult
                                getAuthorResult
                                c
                    }
            )
    }
