using Agenda_ICS.Views.Calendar;
using NDatasModel;
using NOutils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Agenda_ICS.Views
{
    /// <summary>
    /// Logique d'interaction pour TaskEditorDialog.xaml
    /// </summary>
    public partial class TaskEditorDialog : Window
    {
        // *** PUBLIC ***************************

        public enum EMode
        {
            CREATION,
            MODIFICATION
        }

        public TaskEditorDialog(IDialogWndOwner owner, ITask task, EMode mode)
        {
            _owner = owner;
            _mode = mode;
            _taskcopy = (mode == EMode.MODIFICATION) ? Model.Instance.AskCopyOfTaskForModification(task.KeyId) : new CTask(task);

            InitializeComponent();

            DayOfBeginningOfTask.Text = DatesExpert.FormatDay(_taskcopy.BeginsAt);
            HourOfBeginningOfTask.Text = DatesExpert.FormatHour(_taskcopy.BeginsAt);
            DayOfEndOfTask.Text = DatesExpert.FormatDay(_taskcopy.EndsAt);
            HourOfEndOfTask.Text = DatesExpert.FormatHour(_taskcopy.EndsAt);
        }

        ~TaskEditorDialog()
        {
            if (EMode.MODIFICATION == _mode)
            {
                Model.Instance.InformThatCopyIsNoLongerNecessary(_taskcopy.KeyId);
            }
        }

        // *** RESTRICTED ***********************

        private IChantier[] _chantiers;

        private EMode _mode;

        private IDialogWndOwner _owner;

        private CTask _taskcopy;

        private readonly DispatcherTimer _timer_500ms = new DispatcherTimer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer_500ms.Tick += OnTimer_500ms;
            _timer_500ms.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer_500ms.Start();
            _timer_500ms.IsEnabled = true;

            var index = 0;
            foreach (var employee in Model.Instance.GetEmployees())
            {
                Employee.Items.Add(employee);
                if (employee.KeyId == _taskcopy._employeeKeyId)
                {
                    Employee.SelectedIndex = index;
                }

                index++;
            }

            index = 0;
            _chantiers = Model.Instance.GetChantiers();
            foreach (var chantier in _chantiers)
            {
                Chantier.Items.Add(chantier);
                if (chantier.KeyId == _taskcopy._chantierKeyId)
                {
                    Chantier.SelectedIndex = index;
                    if (index != -1)
                    {
                        Chantier.ScrollIntoView(Chantier.SelectedItem);
                    }
                }

                index++;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _owner.OnCloseDialog(this);
        }

        private void OnTimer_500ms(object sender, EventArgs e)
        {
            DayOfBeginningOfTask.Background = new SolidColorBrush((false == DatesExpert.IsDayValid(DayOfBeginningOfTask.Text) ? Colors.Orange : Colors.White));
            DayOfEndOfTask.Background = new SolidColorBrush((false == DatesExpert.IsDayValid(DayOfEndOfTask.Text) ? Colors.Orange : Colors.White));
            HourOfBeginningOfTask.Background = new SolidColorBrush((false == DatesExpert.IsHourValid(HourOfBeginningOfTask.Text) ? Colors.Orange : Colors.White));
            HourOfEndOfTask.Background = new SolidColorBrush((false == DatesExpert.IsHourValid(HourOfEndOfTask.Text) ? Colors.Orange : Colors.White));
        }

        private void OnClickValider(object sender, RoutedEventArgs e)
        {
            if (false == DatesExpert.IsDayValid(DayOfBeginningOfTask.Text, out int dayBeginning, out int monthBeginning, out int yearBeginning) 
                || false == DatesExpert.IsDayValid(DayOfEndOfTask.Text, out int dayEnding, out int monthEnding, out int yearEnding)
                || false == DatesExpert.IsHourValid(HourOfBeginningOfTask.Text, out int hourBeginning, out int minutesBeginning) 
                || false == DatesExpert.IsHourValid(HourOfEndOfTask.Text, out int hourEnding, out int minutesEnding)
                )
            {
                MessageBox.Show("Merci de corriger les éléments colorisés", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DateTime beginsAt = new DateTime(yearBeginning, monthBeginning, dayBeginning, hourBeginning, minutesBeginning, 0);
            DateTime endsAt = new DateTime(yearEnding, monthEnding, dayEnding, hourEnding, minutesEnding, 0);

            if (beginsAt >= endsAt)
            {
                MessageBox.Show("La date de début doit être AVANT la date de fin", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (null == Employee.SelectedItem)
            {
                MessageBox.Show("Vous devez sélectionner un employé avant de valider", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (null == Chantier.SelectedItem)
            {
                MessageBox.Show("Vous devez sélectionner un chantier avant de valider", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _taskcopy._employeeKeyId = ((IEmployee)Employee.SelectedItem).KeyId;          
            _taskcopy._chantierKeyId = ((IChantier)Chantier.SelectedItem).KeyId;
            _taskcopy._beginsAt = beginsAt;
            _taskcopy._endsAt = endsAt;

            if (EMode.MODIFICATION == _mode)
            {
                Model.Instance.ModifyTask(_taskcopy);
            }
            else
            {
                Model.Instance.AddTaskToEmployee(
                    _taskcopy.EmployeeKeyId,
                    _taskcopy.ChantierKeyId,
                    _taskcopy.BeginsAt,
                    _taskcopy.EndsAt
                    );
            }

            this.Close();
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Return)
            {
                OnClickValider(null, null);
            }
        }

        private Color ConvertFromColorDialogBox(System.Drawing.Color couleur)
        {
            var output = new System.Windows.Media.Color()
            {
                R = couleur.R,
                G = couleur.G,
                B = couleur.B,
                A = 255
            };

            return output;
        }

        private System.Drawing.Color ConvertToColorDialogBox(Color couleur)
        {
            var output = System.Drawing.Color.FromArgb(couleur.R, couleur.G, couleur.B);
            
            return output;
        }

        private void OnFilterChantierChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Filter.Text))
            {
                Chantier.Items.Clear();
                for (var i = 0; i < _chantiers.Length; i++)
                {
                    Chantier.Items.Add(_chantiers[i]);
                }
                Chantier.SelectedIndex = -1;
                return;
            }

            var filter = Filter.Text.ToLower();

            var filteredChantiers = new List<IChantier>();
            for(var i = 0; i < _chantiers.Length; i++)
            {
                var chantier = _chantiers[i].ToString().ToLower();
                if (chantier.Contains(filter))
                {
                    filteredChantiers.Add(_chantiers[i]);
                }
            }

            Chantier.Items.Clear();
            for(var i = 0; i < filteredChantiers.Count; i++)
            {
                Chantier.Items.Add(filteredChantiers[i]);
            }

            Chantier.SelectedIndex = (filteredChantiers.Count == 1) ? 0 : -1;
        }
    }
}
