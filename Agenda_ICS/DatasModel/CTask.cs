using System;
using System.Collections.Generic;

namespace NDatasModel
{
    public class CTask : ITask
    {
        // *** INTERFACE **********************

        public long KeyId => _keyId;

        public DateTime BeginsAt => _beginsAt;

        public DateTime EndsAt => _endsAt;

        public long ChantierKeyId => _chantierKeyId;

        public long EmployeeKeyId => _employeeKeyId;

        public double AltitudeUI => _altitudeUI;

        // *** DATAS ***************************

        public long _keyId;

        public DateTime _beginsAt;

        public DateTime _endsAt;

        public long _chantierKeyId;

        public long _employeeKeyId;

        public double _altitudeUI = 0;

        public bool _isCopy;

        // *** METHODES PUBLIQUES **************

        public CTask(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt, long keyId)
        {
            _beginsAt = beginsAt;
            _endsAt = endsAt;
            _chantierKeyId = chantierKeyId;
            _employeeKeyId = employeeKeyId;
            _keyId = keyId;
        }

        public CTask(long keyId)
        {
            _keyId = keyId;
            _isCopy = false;
        }

        public CTask(ITask taskToCopy)
        {
            _keyId = taskToCopy.KeyId;
            _isCopy = true;

            CopyDatasFromTask(taskToCopy);
        }

        public void CopyDatasFromTask(ITask taskToCopy)
        {
            _beginsAt = taskToCopy.BeginsAt;
            _endsAt = taskToCopy.EndsAt;
            _employeeKeyId = taskToCopy.EmployeeKeyId;
            _chantierKeyId = taskToCopy.ChantierKeyId;
            _altitudeUI = taskToCopy.AltitudeUI;
        }

        public bool IsTaskRunningDuring(DateTime time)
        {
            return (time >= _beginsAt && time <= _endsAt);
        }
    }
}
