using Agenda_ICS.Views.Calendar;
using NDatasModel;
using System;
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

            DayOfBeginningOfTask.Text = FormatDay(_taskcopy.BeginsAt);
            HourOfBeginningOfTask.Text = FormatHour(_taskcopy.BeginsAt);
            DayOfEndOfTask.Text = FormatDay(_taskcopy.EndsAt);
            HourOfEndOfTask.Text = FormatHour(_taskcopy.EndsAt);
        }

        ~TaskEditorDialog()
        {
            if (EMode.MODIFICATION == _mode)
            {
                Model.Instance.InformThatCopyIsNoLongerNecessary(_taskcopy.KeyId);
            }
        }

        // *** RESTRICTED ***********************

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
            var chantiers = Model.Instance.GetChantiers();
            foreach (var chantier in chantiers)
            {
                Chantier.Items.Add(chantier);
                if (chantier.KeyId == _taskcopy._chantierKeyId)
                {
                    Chantier.SelectedIndex = index;
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
            DayOfBeginningOfTask.Background = new SolidColorBrush((false == IsDayValid(DayOfBeginningOfTask.Text) ? Colors.Orange : Colors.White));
            DayOfEndOfTask.Background = new SolidColorBrush((false == IsDayValid(DayOfEndOfTask.Text) ? Colors.Orange : Colors.White));
            HourOfBeginningOfTask.Background = new SolidColorBrush((false == IsHourValid(HourOfBeginningOfTask.Text) ? Colors.Orange : Colors.White));
            HourOfEndOfTask.Background = new SolidColorBrush((false == IsHourValid(HourOfEndOfTask.Text) ? Colors.Orange : Colors.White));
        }

        private void OnClickValider(object sender, RoutedEventArgs e)
        {
            if (false == IsDayValid(DayOfBeginningOfTask.Text, out int dayBeginning, out int monthBeginning, out int yearBeginning) 
                || false == IsDayValid(DayOfEndOfTask.Text, out int dayEnding, out int monthEnding, out int yearEnding)
                || false == IsHourValid(HourOfBeginningOfTask.Text, out int hourBeginning, out int minutesBeginning) 
                || false == IsHourValid(HourOfEndOfTask.Text, out int hourEnding, out int minutesEnding)
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

        private string FormatDay(DateTime date)
        {
            return $"{date.Day:00}/{date.Month:00}/{date.Year}";
        }

        private string FormatHour(DateTime date)
        {
            return $"{date.Hour:00}:{date.Minute:00}";
        }

        private bool IsDayValid(string dateAsString)
        {
            return IsDayValid(dateAsString, out int day, out int month, out int year);
        }

        private bool IsDayValid(string dateAsString, out int day, out int month, out int year)
        {
            day = 0;
            month = 0;
            year = 0;

            try
            {
                // Récupération des éléments jour, mois, année sous forme de texte
                var components = dateAsString.Split('/');
                if (components.Length < 2 || components.Length > 3)
                {
                    return false;
                }
                var dayAsString = components[0];
                var monthAsString = components[1];
                var yearAsString = DateTime.Now.Year.ToString();
                if (components.Length == 3)
                {
                    yearAsString = components[2];
                }

                // Transformation des éléments jour, mois, année sous forme d'entiers
                if (false == int.TryParse(dayAsString, out day))
                {
                    return false;
                }
                if (false == int.TryParse(monthAsString, out month))
                {
                    return false;
                }
                if (false == int.TryParse(yearAsString, out year))
                {
                    return false;
                }

                var date = new DateTime(year, month, day);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool IsHourValid(string timeAsString)
        {
            return IsHourValid(timeAsString, out int hour, out int minutes);
        }

        private bool IsHourValid(string timeAsString, out int hour, out int minutes)
        {
            hour = 0;
            minutes = 0;

            try
            {
                // Récupération des éléments jour, mois, année sous forme de texte
                var components = timeAsString.Split(':');
                if (components.Length != 2)
                {
                    return false;
                }
                var hourAsString = components[0];
                var minutesAsString = components[1];

                // Transformation des éléments jour, mois, année sous forme d'entiers
                if (false == int.TryParse(hourAsString, out hour))
                {
                    return false;
                }
                if (false == int.TryParse(minutesAsString, out minutes))
                {
                    return false;
                }

                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minutes, 0);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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
            var filter = Filter.Text.ToLower();

            var nbMatch = 0;
            var idMatch = -1;
            for(var i = 0; i < Chantier.Items.Count; i++)
            {
                var chantier = Chantier.Items[i].ToString().ToLower();
                if (chantier.Contains(filter))
                {
                    idMatch = i;
                    nbMatch++;
                }
            }

            if (0 == nbMatch || nbMatch > 1)
                Chantier.SelectedIndex = -1;
            else
                Chantier.SelectedIndex = idMatch;
        }
    }
}
