using NDatasModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace Agenda_ICS.Views.Calendar
{
    class TaskUI : Label
    {
        public ITask _task;

        private CalendarOfEmployee.IOwnerOfTaskUI _owner;

        public TaskUI(CalendarOfEmployee.IOwnerOfTaskUI owner) : base()
        {
            _owner = owner;
            MouseUp += new MouseButtonEventHandler(OnMouseUp);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _owner.OnMouseUpOnTaskUI(this, e);
        }
   }
}
