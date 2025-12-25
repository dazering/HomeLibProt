module HomeLibProt.CollectionImporter.CollectionValidator

open System
open System.Collections.Generic
open System.Data.Common
open System.IO
open System.IO.Compression
open System.Threading.Tasks

open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.Utils

type CollectionValidatorParameters =
    { PathToArchives: string
      ProgressReport: string -> unit
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

let private makeAbsentFilesMessage (absentFiles: List<string>) = String.Join(", ", absentFiles)

let private appendAbsentFileIfNotExsists
    (entryName: string)
    (absentFiles: List<string>)
    (entry: ZipArchiveEntry option)
    =
    match entry with
    | Some _ -> ()
    | None -> absentFiles.Add entryName

let private tryToGetEntry (archive: ZipArchive) (entryName: string) : ZipArchiveEntry option =
    match archive.GetEntry entryName with
    | null -> None
    | entry -> Some entry

let private findAbsentFilesAsync
    (folder: string)
    (connection: DbConnection)
    (archive: ZipArchive)
    : Task<List<string>> =
    task {
        let folderEntities = Books.GetFolderEntitiesByFolderAsync(connection, folder)
        let enumerator = folderEntities.GetAsyncEnumerator()

        let absentFiles = List<string>()

        while! enumerator.MoveNextAsync() do
            let entity = enumerator.Current
            let fileName = Path.ChangeExtension(entity.FileName, entity.Extension)

            do
                fileName
                |> tryToGetEntry archive
                |> appendAbsentFileIfNotExsists fileName absentFiles

        return absentFiles
    }

let private validateFolderAsync
    (fullPath: string)
    (folder: string)
    (progressReport: string -> unit)
    (connection: DbConnection)
    =
    task {
        let! absentFiles = ArchiveUtils.DoWithArchiveAsync(fullPath, findAbsentFilesAsync folder connection)

        if absentFiles.Count > 0 then
            progressReport $"Absent files in {folder}: {absentFiles |> makeAbsentFilesMessage}"
    }

let private validate (pathToArchives: string) (progressReport: string -> unit) (connection: DbConnection) : Task =
    task {
        let folders = Books.GetFoldersAsync connection
        let folderEnumerator = folders.GetAsyncEnumerator()

        while! folderEnumerator.MoveNextAsync() do
            let folder = folderEnumerator.Current

            progressReport $"Validating {folder}"

            let fullPath = Path.Combine(pathToArchives, folder)

            if File.Exists fullPath then
                try
                    do! validateFolderAsync fullPath folder progressReport connection
                with :? InvalidDataException ->
                    progressReport $"Archive {folder} is invalid"
            else
                progressReport $"Archive isn't exsists: {folder}"
    }

let validateCollectionAsync (parameters: CollectionValidatorParameters) (connection: DbConnection) : Task<unit> =
    task {
        "Collection validation" |> parameters.ProgressReport

        do!
            (connection, validate parameters.PathToArchives parameters.ProgressReport)
            |> parameters.DoInTransactionAsync

        "Collection validation finished" |> parameters.ProgressReport
    }
