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
               "Validating 000001-000001.zip"
               "Absent files in 000001-000001.zip: 2.fb2"
               "Validating 000002-000003.zip"
               "Archive isn't exsists: 000002-000003.zip"
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
                    do! ConnectionUtils.DoInTransactionAsync(connection, CollectionValidatorUtils.setUpData)

                    do! validateCollectionAsync collectionValidatorParameters connection
                })

        Assert.That(actualMessages, Is.EqualTo(expected).AsCollection)

    }
