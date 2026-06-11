module HomeLibProt.CollectionManager.Inpx.InpxGenerator

open System
open System.Data.Common
open System.IO
open System.IO.Compression
open System.Threading.Tasks

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

let private authorInpxInfoToAuthorsName (author: AuthorInpxInfo) : InpLine.AuthorName =
    { First = author.FirstName
      Middle = author.MiddleName
      Last = author.LastName }

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

let private getRateAsync (bookId: int64) (connection: DbConnection) : Task<string> =
    task {
        match! Rates.GetAvgRateByBookIdAsync(connection, bookId) with
        | null -> return String.Empty
        | r -> return r.AvgRate.ToString()
    }

let private getInpLinesAsync
    (bookId: int64)
    (extension: string)
    (size: string)
    (reportError: string -> unit)
    (connection: DbConnection)
    : Task<string array> =
    task {
        let! bookInfo = Books.GetBookInpxInfosByIdAsync(connection, bookId)

        match bookInfo with
        | [||] ->
            reportError $"Not found any information in database for Book Id: {bookId}"
            return [||]
        | bf ->
            let! authors = Authors.GetAuthorInpxInfosByBookIdAsync(connection, bookId)
            let! genres = Genres.GetGenreKeysByBookIdAsync(connection, bookId)
            let! keywords = Keywords.GetKeywordsByBookIdAsync(connection, bookId)
            let! rate = getRateAsync bookId connection

            return
                bf
                |> Array.map (fun bi ->
                    bookAttributesToInpRecord (bookId.ToString()) extension size rate bi authors genres keywords
                    |> InpLine.makeLine)
    }

let private tryToGetInpLinesAsync
    (reportError: string -> unit)
    (entry: ZipArchiveEntry)
    (connection: DbConnection)
    : Task<string array option> =
    task {
        match Int64.TryParse(entry.Name |> Path.GetFileNameWithoutExtension) with
        | true, bookId ->
            let extension = (entry.Name |> Path.GetExtension).Replace(".", String.Empty)
            let size = entry.Length.ToString()

            let! lines = getInpLinesAsync bookId extension size reportError connection

            return Some lines
        | false, _ -> return None
    }

let private createInp (inpx: ZipArchive) (name: string) : ZipArchiveEntry = inpx.CreateEntry name

let private createInpFromLibraryArchive
    (archiveName: string)
    (inpx: ZipArchive)
    (reportProgress: string -> unit)
    (reportError: string -> unit)
    (connection: DbConnection)
    (libraryArchive: ZipArchive)
    : Task =
    task {
        let inp = Path.ChangeExtension(archiveName, "inp") |> createInp inpx

        use fs = inp.Open()
        use sw = new StreamWriter(fs)

        for entry in libraryArchive.Entries do

            let! linesResult = tryToGetInpLinesAsync reportError entry connection

            match linesResult with
            | Some lines ->
                for line in lines do
                    do! sw.WriteLineAsync line
            | None -> ()
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
{name.ToLower}_{libType.ToLower}_{date}
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

        for archive in zipArchives do
            let archiveName = Path.GetFileName archive

            reportProgress $"Generating inp for {archiveName}"

            try
                do!
                    ArchiveUtils.DoWithArchiveAsync(
                        archive,
                        createInpFromLibraryArchive archiveName inpx reportProgress reportError connection
                    )
            with :? InvalidDataException as _ ->
                reportError $"Invalid archive {archiveName}"
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
