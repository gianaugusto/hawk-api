﻿namespace Hawk.Infrastructure.Data.Neo4J.Entities.Store
{
    using Hawk.Domain.Shared.Exceptions;
    using Hawk.Domain.Store;
    using Hawk.Infrastructure.Monad;

    using Neo4j.Driver.V1;

    using static Hawk.Domain.Store.Store;

    using static Neo4JRecord;

    internal static class StoreMapping
    {
        private const string Name = "name";
        private const string Total = "total";

        internal static Try<(Store Store, uint Count)> MapStore(IRecord data) => MapRecord(data, "data").Match(
            record =>
            {
                var total = record.Get<uint>(Total);
                if (!total.IsDefined)
                {
                    return new InvalidObjectException("Invalid store.");
                }

                return MapStore(record).Match<Try<(Store, uint)>>(
                    _ => _,
                    store => (store, total.Get()));
            },
            () => new NotFoundException("Store not found."));

        internal static Try<Store> MapStore(Option<Neo4JRecord> record) => record.Match(
            some => NewStore(some.Get<string>(Name)),
            () => new NotFoundException("Store not found."));
    }
}
