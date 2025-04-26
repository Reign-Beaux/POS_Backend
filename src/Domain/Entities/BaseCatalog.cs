﻿namespace Domain.Entities
{
    public abstract class BaseCatalog : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public const int NameMaxLength = 64;
        public const int DescriptionMaxLength = 256;
    }
}
