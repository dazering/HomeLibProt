module HomeLibProt.CollectionManager.Books.BooksMerger

open System
open System.IO
open System.IO.Compression
open System.Threading.Tasks

type BooksMergerParameters =
    { PathToLibrary: string
      OutputPath: string
      ArchiveSize: int
      ArchiveFilter: string
      Prefix: string
      KeepArchives: bool
      ProgressReport: string -> unit }

type ArchiveInfo =
    { Entries: string array
      Path: string
      LastOne: bool }

type NewArchive =
    { FirstBook: int64
      LastBook: int64
      ArchiveInfos: ArchiveInfo array }

type ArchiveEntry =
    { ArchivePath: string
      EntryName: string }

type FirstAndLastEntries = { First: int64; Last: int64 }

let private copyEntryAsync (targetEntry: ZipArchiveEntry) (outputEntry: ZipArchiveEntry) : Task<unit> =
    task {
        use outputEntryFs = outputEntry.Open()

        use entryToCopyFs = targetEntry.Open()

        do! entryToCopyFs.CopyToAsync outputEntryFs
    }

let private copyEntriesAsync
    (targetArchive: ZipArchive)
    (outputArchive: ZipArchive)
    (entriesToCopy: string array)
    : Task<unit> =
    task {
        for entry in entriesToCopy do
            let newEntry = outputArchive.CreateEntry(entry, CompressionLevel.SmallestSize)
            let entryToCopy = targetArchive.GetEntry entry

            do! copyEntryAsync entryToCopy newEntry
    }

let private copyArchiveAsync (outputArchive: ZipArchive) (archiveInfo: ArchiveInfo) : Task<unit> =
    task {
        use fs = File.OpenRead archiveInfo.Path
        use archiveToCopy = new ZipArchive(fs, ZipArchiveMode.Read)

        do! copyEntriesAsync archiveToCopy outputArchive archiveInfo.Entries
    }

let private getNewArchiveName (prefix: string) (newArchive: NewArchive) : string =
    $"{prefix}{newArchive.FirstBook:d7}-{newArchive.LastBook:d7}.zip"

let private copyNewArchiveAsync
    (path: string)
    (prefix: string)
    (deleteArchives: bool)
    (progressReport: string -> unit)
    (newArchive: NewArchive)
    : Task =
    task {
        let archiveName = getNewArchiveName prefix newArchive
        use fs = File.Create(Path.Combine(path, archiveName))
        use archive = new ZipArchive(fs, ZipArchiveMode.Create)

        for ai in newArchive.ArchiveInfos do
            $"Copy books from {ai.Path |> Path.GetFileName} to {archiveName}"
            |> progressReport

            do! copyArchiveAsync archive ai

            if ai.LastOne && deleteArchives then
                do File.Delete ai.Path
    }

let private foldEntryFirstAndLast (state: FirstAndLastEntries) (entry: string) : FirstAndLastEntries =
    let bookId = entry |> Path.GetFileNameWithoutExtension

    match Int64.TryParse bookId with
    | true, v ->
        if state.First = 0 then
            { state with First = v; Last = v }
        else
            { state with Last = v }
    | false, _ -> state

let private getFirstAndLastBook (archivePath: string) : FirstAndLastEntries =
    use archive = ZipFile.OpenRead archivePath

    archive.Entries
    |> Seq.map (fun e -> e.Name)
    |> Seq.fold foldEntryFirstAndLast { First = 0; Last = 0 }

let private mapArchiveEntriesToNewArchive (archiveEntries: ArchiveEntry array) : NewArchive =
    let firstAndLastEntry =
        archiveEntries
        |> Array.map (fun e -> e.EntryName)
        |> Array.fold foldEntryFirstAndLast { First = 0; Last = 0 }

    let archiveInfos =
        archiveEntries
        |> Array.groupBy (fun c -> c.ArchivePath)
        |> Array.map (fun (archive, entries) ->
            let firstAndLastBook = archive |> getFirstAndLastBook

            { Path = archive
              Entries = entries |> Array.map (fun e -> e.EntryName)
              LastOne =
                entries
                |> Array.map (fun e -> e.EntryName |> Path.GetFileNameWithoutExtension)
                |> Array.contains (firstAndLastBook.Last.ToString()) })

    { FirstBook = firstAndLastEntry.First
      LastBook = firstAndLastEntry.Last
      ArchiveInfos = archiveInfos }

let private filterByBookName (entry: ZipArchiveEntry) : bool =
    match Int64.TryParse(entry.Name |> Path.GetFileNameWithoutExtension) with
    | true, _ -> true
    | false, _ -> false

let private getArchiveEntries (archives: string array) : ArchiveEntry seq =
    seq {
        for archive in archives do
            use a = ZipFile.OpenRead archive

            yield!
                a.Entries
                |> Seq.filter filterByBookName
                |> Seq.sortBy (fun e -> e.Name |> Path.GetFileNameWithoutExtension |> int64)
                |> Seq.map (fun e ->
                    { ArchivePath = archive
                      EntryName = e.Name })
    }

let private getArchivesToCopy (pathToLibrary: string) (archiveFilter: string) : string array =
    Directory.EnumerateFiles(pathToLibrary, archiveFilter)
    |> Seq.sortBy (fun s -> getFirstAndLastBook s |> fun x -> x.First)
    |> Seq.toArray

let megreBooksAsync (parameters: BooksMergerParameters) : Task<unit> =
    task {

        $"Megring archives from {parameters.PathToLibrary} to {parameters.OutputPath} with filter: {parameters.ArchiveFilter}"
        |> parameters.ProgressReport

        let archives = getArchivesToCopy parameters.PathToLibrary parameters.ArchiveFilter

        let newArchives =
            getArchiveEntries archives
            |> Seq.chunkBySize parameters.ArchiveSize
            |> Seq.map mapArchiveEntriesToNewArchive


        for newArchive in newArchives do
            do!
                copyNewArchiveAsync
                    parameters.OutputPath
                    parameters.Prefix
                    (not parameters.KeepArchives)
                    parameters.ProgressReport
                    newArchive

    }
