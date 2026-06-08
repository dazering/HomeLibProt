module HomeLibProt.CollectionManager.Books.BooksDownloader

open System
open System.Net.Http
open System.IO
open System.Text.RegularExpressions
open System.Threading.Tasks

open HomeLibProt.CollectionManager.Common

type ArchiveTypeDownload =
    | All
    | Fb2
    | Binary

type BooksDownloaderParameters =
    { PathToLibrary: string
      OutputPath: string
      HttpClient: HttpClient
      Retries: uint
      ArchiveTypeDownload: ArchiveTypeDownload
      ProgressReport: string -> unit }

type private FirstAndLastBook = { First: int64; Last: int64 }

type private ArchiveType =
    | Fb2
    | Binary

type private Reference =
    { Reference: string
      Type: ArchiveType
      First: int64 }

module private RegExGroups =
    let reference = "reference"
    let typeGroup = "type"
    let first = "first"

module private Flibusta =
    let url = "http://https.flibusta.is/daily/"

let private extractGroupValue (group: Group) : string = group.Value

module private FlibustaRegEx =
    open RegExGroups

    let referenceRegExPattern =
        $"href=\"(?<{reference}>f.(?<{typeGroup}>fb2|n).(?<{first}>\\d+)-(?<last>\\d+).zip)\""

    let private stringToArchiveType (s: string) : ArchiveType =
        match s with
        | "fb2" -> Fb2
        | "n" -> Binary
        | _ -> failwith $"Unsupported archive type: {s}"

    let getReference (groups: GroupCollection) : Reference =
        { Reference = groups.[reference] |> extractGroupValue
          Type = groups.[typeGroup] |> extractGroupValue |> stringToArchiveType
          First = groups.[first] |> extractGroupValue |> int64 }

let private getReferenceFilter (lastBook: int64) (archiveTypeDownload: ArchiveTypeDownload) : (Reference -> bool) =
    match archiveTypeDownload with
    | ArchiveTypeDownload.All -> fun r -> r.First > lastBook
    | ArchiveTypeDownload.Fb2 -> fun r -> r.First > lastBook && r.Type = ArchiveType.Fb2
    | ArchiveTypeDownload.Binary -> fun r -> r.First > lastBook && r.Type = ArchiveType.Binary

let private getReferences
    (httpClient: HttpClient)
    (progressReport: string -> unit)
    (retries: uint)
    (regEx: Regex)
    (getReference: GroupCollection -> Reference)
    (uri: Uri)
    : Task<Reference array> =
    task {
        let! body = HttpDownloader.downloadStringAsync httpClient progressReport uri retries

        let matches = regEx.Matches body

        return matches |> Seq.map (fun m -> m.Groups |> getReference) |> Seq.toArray
    }

let private firstLastRegExPattern = "-(?<first>\\d+)-(?<last>\\d+)"

let private getFirstAndLastBookGroups (regEx: Regex) (fileName: string) =
    let m = regEx.Match fileName
    m.Groups

let private getFirsAndLastBook (group: GroupCollection) : FirstAndLastBook =
    { First = group.["first"] |> extractGroupValue |> int64
      Last = group.["last"] |> extractGroupValue |> int64 }

let private getLastBookFromArchives (path: string) : int64 =
    let firstAndLastRegEx = new Regex(firstLastRegExPattern, RegexOptions.Compiled)

    Directory.EnumerateFiles(path, "*.zip")
    |> Seq.map Path.GetFileName
    |> Seq.map (getFirstAndLastBookGroups firstAndLastRegEx >> getFirsAndLastBook)
    |> Seq.map (fun fl -> fl.Last)
    |> Seq.max

let downloadFlibustaArchives (parameters: BooksDownloaderParameters) : Task<unit> =
    task {
        let lastLibraryBook = getLastBookFromArchives parameters.PathToLibrary

        parameters.ProgressReport $"Found last book: {lastLibraryBook}"

        let referenceRegEx =
            new Regex(FlibustaRegEx.referenceRegExPattern, RegexOptions.Compiled)

        let! references =
            getReferences
                parameters.HttpClient
                parameters.ProgressReport
                parameters.Retries
                referenceRegEx
                FlibustaRegEx.getReference
                (Uri Flibusta.url)

        let referenceFilter =
            getReferenceFilter lastLibraryBook parameters.ArchiveTypeDownload

        let referencesToDownload =
            references |> Array.filter referenceFilter |> Array.map (fun r -> r.Reference)

        for r in referencesToDownload do
            parameters.ProgressReport $"Downloading: {r}"

            let uri = Uri $"{Flibusta.url}/{r}"

            do!
                HttpDownloader.downloadFileAsync
                    parameters.OutputPath
                    parameters.HttpClient
                    parameters.ProgressReport
                    uri
                    r
                    parameters.Retries
    }
