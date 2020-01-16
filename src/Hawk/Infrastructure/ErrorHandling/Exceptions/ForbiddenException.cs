﻿namespace Hawk.Infrastructure.ErrorHandling.Exceptions
{
    public sealed class ForbiddenException : BaseException
    {
        public ForbiddenException(string message)
            : base(message)
        {
        }
    }
}
