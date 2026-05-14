module HomeLibProt.CollectionManager.SqlDumps.SqlDumpImporter

open System
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

let private rateResultToEntityParam (rate: RateResult) : RateEntityParam =
    RateEntityParam(BookId = rate.BookId, Rate = rate.Rate)

let private importRateResult (connection: DbConnection) (result: RateResult) : Task<unit> =
    task {
        let entityParam = result |> rateResultToEntityParam

        match! Rates.CheckIfBookExistsAsync(connection, entityParam) with
        | RatesCheckResult.BookExsists -> do! Rates.InsertRateEntityAsync(connection, entityParam)
        | RatesCheckResult.NoRecord ->
            eprintfn $"Book Id: {entityParam.BookId}, Rate: {entityParam.Rate}. Book not found."
        | r -> raise (InvalidOperationException $"Unsupported check result: {r}")
    }

let private bookSeriesResultToEntityParam (bookSeries: BookSeriesResult) : BookSeriesParam =
    BookSeriesParam(BookId = bookSeries.BookId, SeriesId = bookSeries.SeriesId, SeriesNumber = bookSeries.SeriesNumber)

let private importBookSeriesResult (connection: DbConnection) (result: BookSeriesResult) : Task<unit> =
    task {
        let entityParam = result |> bookSeriesResultToEntityParam

        match! BookSeries.CheckIfBookSeriesExistsAsync(connection, entityParam) with
        | BookSeriesCheckResult.Duplicate ->
            eprintfn
                $"Unable to insert duplicate of book series with Book Id: {entityParam.BookId}, Series Id: {entityParam.SeriesId}."
        | BookSeriesCheckResult.AllExsists -> do! BookSeries.InsertBookSeriesAsync(connection, entityParam)
        | BookSeriesCheckResult.OnlyBook ->
            eprintfn
                $"Book Id: {entityParam.BookId}, Series Id: {entityParam.SeriesId}, Series Number: {entityParam.SeriesNumber}. Series not found."
        | BookSeriesCheckResult.OnlySeries ->
            eprintfn
                $"Book Id: {entityParam.BookId}, Series Id: {entityParam.SeriesId}, Series Number: {entityParam.SeriesNumber}. Book not found."
        | BookSeriesCheckResult.NoRecords ->
            eprintfn
                $"Book Id: {entityParam.BookId}, Series Id: {entityParam.SeriesId}, Series Number: {entityParam.SeriesNumber}. Book and Series not found."
        | r -> raise (InvalidOperationException $"Unsupported check result: {r}")
    }

let private seriesResultToEntityParam (series: SeriesResult) : SeriesEntityParam =
    SeriesEntityParam(Id = series.Id, Name = series.Name)

let private importSeriesResult (connection: DbConnection) (result: SeriesResult) : Task<unit> =
    task {
        let entityParam = result |> seriesResultToEntityParam

        do! Series.InsertSeriesEntityAsync(connection, entityParam)
    }

let private bookGenreResultToEntityParam (bookGenre: BookGenreResult) : BookGenreParam =
    BookGenreParam(BookId = bookGenre.BookId, GenreId = bookGenre.GenreId)

let private importBookGenreResult (connection: DbConnection) (result: BookGenreResult) : Task<unit> =
    task {
        let entityParam = result |> bookGenreResultToEntityParam

        match! BookGenres.CheckIfBookGenreExistsAsync(connection, entityParam) with
        | BookGenreCheckResult.Duplicate ->
            eprintfn
                $"Unable to insert duplicate of book genre with Book Id: {entityParam.BookId}, Genre Id: {entityParam.GenreId}."
        | BookGenreCheckResult.AllExsists -> do! BookGenres.InsertBookGenresAsync(connection, [| entityParam |])
        | BookGenreCheckResult.OnlyBook ->
            eprintfn $"Book Id: {entityParam.BookId}, Genre Id: {entityParam.GenreId}. Genre not found."
        | BookGenreCheckResult.OnlyGenre ->
            eprintfn $"Book Id: {entityParam.BookId}, Genre Id: {entityParam.GenreId}. Book not found."
        | BookGenreCheckResult.NoRecords ->
            eprintfn $"Book Id: {entityParam.BookId}, Genre Id: {entityParam.GenreId}. Book and Genre not found."
        | r -> raise (InvalidOperationException $"Unsupported check result: {r}")
    }

let private genreResultToEntityParam (genre: GenreResult) : GenreEntityParam =
    GenreEntityParam(Id = genre.Id, Key = genre.Key, Name = genre.Name)

let private importGenreResult (connection: DbConnection) (result: GenreResult) : Task<unit> =
    task {
        let entityParam = result |> genreResultToEntityParam

        do! Genres.InsertGenreEntityAsync(connection, entityParam)
    }

let private authorshipsResultToParam (authorships: AuthorshipsResult) : AuthorshipParam =
    AuthorshipParam(BookId = authorships.BookId, AuthorId = authorships.AuthorId)

let private importAuthorshipsResult (connection: DbConnection) (result: AuthorshipsResult) : Task<unit> =
    task {
        let entityParam = result |> authorshipsResultToParam

        match! Authorships.CheckIfAuthorshipsExistsAsync(connection, entityParam) with
        | AuthorshipsCheckResult.Duplicate ->
            eprintfn
                $"Unable to insert duplicate of authorship with Author Id: {entityParam.AuthorId}, Book Id: {entityParam.BookId}."
        | AuthorshipsCheckResult.AllExsists -> do! Authorships.InsertAuthorshipsAsync(connection, [| entityParam |])
        | AuthorshipsCheckResult.OnlyBook ->
            eprintfn $"Author Id: {entityParam.AuthorId}, Book Id: {entityParam.BookId}. Author not found."
        | AuthorshipsCheckResult.OnlyAuthor ->
            eprintfn $"Author Id: {entityParam.AuthorId}, Book Id: {entityParam.BookId}. Book not found."
        | AuthorshipsCheckResult.NoRecords ->
            eprintfn $"Author Id: {entityParam.AuthorId}, Book Id: {entityParam.BookId}. Author and Book not found."
        | r -> raise (InvalidOperationException $"Unsupported check result: {r}")
    }

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
        LibRate = Nullable(),
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
        let authorships = Path.Combine(parameters.PathToSqlDumps, Flibusta.authorships)
        let books = Path.Combine(parameters.PathToSqlDumps, Flibusta.books)
        let genres = Path.Combine(parameters.PathToSqlDumps, Flibusta.genres)
        let bookGenres = Path.Combine(parameters.PathToSqlDumps, Flibusta.bookGenres)
        let series = Path.Combine(parameters.PathToSqlDumps, Flibusta.series)
        let bookSeries = Path.Combine(parameters.PathToSqlDumps, Flibusta.bookSeries)
        let rates = Path.Combine(parameters.PathToSqlDumps, Flibusta.rates)

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

        parameters.ProgressReport $"Importing: {Flibusta.authorships}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                authorships
                                HomeLibProt.CollectionManager.RegEx.Flibusta.authorships
                                importAuthorshipsResult
                                getAuthorshipsResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Flibusta.genres}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                genres
                                HomeLibProt.CollectionManager.RegEx.Flibusta.genres
                                importGenreResult
                                getGenreResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Flibusta.bookGenres}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                bookGenres
                                HomeLibProt.CollectionManager.RegEx.Flibusta.bookGenres
                                importBookGenreResult
                                getBookGenreResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Flibusta.series}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                series
                                HomeLibProt.CollectionManager.RegEx.Flibusta.series
                                importSeriesResult
                                getSeriesResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Flibusta.bookSeries}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                bookSeries
                                HomeLibProt.CollectionManager.RegEx.Flibusta.bookSeries
                                importBookSeriesResult
                                getBookSeriesResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Flibusta.rates}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                rates
                                HomeLibProt.CollectionManager.RegEx.Flibusta.rates
                                importRateResult
                                getRateResult
                                c
                    }
            )
    }

let importSqlDumpsLibrusecAsync (parameters: SqlDumpImporterParameters) (connection: DbConnection) : Task<unit> =
    task {
        do! parameters.DoInTransactionAsync(connection, DbStructure.CreateImportSqlDumpStructure)

        let authors = Path.Combine(parameters.PathToSqlDumps, Librusec.authors)
        let authorships = Path.Combine(parameters.PathToSqlDumps, Librusec.authorships)
        let books = Path.Combine(parameters.PathToSqlDumps, Librusec.books)
        let genres = Path.Combine(parameters.PathToSqlDumps, Librusec.genres)
        let bookGenres = Path.Combine(parameters.PathToSqlDumps, Librusec.bookGenres)
        let series = Path.Combine(parameters.PathToSqlDumps, Librusec.series)
        let bookSeries = Path.Combine(parameters.PathToSqlDumps, Librusec.bookSeries)
        let rates = Path.Combine(parameters.PathToSqlDumps, Librusec.rates)

        parameters.ProgressReport $"Importing: {Librusec.authors}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                authors
                                HomeLibProt.CollectionManager.RegEx.Librusec.authors
                                importAuthorResult
                                getAuthorResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.books}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        let! archiveId = getArchiveId c

                        do!
                            importFromGZip
                                books
                                HomeLibProt.CollectionManager.RegEx.Librusec.books
                                (importBookResult archiveId)
                                getBookResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.authorships}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                authorships
                                HomeLibProt.CollectionManager.RegEx.Librusec.authorships
                                importAuthorshipsResult
                                getAuthorshipsResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.genres}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                genres
                                HomeLibProt.CollectionManager.RegEx.Librusec.genres
                                importGenreResult
                                getGenreResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.bookGenres}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                bookGenres
                                HomeLibProt.CollectionManager.RegEx.Librusec.bookGenres
                                importBookGenreResult
                                getBookGenreResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.series}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                series
                                HomeLibProt.CollectionManager.RegEx.Librusec.series
                                importSeriesResult
                                getSeriesResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.bookSeries}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                bookSeries
                                HomeLibProt.CollectionManager.RegEx.Librusec.bookSeries
                                importBookSeriesResult
                                getBookSeriesResult
                                c
                    }
            )

        parameters.ProgressReport $"Importing: {Librusec.rates}"

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun (c: DbConnection) ->
                    task {
                        do!
                            importFromGZip
                                rates
                                HomeLibProt.CollectionManager.RegEx.Librusec.rates
                                importRateResult
                                getRateResult
                                c
                    }
            )
    }
