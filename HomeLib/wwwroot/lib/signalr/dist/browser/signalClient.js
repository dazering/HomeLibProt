"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/scanhub").build();

const countLocalRepoLabel = document.getElementById("countLocalRepo");
const countInDb = document.getElementById("countInDb");
const notAddedBooks = document.getElementById("notAddedBooks");
const isScanning = document.getElementById("isScanning");
const errorsCount = document.getElementById("errorsCount");
const finishTime = document.getElementById("finishTime");
const elapsedTime = document.getElementById("elapsedTime");
const startTime = document.getElementById("startTime");
const startScan = document.getElementById("startBtn");
const cancelScaning = document.getElementById("cancelBtn");
const addedCount = document.getElementById("addedCount");

connection.on("getStatus", function (status) {
    countLocalRepoLabel.innerText = status.booksInLocalRepository;
    countInDb.innerText = status.booksInDataBase;
    notAddedBooks.innerText = status.booksNotAddedInDb;
    errorsCount.innerText = status.currentErrorsCount;
    addedCount.innerText = status.booksAdded;
    isScanning.innerText = status.isScanningRun ? "In Progres" : "Stopped";
    if(status.elapsedTime)
    {
        elapsedTime.innerText = status.elapsedTime;
    }
    if (status.finishTime) {
        finishTime.innerText = status.finishTime;
    }
    if(status.startTime)
    {
        startTime.innerText = status.startTime;
    }
});

startScan.addEventListener("click", function (event) {
    connection.invoke("StartScanning");
    cancelScaning.classList.toggle("disabled");
});

cancelScaning.addEventListener("click",
    function (event) {
        connection.invoke("StopScanning");
        cancelScaning.classList.toggle("disabled");
    });
connection.start();