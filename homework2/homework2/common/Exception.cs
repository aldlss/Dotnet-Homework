using System;

namespace homework2.common;

public class BadCashException: Exception
{
    public BadCashException(string message) : base(message)
    {
    }
}
