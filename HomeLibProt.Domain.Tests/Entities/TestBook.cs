namespace HomeLibProt.Domain.Tests.Entities;

public record TestBook(long Id, string Title, string FileName, long Size, string LibId, long Deleted, string Extension, string Date, long ArchiveId, long? LibRate, long LanguageId);
