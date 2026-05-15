module HomeLibProt.CollectionManager.SqlDumps.SqlDumpsDownloader

open System.IO
open System.Net.Http
open System
open System.Threading.Tasks

type SqlDumpDownloaderParameters =
    { PathToSqlDumps: string
      HttpClient: HttpClient
      Retries: uint
      ProgressReport: string -> unit }

let private downloadSqlDumpAsync
    (pathToSqlDumps: string)
    (httpClient: HttpClient)
    (uri: Uri)
    (fileName: string)
    : Task<option<unit>> =
    task {
        try
            let! responseStream = httpClient.GetStreamAsync uri

            use fileStrean = File.Create(Path.Combine(pathToSqlDumps, fileName))

            do! responseStream.CopyToAsync fileStrean

            return Some()
        with :? HttpRequestException as exe ->
            return None

    }

let rec downloadSqkDumpWithRetryAsync
    (pathToSqlDumps: string)
    (httpClient: HttpClient)
    (reportProgress: string -> unit)
    (uri: Uri)
    (fileName: string)
    (retries: uint)
    : Task<unit> =
    task {
        if retries <= 0u then
            raise (InvalidOperationException $"Count of retries is exhausted for {fileName}")

        match! downloadSqlDumpAsync pathToSqlDumps httpClient uri fileName with
        | Some() -> return ()
        | None ->
            reportProgress $"Attempt download {fileName} failed. Remaing retries {retries - 1u}"
            do! downloadSqkDumpWithRetryAsync pathToSqlDumps httpClient reportProgress uri fileName (retries - 1u)

    }

let downloadSqlDumpsFlibustaAsync (parameters: SqlDumpDownloaderParameters) : Task<unit> =
    task {
        let urisToDownload =
            [| Uri $"{Flibusta.url}/{Flibusta.authors}", Flibusta.authors
               Uri $"{Flibusta.url}/{Flibusta.authorships}", Flibusta.authorships
               Uri $"{Flibusta.url}/{Flibusta.bookGenres}", Flibusta.bookGenres
               Uri $"{Flibusta.url}/{Flibusta.books}", Flibusta.books
               Uri $"{Flibusta.url}/{Flibusta.bookSeries}", Flibusta.bookSeries
               Uri $"{Flibusta.url}/{Flibusta.genres}", Flibusta.genres
               Uri $"{Flibusta.url}/{Flibusta.rates}", Flibusta.rates
               Uri $"{Flibusta.url}/{Flibusta.series}", Flibusta.series |]

        for uri, fileName in urisToDownload do
            parameters.ProgressReport $"Downloading: {fileName}"

            do!
                downloadSqkDumpWithRetryAsync
                    parameters.PathToSqlDumps
                    parameters.HttpClient
                    parameters.ProgressReport
                    uri
                    fileName
                    parameters.Retries

    }

let downloadSqlDumpsLibrusecAsync (parameters: SqlDumpDownloaderParameters) =
    task {
        let urisToDownload =
            [| Uri $"{Librusec.url}/{Librusec.authors}", Librusec.authors
               Uri $"{Librusec.url}/{Librusec.authorships}", Librusec.authorships
               Uri $"{Librusec.url}/{Librusec.bookGenres}", Librusec.bookGenres
               Uri $"{Librusec.url}/{Librusec.books}", Librusec.books
               Uri $"{Librusec.url}/{Librusec.bookSeries}", Librusec.bookSeries
               Uri $"{Librusec.url}/{Librusec.genres}", Librusec.genres
               Uri $"{Librusec.url}/{Librusec.rates}", Librusec.rates
               Uri $"{Librusec.url}/{Librusec.series}", Librusec.series |]

        for uri, fileName in urisToDownload do
            parameters.ProgressReport $"Downloading: {fileName}"

            do!
                downloadSqkDumpWithRetryAsync
                    parameters.PathToSqlDumps
                    parameters.HttpClient
                    parameters.ProgressReport
                    uri
                    fileName
                    parameters.Retries
    }
