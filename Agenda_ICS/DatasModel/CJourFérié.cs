using System;

namespace NDatasModel
{
    public class CJourFérié : IJourFérié
    {
        public long KeyId => _keyId;

        public DateTime Jour => _jour;

        public CJourFérié(long keyId, DateTime jour)
        {
            _keyId = keyId;
            _jour = jour;
        }

        public long _keyId;

        public DateTime _jour;

        public override string ToString()
        {
            return _jour.ToString();
        }
    }
}
