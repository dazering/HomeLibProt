# HomeLibProt
Asp home library project based on sql-server.

Проект представляет собой каталог книг. Источник с книгами представляет собой архив с файлами fb2. Помимо возможности скачать книгу, реализована возможность указать путь до архива(-ов) с дальнейшем их сканированием и записью в базу данных.

Стек:
Asp.net Core Mvc, EF Core, SignalR

Так же есть ряд проблем в проекте:
1) Разметку надо поправить. Т.к. в этом проекте в первый раз столкнулся с bootsrap и css в целом.
2) По хорошему надо было бы если не закрыть полностью страницу со статусом сканера от неавторизованного пользователя, то хотя бы закрыть управление сканером.

В планах вынести фронт на Angular, бэкенд переделать в api, а так же исправить\дополнить выше написанное.
