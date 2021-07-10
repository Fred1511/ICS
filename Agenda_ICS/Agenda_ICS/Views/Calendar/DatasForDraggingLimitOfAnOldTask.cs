using NDatasModel;
using System;

namespace Agenda_ICS.Views.Calendar
{
    class DatasForDraggingLimitOfAnOldTask
    {
        public enum ELimit
        {
            BEGINNING,
            ENDING
        }

        public CTask _task;

        public TaskUI _taskUI;

        public ELimit _typeLimite;

        public DateTime _newLimit;

        public DatasForDraggingLimitOfAnOldTask(TaskUI taskUI, ELimit typeLimite)
        {
            _taskUI = taskUI ?? throw new ArgumentNullException(nameof(taskUI));
            _typeLimite = typeLimite;

            switch (_typeLimite)
            {
                case ELimit.BEGINNING:
                    _newLimit = taskUI._task.BeginsAt;
                    break;
                case ELimit.ENDING:
                    _newLimit = taskUI._task.EndsAt;
                    break;
            }

            _task = Model.Instance.AskCopyOfTaskForModification(_taskUI._task.KeyId);
            _taskUI._task = _task;
        }

        public void Release()
        {
            Model.Instance.InformThatCopyIsNoLongerNecessary(_task.KeyId);
        }

        public void OnEndDraggingAndValidModifications()
        {
            _taskUI._task = Model.Instance.ModifyTask(_task);
        }

        public bool IsValid()
        {
            switch (_typeLimite)
            {
                case ELimit.BEGINNING:
                    return _task._endsAt > _newLimit;
                case ELimit.ENDING:
                    return _task._beginsAt < _newLimit;
            }

            throw new System.ArgumentException();
        }

        public void UpdateNewLimit(DateTime newLimit)
        {
            _newLimit = newLimit;

            if (false == IsValid())
            {
                return;
            }

            switch (_typeLimite)
            {
                case ELimit.BEGINNING:
                    _task._beginsAt = newLimit;
                    break;
                case ELimit.ENDING:
                    _task._endsAt = newLimit;
                    break;
            }
        }
    }
}
