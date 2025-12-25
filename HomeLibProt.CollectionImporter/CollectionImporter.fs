module HomeLibProt.CollectionImporter.CollectionImporter

open System.Data.Common
open System.Threading.Tasks

open HomeLibProt.Domain.DataAccess

type ImportInpxParameters =
    { PathToInpx: string
      BatchSize: int
      PathToArchives: string
      MaxCountLeafs: int
      ProgressReport: string -> unit
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

type ReimportAHSParameters =
    { MaxCountLeafs: int
      ProgressReport: string -> unit
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

let importCollectionToDb (parameters: ImportInpxParameters) (connection: DbConnection) : Task =
    task {
        do! (connection, DbStructure.CreateFullStructure) |> parameters.DoInTransactionAsync

        let inpxImporterParameters: InpxImporter.InpxImporterParameters =
            { PathToInpx = parameters.PathToInpx
              BatchSize = parameters.BatchSize
              ProgressReport = parameters.ProgressReport
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! InpxImporter.importInpxToDb inpxImporterParameters connection

        let collectionValidatorParameters: CollectionValidator.CollectionValidatorParameters =
            { PathToArchives = parameters.PathToArchives
              ProgressReport = parameters.ProgressReport
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! CollectionValidator.validateCollectionAsync collectionValidatorParameters connection

        let ahsImporterParameters: AuthorHierarchicalSearchImporter.AuthorHierarchicalSearchImporterParameters =
            { MaxCountLeafs = parameters.MaxCountLeafs
              ProgressReport = parameters.ProgressReport
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! AuthorHierarchicalSearchImporter.importAuthorHierarchicalSearchToDbAsync ahsImporterParameters connection
    }

let reimportAuthorHierarchicalSearch (parameters: ReimportAHSParameters) (connection: DbConnection) : Task =
    task {
        do! (connection, DbStructure.CreateAHSStructure) |> parameters.DoInTransactionAsync

        let ahsImporterParameters: AuthorHierarchicalSearchImporter.AuthorHierarchicalSearchImporterParameters =
            { MaxCountLeafs = parameters.MaxCountLeafs
              ProgressReport = parameters.ProgressReport
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! AuthorHierarchicalSearchImporter.importAuthorHierarchicalSearchToDbAsync ahsImporterParameters connection
    }
