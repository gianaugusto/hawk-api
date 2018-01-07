﻿namespace Hawk.Domain.Queries.Transaction
{
    using System.Threading.Tasks;

    using Hawk.Domain.Entities;
    using Hawk.Infrastructure;

    using Http.Query.Filter;

    public interface IGetAllQuery
    {
        Task<Paged<Transaction>> GetResult(string email, Filter filter);
    }
}
