﻿namespace Hawk.Infrastructure.Data.Neo4J.Mappings
{
    using Hawk.Domain.Entities;

    internal sealed class ParcelMapping
    {
        public Parcel MapFrom(Record record)
        {
            Guard.NotNull(record, nameof(record), "Parcel's record cannot be null.");

            if (!record.Any())
            {
                return null;
            }

            return new Parcel(
                record.Get<int>("total"),
                record.Get<int>("number"));
        }
    }
}