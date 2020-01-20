"use strict"

var connection = new signalR.HubConnectionBuilder().withUrl("/scanhub").build();

const countLocalRepoLabel = document.getElementById("countLocalRepo");
const countInDb = document.getElementById("countInDb");
const notAddedBooks = document.getElementById("notAddedBooks");
const isScanning = document.getElementById("isScanning");
const elapsedTime = document.getElementById("elapsedTime");
const startTime = document.getElementById("startTime");
const startScan = document.getElementById("startBtn");
const cancelScaning = document.getElementById("cancelBtn");

connection.on("getStatus", function(status) {
        countLocalRepoLabel.innerText = status.booksInLocalRepository;
        countInDb.innerText = status.booksInDataBase;
        notAddedBooks.innerText = status.booksNotAddedInDb;
        isScanning.innerText = status.isScanningRun ? "In Progresssss" : "Not runnig";
    elapsedTime.innerText = status.elapsedTime;
    startTime.innerText = status.startTime;
});

startScan.addEventListener("click", function(event) {
    connection.invoke("StartScanning");
    cancelScaning.classList.toggle("disabled");
});

cancelScaning.addEventListener("click",
    function(event) {
        connection.invoke("StopScanning");
        cancelScaning.classList.toggle("disabled");
    });
connection.start();