﻿namespace Hawk.Domain.Payee.Data.Neo4J.Commands
{
    using System.Threading.Tasks;

    using Hawk.Domain.Payee;
    using Hawk.Domain.Payee.Commands;
    using Hawk.Domain.Shared.Commands;
    using Hawk.Infrastructure.Data.Neo4J;
    using Hawk.Infrastructure.Monad;

    using static System.IO.Path;

    using static Hawk.Infrastructure.Data.Neo4J.CypherScript;

    internal sealed class UpsertPayee : Command<UpsertParam<string, Payee>>, IUpsertPayee
    {
        private static readonly Option<string> Statement = ReadCypherScript(Combine("Payee", "Data.Neo4J", "Commands", "UpsertPayee.cql"));
        private readonly Neo4JConnection connection;

        public UpsertPayee(Neo4JConnection connection) => this.connection = connection;

        protected override Task<Try<Unit>> Execute(UpsertParam<string, Payee> param) => this.connection.ExecuteCypher(
            Statement,
            new
            {
                email = param.Email.Value,
                name = param.Id,
                newName = param.Entity.Id,
            });
    }
}
