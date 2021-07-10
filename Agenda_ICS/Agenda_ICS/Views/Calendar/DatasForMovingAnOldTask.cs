using NDatasModel;
using System.Windows;

namespace Agenda_ICS.Views.Calendar
{
    class DatasForMovingAnOldTask
    {
        public CTask _task;

        public TaskUI _taskUI;

        public Point _initialMousePosition;

        public double _initialTaskAltitudeUI;

        public DatasForMovingAnOldTask(TaskUI taskUI, Point initialMousePosition)
        {
            _taskUI = taskUI;
            _task = Model.Instance.AskCopyOfTaskForModification(_taskUI._task.KeyId);
            _taskUI._task = _task;

            _initialMousePosition = initialMousePosition;
            _initialTaskAltitudeUI = taskUI._task.AltitudeUI;
        }

        public void Release()
        {
            Model.Instance.InformThatCopyIsNoLongerNecessary(_task.KeyId);
        }

        public void OnEndMovingAndValidModifications()
        {
            var newTask = Model.Instance.ModifyTask(_task);
            if (null != newTask)
            {
                _taskUI._task = newTask;
            }
        }
    }
}
