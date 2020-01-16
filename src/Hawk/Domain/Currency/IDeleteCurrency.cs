﻿namespace Hawk.Domain.Currency
{
    using System.Threading.Tasks;

    using Hawk.Domain.Shared;
    using Hawk.Infrastructure.Monad;

    public interface IDeleteCurrency
    {
        Task<Try<Unit>> Execute(Email email, string name);
    }
}