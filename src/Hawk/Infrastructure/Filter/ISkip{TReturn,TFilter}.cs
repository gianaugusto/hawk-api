﻿namespace Hawk.Infrastructure.Filter
{
    using Http.Query.Filter;

    internal interface ISkip<out TReturn, in TFilter>
        where TFilter : IFilter
    {
        TReturn Apply(TFilter filter);
    }
}
