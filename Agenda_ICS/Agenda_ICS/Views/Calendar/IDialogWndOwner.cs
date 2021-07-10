using System.Windows;

namespace Agenda_ICS.Views.Calendar
{
    public interface IDialogWndOwner
    {
        void OnCloseDialog(Window wnd);
    }
}
