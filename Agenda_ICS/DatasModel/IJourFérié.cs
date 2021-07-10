using System;

namespace NDatasModel
{
    public interface IJourFérié
    {
        long KeyId { get; }

        DateTime Jour { get; }
    }
}
