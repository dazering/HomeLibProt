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
      ProgressReport: string -> unit
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

let private archiveName = "SQL_Dump"

let private bookResultToEntityParam (archiveId: int64) (languageId: int64) (book: BookResult) : BookEntityParam =
    BookEntityParam(
        Id = book.Id,
        Title = book.Title,
        FileName = $"{book.Id}",
        Size = book.FileSize,
        LibId = $"{book.Id}",
        Deleted = book.Deleted,
        Extension = book.Extension,
        Date = book.Date,
        ArchiveId = archiveId,
        LibRate = 0,
        LanguageId = languageId
    )

let importBookKeywords (bookId: int64) (keywords: string array) (connection: DbConnection) : Task<unit> =
    task {
        let! exsisted = Keywords.GetKeywordsByNameAsync(connection, keywords)

        let bookKeywordParams =
            exsisted
            |> Array.map (fun k -> BookKeywordParam(BookId = bookId, KeywordId = k.Id))

        do! BookKeywords.InsertBookKeywordsAsync(connection, bookKeywordParams)
    }

let importKeywords (keywords: string array) (connection: DbConnection) : Task<unit> =
    task {
        let! exsisted = Keywords.GetKeywordsByNameAsync(connection, keywords)

        let map = exsisted |> Array.map (fun k -> k.Name, k.Id) |> Map

        let keywordsToAdd =
            keywords |> Array.filter (fun k -> map |> Map.containsKey k |> not)

        do! Keywords.InsertKeywordsAsync(connection, keywordsToAdd)
    }

let private getLanguageId (lang: string) (connection: DbConnection) : Task<int64> =
    task {
        match! Languages.GetLanguagesByNameAsync(connection, [| lang |]) with
        | [||] ->
            do! Languages.InsertLanguagesAsync(connection, [| lang |])
            let! ee = Languages.GetLanguagesByNameAsync(connection, [| lang |])

            match ee with
            | [| language |] -> return language.Id
            | _ -> return! failwith $"Language '{lang}' not found"
        | [| language |] -> return language.Id
        | _ -> return! failwith $"There are more then one '{lang}'"

    }

let private importBookResult (archiveId: int64) (connection: DbConnection) (result: BookResult) : Task<unit> =
    task {
        let! languageId = getLanguageId result.Lang connection

        let entityParam = result |> bookResultToEntityParam archiveId languageId

        do! Books.InsertBookEntityAsync(connection, entityParam)

        do! importKeywords result.Keywords connection
        do! importBookKeywords result.Id result.Keywords connection
    }

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

let getArchiveId (connection: DbConnection) : Task<int64> =
    task {
        do! Archives.InsertArchivesAsync(connection, [| archiveName |])

        match! Archives.GetArchivesByNameAsync(connection, [| archiveName |]) with
        | [| archive |] -> return archive.Id
        | [||] -> return! failwith $"There is no archive with name: {archiveName}"
        | _ -> return! failwith $"There are more then one archive with name: {archiveName}"
    }

let importSqlDumpsFlibustaAsync (parameters: SqlDumpImporterParameters) (connection: DbConnection) : Task<unit> =
    task {
        do! parameters.DoInTransactionAsync(connection, DbStructure.CreateImportSqlDumpStructure)

        let authors = Path.Combine(parameters.PathToSqlDumps, Flibusta.authors)
        let books = Path.Combine(parameters.PathToSqlDumps, Flibusta.books)

        parameters.ProgressReport $"Importing: {Flibusta.authors}"

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

        parameters.ProgressReport $"Importing: {Flibusta.books}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        let! archiveId = getArchiveId c

                        do!
                            importFromGZip
                                books
                                HomeLibProt.CollectionManager.RegEx.Flibusta.books
                                (importBookResult archiveId)
                                getBookResult
                                c
                    }
            )
    }
