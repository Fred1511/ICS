using System;

namespace NDatasModel
{
    public class CEmployee : IEmployee
    {
        public long KeyId => _keyId;

        public string Name => _name;

        public CEmployee(long keyId, string name)
        {
            _keyId = keyId;
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public long _keyId;

        public string _name;

        public override string ToString()
        {
            return _name;
        }
    }
}
