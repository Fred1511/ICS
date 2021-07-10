using Agenda_ICS.Views.Calendar;
using Agenda_ICS.Views.EmployeeEditor;
using NDatasModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Agenda_ICS.Views.Configuration.Pages
{
    /// <summary>
    /// Logique d'interaction pour EmployeesConfig.xaml
    /// </summary>
    public partial class EmployeesConfig : UserControl, IDialogWndOwner
    {
        public EmployeesConfig()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateListOfEmployees();
        }

        private void UpdateListOfEmployees(long selectedEmployeeKeyId = -1)
        {
            ListOfEmployees.Items.Clear();
            var listOfEmployees = Model.Instance.GetEmployees();
            var index = 0;
            foreach (var employee in listOfEmployees)
            {
                ListOfEmployees.Items.Add(employee);

                if (employee.KeyId == selectedEmployeeKeyId)
                {
                    ListOfEmployees.SelectedIndex = index;
                }

                index++;
            }
        }

        public void OnCloseDialog(Window wnd)
        {
            if (wnd == _editDialog)
            {
                var employee = _editDialog._employee;

                switch (_editDialog._mode)
                {
                    case EmployeeEditorDialog.EMode.CREATION:
                        var keyIdOfCreatedEmployee = Model.Instance.CreateEmployee(employee.Name);
                        UpdateListOfEmployees(keyIdOfCreatedEmployee);
                        break;
                    case EmployeeEditorDialog.EMode.MODIFICATION:
                        Model.Instance.ModifyEmployee(employee.KeyId, employee.Name);
                        UpdateListOfEmployees(employee.KeyId);
                        break;
                }

                _editDialog = null;
            }
        }

        EmployeeEditorDialog _editDialog;

        private void OnClick_RemoveEmployee(object sender, RoutedEventArgs e)
        {

        }

        private void OnClick_ModifyEmployee(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = (IEmployee)ListOfEmployees.SelectedItem;
            if (null == selectedEmployee)
            {
                MessageBox.Show("Veuillez sélectionner un employé à modifier", "Impossible", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _editDialog = new EmployeeEditorDialog(this, selectedEmployee);
            _editDialog.ShowDialog();
        }

        private void OnClick_CreateEmployee(object sender, RoutedEventArgs e)
        {
            _editDialog = new EmployeeEditorDialog(this);
            _editDialog.ShowDialog();
        }
    }
}
