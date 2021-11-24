using System;

namespace Play.Catalog.Contracts
{
    public record CatelogItemCreated(Guid Id, string Name, string Description);

    public record CatelogItemUpdated(Guid Id, string Name, string Description);

    public record CatelogItemDeleted(Guid Id);
}
