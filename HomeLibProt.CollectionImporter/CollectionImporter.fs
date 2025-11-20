module HomeLibProt.CollectionImporter.CollectionImporter

open System.Data.Common
open System.Threading.Tasks

open HomeLibProt.Domain.DataAccess

type ImportInpxParameters =
    { PathToInpx: string
      BatchSize: int
      MaxCountLeafs: int
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

type ReimportAHSParameters =
    { MaxCountLeafs: int
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

let importCollectionToDb (parameters: ImportInpxParameters) (connection: DbConnection) : Task =
    task {
        do! (connection, DbStructure.CreateFullStructure) |> parameters.DoInTransactionAsync

        let inpxImporterParameters: InpxImporter.InpxImporterParameters =
            { PathToInpx = parameters.PathToInpx
              BatchSize = parameters.BatchSize
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! InpxImporter.importInpxToDb inpxImporterParameters connection

        let ahsImporterParameters: AuthorHierarchicalSearchImporter.AuthorHierarchicalSearchImporterParameters =
            { MaxCountLeafs = parameters.MaxCountLeafs
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! AuthorHierarchicalSearchImporter.importAuthorHierarchicalSearchToDbAsync ahsImporterParameters connection
    }

let reimportAuthorHierarchicalSearch (parameters: ReimportAHSParameters) (connection: DbConnection) : Task =
    task {
        do! (connection, DbStructure.CreateAHSStructure) |> parameters.DoInTransactionAsync

        let ahsImporterParameters: AuthorHierarchicalSearchImporter.AuthorHierarchicalSearchImporterParameters =
            { MaxCountLeafs = parameters.MaxCountLeafs
              DoInTransactionAsync = parameters.DoInTransactionAsync }

        do! AuthorHierarchicalSearchImporter.importAuthorHierarchicalSearchToDbAsync ahsImporterParameters connection
    }
