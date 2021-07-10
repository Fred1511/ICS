using System;

namespace NDatasModel
{
    public class CEmployement : IEmployement
    {
        // *** INTERFACE **********************

        public long KeyId => _keyID;

        public long EmployeeId => _employeeId;

        public long ChantierId => _chantierId;

        // *** DATAS ***************************

        public long _keyID;

        public long _employeeId;

        public long _chantierId;
    }
}
