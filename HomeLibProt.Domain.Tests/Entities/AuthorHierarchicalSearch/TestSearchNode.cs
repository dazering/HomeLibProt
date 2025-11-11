namespace HomeLibProt.Domain.Tests.Entities.AuthorHierarchicalSearch;

public record TestSearchNode(long Id, string Letters, long AuthorsCount, long? PreviousId);
