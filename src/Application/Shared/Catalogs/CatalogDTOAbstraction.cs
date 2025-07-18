﻿namespace Application.Shared.Catalogs
{
    public abstract record CatalogDTOAbstraction
    {
        public Guid? Id { get; init; } = null;
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    };
}
