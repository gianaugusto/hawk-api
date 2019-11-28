﻿namespace Hawk.Infrastructure.Data.Neo4J.Entities.Tag
{
    using System.Threading.Tasks;

    using Hawk.Domain.Shared;
    using Hawk.Domain.Tag;
    using Hawk.Infrastructure.Filter;
    using Hawk.Infrastructure.Monad;

    using Http.Query.Filter;

    using static Hawk.Infrastructure.Data.Neo4J.CypherScript;
    using static Hawk.Infrastructure.Data.Neo4J.Entities.Tag.TagMapping;

    internal sealed class GetTagsByStore : IGetTagsByStore
    {
        private static readonly Option<string> Statement = ReadCypherScript("Tag\\GetTagsByStore.cql");
        private readonly Neo4JConnection connection;
        private readonly ILimit<int, Filter> limit;
        private readonly ISkip<int, Filter> skip;

        public GetTagsByStore(
            Neo4JConnection connection,
            ILimit<int, Filter> limit,
            ISkip<int, Filter> skip)
        {
            this.connection = connection;
            this.limit = limit;
            this.skip = skip;
        }

        public async Task<Try<Page<Try<(Tag Tag, uint Count)>>>> GetResult(Email email, string store, Filter filter)
        {
            var parameters = new
            {
                email = email.ToString(),
                store,
                skip = this.skip.Apply(filter),
                limit = this.limit.Apply(filter),
            };

            var data = await this.connection.ExecuteCypher(MapTag, Statement, parameters);

            return data.Match<Try<Page<Try<(Tag, uint)>>>>(
                _ => _,
                items => new Page<Try<(Tag, uint)>>(items, parameters.skip, parameters.limit));
        }
    }
}