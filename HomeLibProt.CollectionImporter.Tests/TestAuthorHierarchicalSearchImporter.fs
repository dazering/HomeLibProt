module HomeLibProt.CollectionImporter.Tests.TestAuthorHierarchicalSearchImporter

open NUnit.Framework
open System

open HomeLibProt.CollectionImporter.AuthorHierarchicalSearchImporter
open HomeLibProt.CollectionImporter.Tests.Utils
open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch
open HomeLibProt.Domain.Tests.Utils
open HomeLibProt.Domain.Tests.Utils.AuthorHierarchicalSearch

let addAuthorToTreeTestCases =
    [| TestCaseData(
           Author(Name = "B B B", Id = 1),
           2,
           Map.empty<string, TreeNode>,
           Map [ ("B", TreeNode.Authors [ Author(Id = 1, Name = "B B B") ]) ]
       )
       TestCaseData(
           Author(Name = "Baa Baa Baa", Id = 2),
           2,
           Map [ ("B", TreeNode.Authors [ Author(Id = 1, Name = "B B B") ]) ],
           Map [ ("B", TreeNode.Authors [ Author(Id = 1, Name = "B B B"); Author(Id = 2, Name = "Baa Baa Baa") ]) ]
       )
       TestCaseData(
           Author(Name = "Bab Bab Bab", Id = 3),
           2,
           Map [ ("B", TreeNode.Authors [ Author(Id = 1, Name = "B B B"); Author(Id = 2, Name = "Baa Baa Baa") ]) ],
           Map
               [ ("B",
                  SearchNodes(
                      Map
                          [ "B ", TreeNode.Authors [ Author(Id = 1, Name = "B B B") ]
                            "Ba",
                            TreeNode.Authors
                                [ Author(Id = 2, Name = "Baa Baa Baa"); Author(Id = 3, Name = "Bab Bab Bab") ] ],
                      TotalNodes = 3
                  )) ]
       )
       TestCaseData(
           Author(Name = "Bac Bac Bac", Id = 4),
           2,
           Map
               [ ("B",
                  SearchNodes(
                      Map
                          [ "B ", TreeNode.Authors [ Author(Id = 1, Name = "B B B") ]
                            "Ba",
                            TreeNode.Authors
                                [ Author(Id = 2, Name = "Baa Baa Baa"); Author(Id = 3, Name = "Bab Bab Bab") ] ],
                      TotalNodes = 3
                  )) ],
           Map
               [ ("B",
                  SearchNodes(
                      Map
                          [ "B ", TreeNode.Authors [ Author(Id = 1, Name = "B B B") ]
                            "Ba",
                            SearchNodes(
                                Map
                                    [ "Baa", TreeNode.Authors [ Author(Id = 2, Name = "Baa Baa Baa") ]
                                      "Bab", TreeNode.Authors [ Author(Id = 3, Name = "Bab Bab Bab") ]
                                      "Bac", TreeNode.Authors [ Author(Id = 4, Name = "Bac Bac Bac") ] ],
                                TotalNodes = 3
                            ) ],
                      TotalNodes = 4
                  )) ]
       ) |]

[<Test>]
[<TestCaseSource(nameof addAuthorToTreeTestCases)>]
let TestAddAuthorToTree
    (author: Author, maxCountLeafs: int, tree: Map<string, TreeNode>, expected: Map<string, TreeNode>)
    =

    let actual = addAuthorToTree maxCountLeafs tree author 1

    Assert.That(actual, Is.EqualTo(expected).AsCollection)

[<Test>]
let TestInsertTree () =
    task {
        let expectedSearchNode =
            [| TestSearchNode(Id = 1L, Letters = "B", AuthorsCount = 3, PreviousId = Nullable())
               TestSearchNode(Id = 2L, Letters = "B ", AuthorsCount = 1, PreviousId = Nullable 1L)
               TestSearchNode(Id = 3L, Letters = "Ba", AuthorsCount = 2, PreviousId = Nullable 1L) |]

        let expectedSearchResults =
            [| TestSearchResult(Id = 1L, NodeId = 2L, AuthorId = 1)
               TestSearchResult(Id = 2L, NodeId = 3L, AuthorId = 2)
               TestSearchResult(Id = 3L, NodeId = 3L, AuthorId = 3) |]

        let tree =
            Map
                [ ("B",
                   SearchNodes(
                       Map
                           [ "B ", TreeNode.Authors [ Author(Id = 1, Name = "B B B") ]
                             "Ba",
                             TreeNode.Authors
                                 [ Author(Id = 2, Name = "Baa Baa Baa"); Author(Id = 3, Name = "Bab Bab Bab") ] ],
                       TotalNodes = 3
                   )) ]

        let! searchNodes, searchResults =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            AuthorHierarchicalSearchImporterUtils.setUpData
                        )

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun c -> task { do! insertTree c tree None 1 }
                        )

                    let! searchNodes = ConnectionUtils.DoInTransactionAsync(connection, SearchNodeUtils.GetTestData)

                    let! searchResults =
                        ConnectionUtils.DoInTransactionAsync(connection, SearchResultUtils.GetTestData)

                    return searchNodes, searchResults
                })

        Assert.That(searchNodes, Is.EqualTo(expectedSearchNode).AsCollection)
        Assert.That(searchResults, Is.EqualTo(expectedSearchResults).AsCollection)

    }

[<Test>]
let TestImportAuthorHierarchicalSearchToDbAsync () =
    task {
        let expectedSearchNode =
            [| TestSearchNode(Id = 1L, Letters = "B", AuthorsCount = 3, PreviousId = Nullable())
               TestSearchNode(Id = 2L, Letters = "B ", AuthorsCount = 1, PreviousId = Nullable 1L)
               TestSearchNode(Id = 3L, Letters = "Ba", AuthorsCount = 2, PreviousId = Nullable 1L) |]

        let expectedSearchResults =
            [| TestSearchResult(Id = 1L, NodeId = 2L, AuthorId = 1)
               TestSearchResult(Id = 2L, NodeId = 3L, AuthorId = 2)
               TestSearchResult(Id = 3L, NodeId = 3L, AuthorId = 3) |]

        let authorHierarchicalSearchImporterParameters =
            { MaxCountLeafs = 2
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        let! searchNodes, searchResults =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            AuthorHierarchicalSearchImporterUtils.setUpData
                        )

                    do! importAuthorHierarchicalSearchToDbAsync authorHierarchicalSearchImporterParameters connection

                    let! searchNodes = ConnectionUtils.DoInTransactionAsync(connection, SearchNodeUtils.GetTestData)

                    let! searchResults =
                        ConnectionUtils.DoInTransactionAsync(connection, SearchResultUtils.GetTestData)

                    return searchNodes, searchResults
                })

        Assert.That(searchNodes, Is.EqualTo(expectedSearchNode).AsCollection)
        Assert.That(searchResults, Is.EqualTo(expectedSearchResults).AsCollection)

    }
