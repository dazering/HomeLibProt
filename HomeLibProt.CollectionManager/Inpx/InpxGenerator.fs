module HomeLibProt.CollectionManager.Inpx.InpxGenerator

open System
open System.Data.Common
open System.IO
open System.IO.Compression
open System.Threading.Tasks

open HomeLibProt.Common.Fb2Reader
open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.Utils

type Site =
    | Flibusta
    | Librusec

type LibraryType =
    | Fb2
    | All

type InpxParameters =
    { Site: Site; LibraryType: LibraryType }

type InpxGeneratorParameters =
    { PathToLibrary: string
      PathToInpx: string
      InpxParameters: InpxParameters
      ProgressReport: string -> unit
      ErrorReport: string -> unit
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

type private InpStat =
    { Processed: uint
      Skipped: uint
      Added: uint }

type private InpxStat =
    { Processed: uint
      Skipped: uint
      Added: uint }

let private authorInpxInfoToAuthorsName (author: AuthorInpxInfo) : InpLine.AuthorName =
    { First = author.FirstName
      Middle = author.MiddleName
      Last = author.LastName }

let private authorFb2InfoToAuthorsName (author: HomeLibProt.Common.Fb2Reader.AuthorName) : InpLine.AuthorName =
    { First = author.First
      Middle = author.Middle
      Last = author.Last }

let private bookAttributesToInpRecord
    (bookId: string)
    (extension: string)
    (size: string)
    (rate: string)
    (bookInpxInfo: BookInpxInfo)
    (authors: AuthorInpxInfo array)
    (genres: string array)
    (keywords: string array)
    : InpLine.InpRecord =
    { AuthorNames = authors |> Array.map authorInpxInfoToAuthorsName
      Genres = genres
      Title = bookInpxInfo.Title
      Series = bookInpxInfo.Series
      SeriesNumber = bookInpxInfo.SeriesNumber.ToString()
      FileName = bookId
      Size = size
      LibId = bookId
      Del = bookInpxInfo.Deleted.ToString()
      Extension = extension
      Date = bookInpxInfo.Date
      Lang = bookInpxInfo.Language
      LibRate = rate
      Keywords = keywords }

let private fb2InfoToInpRecord
    (bookId: string)
    (extension: string)
    (size: string)
    (fb2Info: Fb2Info)
    (series: HomeLibProt.Common.Fb2Reader.Series)
    : InpLine.InpRecord =
    { AuthorNames = fb2Info.Authors |> Array.map authorFb2InfoToAuthorsName
      Genres = fb2Info.Genres
      Title = fb2Info.Title
      Series = series.Name
      SeriesNumber = series.Number
      FileName = bookId
      Size = size
      LibId = bookId
      Del = "0"
      Extension = extension
      Date = "1999-01-01"
      Lang = fb2Info.Language
      LibRate = String.Empty
      Keywords = fb2Info.Keywords }

let private getRateAsync (bookId: int64) (connection: DbConnection) : Task<string> =
    task {
        match! Rates.GetAvgRateByBookIdAsync(connection, bookId) with
        | null -> return String.Empty
        | r -> return r.AvgRate.ToString()
    }

let private tryToGetInpLinesFromFb2
    (bookId: int64)
    (extension: string)
    (size: string)
    (bookEntry: ZipArchiveEntry)
    (inpStat: InpStat)
    (reportError: string -> unit)
    : string array * InpStat =
    use bs = bookEntry.Open()
    let fb2Info = getFb2Info bs

    match fb2Info with
    | Some fb2I ->
        fb2I.Series
        |> Array.map (fun series ->
            fb2InfoToInpRecord (bookId.ToString()) extension size fb2I series
            |> InpLine.makeLine),
        { inpStat with
            Processed = inpStat.Processed + 1u
            Added = inpStat.Added + 1u }
    | None ->
        reportError $"Not found any information for Book Id: {bookId}"

        [||],
        { inpStat with
            Processed = inpStat.Processed + 1u
            Skipped = inpStat.Skipped + 1u }

let private getInpLinesAsync
    (bookId: int64)
    (extension: string)
    (size: string)
    (bookEntry: ZipArchiveEntry)
    (inpStat: InpStat)
    (reportError: string -> unit)
    (connection: DbConnection)
    : Task<string array * InpStat> =
    task {
        let! bookInfo = Books.GetBookInpxInfosByIdAsync(connection, bookId)

        match bookInfo with
        | [||] -> return tryToGetInpLinesFromFb2 bookId extension size bookEntry inpStat reportError
        | bf ->
            let! authors = Authors.GetAuthorInpxInfosByBookIdAsync(connection, bookId)
            let! genres = Genres.GetGenreKeysByBookIdAsync(connection, bookId)
            let! keywords = Keywords.GetKeywordsByBookIdAsync(connection, bookId)
            let! rate = getRateAsync bookId connection

            return
                bf
                |> Array.map (fun bi ->
                    bookAttributesToInpRecord (bookId.ToString()) extension size rate bi authors genres keywords
                    |> InpLine.makeLine),
                { inpStat with
                    Processed = inpStat.Processed + 1u
                    Added = inpStat.Added + 1u }
    }

let private tryToGetInpLinesAsync
    (reportError: string -> unit)
    (entry: ZipArchiveEntry)
    (inpStat: InpStat)
    (connection: DbConnection)
    : Task<string array option * InpStat> =
    task {
        match Int64.TryParse(entry.Name |> Path.GetFileNameWithoutExtension) with
        | true, bookId ->
            let extension = (entry.Name |> Path.GetExtension).Replace(".", String.Empty)
            let size = entry.Length.ToString()

            let! lines, stat = getInpLinesAsync bookId extension size entry inpStat reportError connection

            return Some lines, stat
        | false, _ ->

            $"Invalid book id: {entry.Name}" |> reportError

            return
                None,
                { inpStat with
                    Processed = inpStat.Processed + 1u
                    Skipped = inpStat.Skipped + 1u }
    }

let private createInp (inpx: ZipArchive) (name: string) : ZipArchiveEntry = inpx.CreateEntry name

let private createInpFromLibraryArchive
    (archiveName: string)
    (inpx: ZipArchive)
    (reportProgress: string -> unit)
    (reportError: string -> unit)
    (connection: DbConnection)
    (libraryArchive: ZipArchive)
    : Task<InpStat> =
    task {
        let inp = Path.ChangeExtension(archiveName, "inp") |> createInp inpx

        use fs = inp.Open()
        use sw = new StreamWriter(fs)

        let mutable inpStat: InpStat =
            { Processed = 0u
              Skipped = 0u
              Added = 0u }

        for entry in libraryArchive.Entries do

            let! linesResult, stat = tryToGetInpLinesAsync reportError entry inpStat connection

            inpStat <- stat

            match linesResult with
            | Some lines ->
                for line in lines do
                    do! sw.WriteLineAsync line
            | None -> ()

        return inpStat
    }

let private genreInpxEntityToString (genre: GenreInpxEntity) : string =
    $"{genre.Key}{InpLine.InpSeparator}{genre.Name}"

let private importGenreList (connection: DbConnection) (inpx: ZipArchive) : Task =
    task {
        let genresList = inpx.CreateEntry "genres.list"

        use fs = genresList.Open()
        use sw = new StreamWriter(fs)

        let genres = Genres.GetGenresInpxEntitiesAsync connection

        let genreEnumerator = genres.GetAsyncEnumerator()

        while! genreEnumerator.MoveNextAsync() do
            do! sw.WriteLineAsync(genreEnumerator.Current |> genreInpxEntityToString)
    }

let private getOffflineCode (libraryType: LibraryType) : string =
    match libraryType with
    | Fb2 -> (65536).ToString Globalization.CultureInfo.InvariantCulture
    | All -> (65537).ToString Globalization.CultureInfo.InvariantCulture

let private getDateString () : string = DateTime.Now.ToString "yyyy-MM-dd"

let private getLibraryType (libraryType: LibraryType) : string =
    match libraryType with
    | Fb2 -> "FB2"
    | All -> "ALL"

let private getLibraryName (site: Site) : string =
    match site with
    | Flibusta -> "Flibusta"
    | Librusec -> "Librusec"

let private getCollectionInfoContent (inpxParameters: InpxParameters) : string =
    let name = getLibraryName inpxParameters.Site
    let libType = getLibraryType inpxParameters.LibraryType
    let date = getDateString ()

    $"{name} {libType} - {date}
{name.ToLower()}_{libType.ToLower()}_{date}
{getOffflineCode inpxParameters.LibraryType}
Collection of {name} {libType} books
"

let private addCollectionInfo (inpx: ZipArchive) (inpxParameters: InpxParameters) : Task =
    task {
        let collectionInfo = inpx.CreateEntry "collection.info"

        use fs = collectionInfo.Open()
        use sw = new StreamWriter(fs)

        do! sw.WriteAsync(getCollectionInfoContent inpxParameters)
    }

let private fillInpxAsync
    (pathToLibrary: string)
    (inpxParameters: InpxParameters)
    (reportProgress: string -> unit)
    (reportError: string -> unit)
    (connection: DbConnection)
    (inpx: ZipArchive)
    =
    task {
        reportProgress $"Make collection.info"

        do! addCollectionInfo inpx inpxParameters

        reportProgress $"Importing Genres List"

        do! importGenreList connection inpx

        let zipArchives = Directory.EnumerateFiles(pathToLibrary, "*.zip")

        let mutable inpxStat: InpxStat =
            { Processed = 0u
              Skipped = 0u
              Added = 0u }

        for archive in zipArchives do
            let archiveName = Path.GetFileName archive

            try
                let! inpStat =
                    ArchiveUtils.DoWithArchiveAsync(
                        archive,
                        createInpFromLibraryArchive archiveName inpx reportProgress reportError connection
                    )

                inpxStat <-
                    { inpxStat with
                        Processed = inpxStat.Processed + inpStat.Processed
                        Added = inpxStat.Added + inpStat.Added
                        Skipped = inpxStat.Skipped + inpStat.Skipped }

                reportProgress
                    $"Generated inp for {archiveName}. Processed: {inpStat.Processed}, Added: {inpStat.Added}, Skipped: {inpStat.Skipped}"
            with :? InvalidDataException as _ ->
                reportError $"Invalid archive {archiveName}"

        reportProgress
            $"Total books Processed: {inpxStat.Processed}, Added: {inpxStat.Added}, Skipped: {inpxStat.Skipped}"
    }

let private createInpxAndFillAsync (path: string) (fillInpx: ZipArchive -> Task<unit>) : Task =
    task {
        use fs = File.Create path
        use archive = new ZipArchive(fs, ZipArchiveMode.Create)
        do! fillInpx archive
    }

let generateInpxAsync (parameters: InpxGeneratorParameters) (connection: DbConnection) : Task =
    task {
        $"Generating inpx: {parameters.PathToInpx}" |> parameters.ProgressReport

        do!
            parameters.DoInTransactionAsync(
                connection,
                fun c ->
                    task {
                        do!
                            createInpxAndFillAsync
                                parameters.PathToInpx
                                (fillInpxAsync
                                    parameters.PathToLibrary
                                    parameters.InpxParameters
                                    parameters.ProgressReport
                                    parameters.ErrorReport
                                    c)
                    }
            )
    }
