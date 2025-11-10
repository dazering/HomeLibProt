module HomeLibProt.CollectionImporter.InpxImporter

open System
open System.Data.Common
open System.IO
open System.IO.Compression
open System.Text.RegularExpressions
open System.Threading.Tasks

open HomeLibProt.CollectionImporter.InpLine

type InpxImporterParameters =
    { PathToInpx: string
      BatchSize: int
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

let private parseLine (regEx: Regex) (line: string) : InpLine =
    line |> parseInpLine regEx |> getInpLine

let private readEntry (stream: StreamReader) : string seq =
    seq {
        while not stream.EndOfStream do
            let line = stream.ReadLineAsync().ConfigureAwait(false).GetAwaiter().GetResult()

            if line |> String.IsNullOrWhiteSpace |> not then
                yield line
    }

let private useEntry (action: StreamReader -> seq<string>) (entry: ZipArchiveEntry) : string seq =
    seq {
        use stream = entry.Open()
        use streamReader = new StreamReader(stream)
        yield! action streamReader
    }

let private getInpEntires (archive: ZipArchive) : ZipArchiveEntry seq =
    archive.Entries |> Seq.filter (fun e -> Path.GetExtension e.Name = ".inp")

let private tryGetStructureInfoEntry (archive: ZipArchive) : ZipArchiveEntry option =
    archive.Entries |> Seq.tryFind (fun e -> e.Name = "structure.info")

let private readStructureInfoContent (structureInfo: ZipArchiveEntry) : string =
    use stream = structureInfo.Open()
    use streamReader = new StreamReader(stream)
    streamReader.ReadToEndAsync().ConfigureAwait(false).GetAwaiter().GetResult()

let private getStructureInfoContentOrDefault (structureInfo: ZipArchiveEntry option) : string =
    match structureInfo with
    | Some si -> si |> readStructureInfoContent
    | None -> defaultStructure

let private readInpx (path: string) : ZipArchive = ZipFile.OpenRead path

let importInpxToDb (parameters: InpxImporterParameters) (connection: DbConnection) : Task<unit> =
    task {
        let inpx = parameters.PathToInpx |> readInpx

        let regEx =
            inpx |> tryGetStructureInfoEntry |> getStructureInfoContentOrDefault |> getRegEx

        let bookChunks =
            inpx
            |> getInpEntires
            |> Seq.collect (fun e ->
                let folderName = (e.Name, "zip") |> Path.ChangeExtension

                (readEntry, e)
                ||> useEntry
                |> Seq.map (parseLine regEx >> Book.convertInpLineToBook folderName))
            |> Seq.chunkBySize parameters.BatchSize


        for books in bookChunks do
            do!
                (connection, books |> BookImporter.processBooks)
                |> parameters.DoInTransactionAsync
    }
