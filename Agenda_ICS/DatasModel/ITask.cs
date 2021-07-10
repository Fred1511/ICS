using System;

namespace NDatasModel
{
    public interface ITask
    {
        long KeyId { get; }

        DateTime BeginsAt { get; }

        DateTime EndsAt { get; }

        long ChantierKeyId { get; }

        long EmployeeKeyId { get; }

        double AltitudeUI { get; }

        bool IsTaskRunningDuring(DateTime time);
    }
}