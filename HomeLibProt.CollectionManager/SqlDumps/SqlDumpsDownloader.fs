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
                FileDownloader.downloadFileAsync
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
                FileDownloader.downloadFileAsync
                    parameters.PathToSqlDumps
                    parameters.HttpClient
                    parameters.ProgressReport
                    uri
                    fileName
                    parameters.Retries
    }
