using NDatasModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.Calendar
{
    class CTaskCustomMenuItem : MenuItem
    {
        public ITask _task;
    }

    class CContextMenuOnTaskController : IDialogWndOwner
    {
        // *** PUBLIC ***************************

        public interface IParent
        {
            void UpdateDisplayOfTasks();

            CEditTaskController EditTaskController { get; }

            void ResetDisplayOfCalendar();
        }

        public CContextMenuOnTaskController(IParent parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public void ShowContextMenuOnTask(ITask task)
        {
            var menuModifyTask = new CTaskCustomMenuItem();
            menuModifyTask._task = task;
            menuModifyTask.Header = "Modifier tâche";
            menuModifyTask.Click += OnSelectModifyTask;

            var menuDeleteTask = new CTaskCustomMenuItem();
            menuDeleteTask._task = task;
            menuDeleteTask.Header = "Supprimer tâche";
            menuDeleteTask.Click += OnSelectDeleteTask;

            var menuDuplicateTask = new CTaskCustomMenuItem();
            menuDuplicateTask._task = task;
            menuDuplicateTask.Header = "Dupliquer tâche";
            menuDuplicateTask.Click += OnSelectDuplicateTask;

            var menuModifyChantier = new CTaskCustomMenuItem();
            menuModifyChantier._task = task;
            menuModifyChantier.Header = "Modifier chantier";
            menuModifyChantier.Click += OnSelectModifyChantier;

            var menu = new ContextMenu();
            menu.Items.Add(menuModifyTask);
            menu.Items.Add(menuDuplicateTask);
            menu.Items.Add(menuDeleteTask);
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            menu.Items.Add(menuModifyChantier);
            menu.IsOpen = true;
        }

        public void OnSelectModifyTask(object sender, RoutedEventArgs e)
        {
            var task = (sender as CTaskCustomMenuItem)._task;
            _parent.EditTaskController.OnModifyTask(task);
        }

        public void OnSelectDeleteTask(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes != MessageBox.Show("Souhaitez-vous vraiment supprimer cette tâche ?",
                "Confirmation", MessageBoxButton.YesNo))
            {
                return;
            }

            var task = (sender as CTaskCustomMenuItem)._task;
            Model.Instance.DeleteTask(task.KeyId);
            _parent.UpdateDisplayOfTasks();
        }
        
        public void OnSelectModifyChantier(object sender, RoutedEventArgs e)
        {
            if (null != _chantierEditorDialog)
            {
                return;
            }

            var task = (sender as CTaskCustomMenuItem)._task;
            var chantier = Model.Instance.GetChantier(task.ChantierKeyId);
            if (null == chantier)
            {
                // une anomalie est survenue
                return;
            }
            _chantierEditorDialog = new Editors.ChantierEditorDialog(this, chantier);
            _chantierEditorDialog.ShowDialog();
            _parent.ResetDisplayOfCalendar();
        }

        public void OnSelectDuplicateTask(object sender, RoutedEventArgs e)
        {
            if (null != _chantierEditorDialog)
            {
                return;
            }

            var task = (sender as CTaskCustomMenuItem)._task;
            _parent.EditTaskController.OnCreateTask(task);
        }

        public void OnCloseDialog(Window wnd)
        {
        }


        // *** RESTRICTED *************************

        private readonly IParent _parent;

        Editors.ChantierEditorDialog _chantierEditorDialog;
    }
}
