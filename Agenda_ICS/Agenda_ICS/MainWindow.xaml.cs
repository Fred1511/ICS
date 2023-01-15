using Agenda_ICS.Views.Calendar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Agenda_ICS
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var model = new Model();

            InitializeComponent();

            Calendar.ClipToBounds = true;
            Calendar.Children.Add(_employeesNamesGrid = new StackPanel());
            Calendar.Children.Add(_calendarDisplayer = new CalendarDisplayer());
            DockPanel.SetDock(_employeesNamesGrid, Dock.Left);

            ConfigureEmployeesNameGrid();

            PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewer_PreviewMouseWheel);
            _employeesNamesGrid.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewer_PreviewMouseWheel);
        }

        StackPanel _caseVideOfTimeLine;
        StackPanel _employeesNamesGrid;
        CalendarDisplayer _calendarDisplayer; 

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _calendarDisplayer.Width = Calendar.ActualWidth - Constantes._widthOfNameOfEmployeeLabel;
            _calendarDisplayer.Height = Calendar.ActualHeight;
            _calendarDisplayer.OnWindowSizeChanged();

            _employeesNamesGrid.Height = ActualHeight;
            var heightOfCalendarOfEmployees = _calendarDisplayer.GetHeightOfCalendarOfEmployees();
            for(var i = 1; i < _employeesNamesGrid.Children.Count; i++)
            {
                // i = 0 correspond à _caseVideOfTimeLine
                ((Label)_employeesNamesGrid.Children[i]).Height = heightOfCalendarOfEmployees;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = false;
        }

        private void ConfigureEmployeesNameGrid()
        {
            _employeesNamesGrid.Orientation = Orientation.Vertical;

            _caseVideOfTimeLine = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical,
                Height = Constantes._heightOfTimeLine + Constantes._heightOfGlobalTimeLine + Constantes._separatorUnderGlobalTimelineThickness,
                Width = Constantes._widthOfNameOfEmployeeLabel,
            };

            //var btnPanel = new StackPanel()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Orientation = Orientation.Vertical,
            //    Height = Constantes._heightOfTimeLine + Constantes._heightOfGlobalTimeLine + Constantes._separatorUnderGlobalTimelineThickness,
            //    Width = Constantes._widthOfNameOfEmployeeLabel,
            //};

            var configurationButton = new Button()
            {
                Content = "Configuration",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5),
            };
            configurationButton.Click += OnClick_Configuration;
            _caseVideOfTimeLine.Children.Add(configurationButton);

            var displaySynthèseButton = new Button()
            {
                Content = "Synthèse",
                Width = 100,
                Height = 30,
                Margin = new Thickness(5, 0, 5, 5),
            };
            displaySynthèseButton.Click += OnClick_DisplaySynthèse;
            _caseVideOfTimeLine.Children.Add(displaySynthèseButton);

            DockPanel.SetDock(_caseVideOfTimeLine, Dock.Top);
            UpdateEmployeesNamesGrid();
        }

        private void UpdateEmployeesNamesGrid()
        {
            var employees = Model.Instance.GetEmployees();

            _employeesNamesGrid.Children.Clear();
            _employeesNamesGrid.Children.Add(_caseVideOfTimeLine);
            foreach (var employee in employees)
            {
                var employeeNameLabel = new Label()
                {
                    Content = employee.Name,
                    Width = Constantes._widthOfNameOfEmployeeLabel,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                };
                _employeesNamesGrid.Children.Add(employeeNameLabel);
            }
        }

        private void OnClick_Configuration(object sender, RoutedEventArgs e)
        {
            (new Views.Configuration.ConfigurationWnd()).ShowDialog();
            UpdateEmployeesNamesGrid();
            Window_SizeChanged(null, null);
        }

        private void OnClick_DisplaySynthèse(object sender, RoutedEventArgs e)
        {
            (new Views.Synthèse.SynthèseDlg()).ShowDialog();
        }
    }
}
