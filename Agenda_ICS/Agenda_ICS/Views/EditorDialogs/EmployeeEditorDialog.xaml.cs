using Agenda_ICS.Views.Calendar;
using NDatasModel;
using System.Windows;

namespace Agenda_ICS.Views.EmployeeEditor
{
    public partial class EmployeeEditorDialog : Window
    {
        public enum EMode
        {
            CREATION,
            MODIFICATION
        }

        public EmployeeEditorDialog(IDialogWndOwner owner, IEmployee employee)
        {
            _owner = owner;

            InitializeComponent();

            _employee = new CEmployee(employee.KeyId, employee.Name);
            _mode = EMode.MODIFICATION;
        }

        public EmployeeEditorDialog(IDialogWndOwner owner)
        {
            _owner = owner;

            InitializeComponent();

            _employee = new CEmployee(-1, "Nouvel Employé");
            _mode = EMode.CREATION;
            this.Title = "Nouvel employé";
       }

        private void OnClick_Valider(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomEmployé.Text))
            {
                MessageBox.Show("Le nom de l'employé n'a pas été renseigné", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _employee._name = NomEmployé.Text;

            this.Close();
        }

        public CEmployee _employee;

        public readonly EMode _mode;

        private IDialogWndOwner _owner;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _owner.OnCloseDialog(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_mode == EMode.MODIFICATION)
            {
                NomEmployé.Text = _employee.Name;
            }
        }
    }
}
