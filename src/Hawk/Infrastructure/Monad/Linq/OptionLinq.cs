﻿namespace Hawk.Infrastructure.Monad.Linq
{
    using System;

    using Hawk.Infrastructure.Monad;

    using static Hawk.Infrastructure.Monad.Utils.Util;

    public static partial class LinqExtension
    {
        public static Option<TReturn> Select<T, TReturn>(this Option<T> @this, Func<T, TReturn> selector) => @this.Match<Option<TReturn>>(
            some => selector(some),
            () => None());

        public static Option<TReturn> Select<T, TReturn>(this Option<T> @this, Func<T, Option<TReturn>> selector) => @this.Match(
            selector,
            () => None());
    }
}
