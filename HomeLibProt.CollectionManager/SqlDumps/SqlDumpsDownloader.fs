module HomeLibProt.CollectionManager.SqlDumps.SqlDumpsDownloader

open System.Net.Http
open System
open System.Threading.Tasks

open HomeLibProt.CollectionManager.Common

type SqlDumpDownloaderParameters =
    { PathToSqlDumps: string
      HttpClient: HttpClient
      Retries: uint
      ProgressReport: string -> unit }

let downloadSqlDumpsFlibustaAsync (parameters: SqlDumpDownloaderParameters) : Task<unit> =
    task {
        let urisToDownload =
            [| Flibusta.authors
               Flibusta.authorships
               Flibusta.bookGenres
               Flibusta.books
               Flibusta.bookSeries
               Flibusta.genres
               Flibusta.rates
               Flibusta.series |]

        for fileName in urisToDownload do
            parameters.ProgressReport $"Downloading: {fileName}"

            let uri = Uri $"{Flibusta.url}/{fileName}"

            do!
                HttpDownloader.downloadFileAsync
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
            [| Librusec.authors
               Librusec.authorships
               Librusec.bookGenres
               Librusec.books
               Librusec.bookSeries
               Librusec.genres
               Librusec.rates
               Librusec.series |]

        for fileName in urisToDownload do
            parameters.ProgressReport $"Downloading: {fileName}"

            let uri = Uri $"{Librusec.url}/{fileName}"

            do!
                HttpDownloader.downloadFileAsync
                    parameters.PathToSqlDumps
                    parameters.HttpClient
                    parameters.ProgressReport
                    uri
                    fileName
                    parameters.Retries
    }
