﻿namespace Hawk.Domain.PaymentMethod.Data.Neo4J
{
    using System.Threading.Tasks;

    using Hawk.Domain.PaymentMethod;
    using Hawk.Domain.Shared;
    using Hawk.Infrastructure.Data.Neo4J;
    using Hawk.Infrastructure.ErrorHandling.Exceptions;
    using Hawk.Infrastructure.Monad;

    using static System.IO.Path;

    using static Hawk.Domain.PaymentMethod.Data.Neo4J.PaymentMethodMapping;
    using static Hawk.Infrastructure.Data.Neo4J.CypherScript;
    using static Hawk.Infrastructure.Monad.Utils.Util;

    internal sealed class UpsertPaymentMethod : IUpsertPaymentMethod
    {
        private static readonly Option<string> Statement = ReadCypherScript(Combine("PaymentMethod", "Data.Neo4J", "UpsertPaymentMethod.cql"));
        private readonly Neo4JConnection connection;

        public UpsertPaymentMethod(Neo4JConnection connection) => this.connection = connection;

        public Task<Try<PaymentMethod>> Execute(Option<Email> email, Option<PaymentMethod> entity) => entity.Match(
            some => this.Execute(email, some.Value, some),
            () => Task(Failure<PaymentMethod>(new NullObjectException("Payment method is required."))));

        public Task<Try<PaymentMethod>> Execute(Option<Email> email, Option<string> name, Option<PaymentMethod> entity) =>
            email
            && name
            && entity
                ? this.connection.ExecuteCypherScalar(
                    MapPaymentMethod,
                    Statement,
                    new
                    {
                        email = email.Get().Value,
                        name = name.Get(),
                        newName = entity.Get().Value,
                    })
                : Task(Failure<PaymentMethod>(new NullObjectException("Parameters are required.")));
    }
}