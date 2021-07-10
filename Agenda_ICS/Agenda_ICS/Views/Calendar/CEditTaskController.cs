using NDatasModel;
using System;
using System.Windows;

namespace Agenda_ICS.Views.Calendar
{
    class CEditTaskController : IDialogWndOwner
    {
        // *** PUBLIC ***************************

        public CEditTaskController(ICalendarDisplayerForChildrens calendar)
        {
            _calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        }

        public void OnCloseDialog(Window wnd)
        {
            _taskEditorDialog = null;
            _calendar.ResetDisplayOfCalendar();
        }

        public void OnModifyTask(ITask taskToModify)
        {
            if (null == _taskEditorDialog)
            {
                EditTask(taskToModify, TaskEditorDialog.EMode.MODIFICATION);
            }
        }

        public void OnCreateTask(ITask newTaskDraft)
        {
            EditTask(newTaskDraft, TaskEditorDialog.EMode.CREATION);
        }

        // *** RESTRICTED ***********************

        private readonly ICalendarDisplayerForChildrens _calendar;

        private TaskEditorDialog _taskEditorDialog;

        private void EditTask(ITask taskToEdit, TaskEditorDialog.EMode mode)
        {
            _taskEditorDialog = new TaskEditorDialog(this, taskToEdit, mode);
            _taskEditorDialog.ShowDialog();
        }
    }
}
