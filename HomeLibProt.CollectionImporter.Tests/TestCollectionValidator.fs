module HomeLibProt.CollectionImporter.Tests.TestCollectionValidator

open NUnit.Framework
open System
open System.IO

open HomeLibProt.CollectionImporter.CollectionValidator
open HomeLibProt.CollectionImporter.Tests.Utils
open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.Tests.Utils

let getArchivesPath (testCaseName: string) =
    Path.Combine(TestCaseUtils.getTestCasesBasePath "TestCollectionValidator", testCaseName)

[<Test>]
let TestValidateCollectionAsync () =
    task {
        let expected =
            [| "Collection validation"
               "Validating 000001-000002.zip"
               "Absent files in 000001-000002.zip: 2.fb2"
               "Validating 000003-000003.zip"
               "Archive isn't exsists: 000003-000003.zip"
               "Collection validation finished" |]

        let actualMessages = Collections.Generic.List<string>()

        let getReportMessages (message: string) = actualMessages.Add message

        let collectionValidatorParameters =
            { PathToArchives = getArchivesPath "Simple"
              ProgressReport = getReportMessages
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        do!
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun c ->
                                task {
                                    let! _ =
                                        BookUtils.Create(
                                            c,
                                            title = "Title1",
                                            fileName = "1",
                                            size = 1,
                                            libId = "1",
                                            deleted = false,
                                            extension = "fb2",
                                            date = "2025-11-07",
                                            folder = "000001-000002.zip",
                                            libRate = 0
                                        )

                                    let! _ =
                                        BookUtils.Create(
                                            c,
                                            title = "Title2",
                                            fileName = "2",
                                            size = 1,
                                            libId = "2",
                                            deleted = false,
                                            extension = "fb2",
                                            date = "2025-11-07",
                                            folder = "000001-000002.zip",
                                            libRate = 0
                                        )

                                    let! _ =
                                        BookUtils.Create(
                                            c,
                                            title = "Title3",
                                            fileName = "3",
                                            size = 1,
                                            libId = "3",
                                            deleted = false,
                                            extension = "fb2",
                                            date = "2025-11-07",
                                            folder = "000003-000003.zip",
                                            libRate = 0
                                        )

                                    do ()
                                }
                        )

                    do! validateCollectionAsync collectionValidatorParameters connection
                })

        Assert.That(actualMessages, Is.EqualTo(expected).AsCollection)

    }
