module HomeLibProt.CollectionImporter.AuthorHierarchicalSearchImporter

open System
open System.Data.Common
open System.Threading.Tasks

open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.DataAccess.AuthorHierarchicalSearch

type AuthorHierarchicalSearchImporterParameters =
    { MaxCountLeafs: int
      DoInTransactionAsync: DbConnection * (DbConnection -> Task) -> Task }

type TreeNode =
    | Authors of Author list
    | SearchNodes of Map<string, TreeNode> * TotalNodes: int

let private convertAuthorsToSearchNodes (depth: int) (authors: Author list) : TreeNode =
    SearchNodes(
        authors
        |> List.groupBy (fun a -> a.Name.Substring(0, depth + 1))
        |> List.map (fun (k, a) -> k, a |> Authors)
        |> Map,
        TotalNodes = authors.Length
    )

let private mapAuthorsToTreeNode
    (mapAuthors: Author list -> TreeNode)
    (maxCountLeafs: int)
    (authors: Author list)
    : TreeNode =
    if authors.Length > maxCountLeafs then
        authors |> mapAuthors
    else
        authors |> Authors

let private addAuthorToList (author: Author) (authors: Author list) : Author list = [ author ] |> List.append authors

let private isNodeExisits (key: string) (tree: Map<String, TreeNode>) : bool = tree |> Map.containsKey key

let rec internal addAuthorToTree
    (maxCountLeafs: int)
    (tree: Map<String, TreeNode>)
    (author: Author)
    (depth: int)
    : Map<string, TreeNode> =
    let key = author.Name.Substring(0, depth)

    if isNodeExisits key tree |> not then
        tree |> Map.add key (Authors [ author ])
    else
        let node = tree.[key]

        match node with
        | Authors a ->
            tree
            |> Map.change key (fun _ ->
                a
                |> addAuthorToList author
                |> mapAuthorsToTreeNode (convertAuthorsToSearchNodes depth) maxCountLeafs
                |> Some)
        | SearchNodes(n, totalNodes) ->
            tree
            |> Map.change key (fun _ ->
                (addAuthorToTree maxCountLeafs n author (depth + 1), totalNodes + 1)
                |> SearchNodes
                |> Some)

let private optionToNullable (v: 'T option) : Nullable<'T> =
    match v with
    | Some v -> Nullable(v)
    | None -> Nullable()

let rec internal insertTree
    (connection: DbConnection)
    (tree: Map<string, TreeNode>)
    (previousNodeId: int64 option)
    (depth: int)
    : Task<unit> =
    task {
        for key in tree.Keys do
            let node = tree.[key]

            match node with
            | Authors authors ->
                let! nodeId =
                    SearchNodes.InsertSearchNodeAsync(
                        connection,
                        SearchNodeParam(
                            Letters = key,
                            AuthorsCount = authors.Length,
                            PreviousId = (previousNodeId |> optionToNullable)
                        )
                    )

                for a in authors do
                    do!
                        SearchResults.InsertSearchResultAsync(
                            connection,
                            SearchResultParam(NodeId = nodeId, AuthorId = a.Id)
                        )
            | SearchNodes(n, totalNodes) ->
                let! nodeId =
                    SearchNodes.InsertSearchNodeAsync(
                        connection,
                        SearchNodeParam(
                            Letters = key,
                            AuthorsCount = totalNodes,
                            PreviousId = (previousNodeId |> optionToNullable)
                        )
                    )

                do! insertTree connection n (Some nodeId) (depth + 1)

    }

let private import (maxCountLeafs: int) (connection: DbConnection) : Task =
    task {
        let mutable tree = Map.empty
        let mutable previousKey = String.Empty
        let mutable currentKey = String.Empty

        let authors = Authors.GetAuthorsAsync connection
        let enumerator = authors.GetAsyncEnumerator()

        while! enumerator.MoveNextAsync() do
            let author = enumerator.Current
            currentKey <- author.Name.Substring(0, 1)

            if previousKey = currentKey |> not then
                do! insertTree connection tree None 1

                tree <- tree |> Map.remove previousKey

                previousKey <- currentKey

            tree <- addAuthorToTree maxCountLeafs tree author 1

        do! insertTree connection tree None 1
    }

let importAuthorHierarchicalSearchToDbAsync
    (parameters: AuthorHierarchicalSearchImporterParameters)
    (connection: DbConnection)
    : Task<unit> =
    task { do! (connection, import parameters.MaxCountLeafs) |> parameters.DoInTransactionAsync }
