using Agenda_ICS.Views.Calendar.TimeLine;
using NDatasModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Agenda_ICS.Views.Calendar
{
    public interface ICalendarDisplayerForChildrens
    {
        void ResetDisplayOfCalendar();
    }

    class CalendarDisplayer : ScrollViewer, 
        ICalendarDisplayerForChildrens, 
        CGlobalTimeLine.ICalendar,
        DayTimeLineDisplayer.ICalendar
    {
        // *** PUBLIC *********************

        public CalendarDisplayer()
        {
            FirstDay = CJoursOuvrablesSuccessifs.GetFirstJourOuvrableBefore(FirstDay);

            _stackPanel = new StackPanel();
            _stackPanel.Orientation = Orientation.Vertical;
            Content = _stackPanel;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            CreateGlobalTimeLine();
            CreateTimeLines(FirstDay);
            CreateCalendarsGrid(FirstDay);

            PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewer_PreviewMouseWheel);
            Loaded += new RoutedEventHandler(OnLoad);

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_1s;
            timer.Start();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_100ms;
            timer.Start();
        }

        public void OnLoad(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        public double GetHeightOfCalendarOfEmployees()
        {
            return ((CalendarOfEmployee)_calendarsGrid.Children[0]).Height;
        }

        public void OnWindowSizeChanged()
        {
            var calendarWidth = Width;

            _timeLine.Width = calendarWidth;
            _timeLine.Height = Constantes._heightOfTimeLine; 
            _timeLine.OnSizeChanged(WidthOfWeek);

            _globalTimeLine.Width = calendarWidth;
            _globalTimeLine.OnSizeChanged();

            _separatorUnderGlobalTimeLine.Width = calendarWidth;

            _calendarsGrid.Width = calendarWidth;
            UpdateSizeOfCalendarsGrid(_calendarsGrid.Width, Height 
                - Constantes._heightOfTimeLine 
                - Constantes._heightOfGlobalTimeLine
                - Constantes._separatorUnderGlobalTimelineThickness
                );
            UpdateDisplay();
        }

        public void ResetDisplayOfCalendar()
        {
            UpdateDisplay();
        }

        public double WidthOfWeek { get; private set; } = 1536 / 2;

        public DateTime FirstDay { get; private set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        public DateTime CenterDay { get; private set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

        public double NbWeekVisibles { get; private set; } = 2;

        public void OnDateSelectedOnGlobalTimeLine(DateTime dateSelected)
        {
            FirstDay = CJoursOuvrablesSuccessifs.GetFirstJourOuvrableAfter(dateSelected - new TimeSpan((int)(NbWeekVisibles * 7 / 2), 0, 0, 0));

            OnFirstDayChanged();
        }

        public bool IsNeedToShowYear()
        {
            return (_globalTimeLine.CurrentYear != CenterDay.Year);
        }

        public void OnChangeYearOnGlobalTimeLine()
        {
            _timeLine.UpdateDisplay();
        }

        public void OnStatutFériéOfADayChanged()
        {
            ResetDisplayOfCalendar();
        }

        // *** RESTRICTED *******************

        private UniformGrid _calendarsGrid;

        private StackPanel _stackPanel;

        private CalendarTimeLineDisplayer _timeLine;

        private CGlobalTimeLine _globalTimeLine;

        private Canvas _separatorUnderGlobalTimeLine;

        private double WidthMaxByWeek => Width / Constantes._nbWeeksMinDisplayable;

        private double WidthMinByWeek => Width / Constantes._nbWeeksMaxDisplayable;
        
        private void UpdateDisplay()
        {
            var joursFeries = Model.Instance.GetJoursFériés();
            foreach (var children in _calendarsGrid.Children)
            {
                ((CalendarOfEmployee)children).UpdateListeOfJoursFériés(joursFeries);
                ((CalendarOfEmployee)children).UpdateVisual();
            }
        }

        private void CreateCalendarsGrid(DateTime firstDay)
        {
            var nbWeeks = Constantes._nbWeeksMaxDisplayable;

            _calendarsGrid = new UniformGrid();
            var employees = Model.Instance.GetEmployees();
            _calendarsGrid.Rows = employees.Length;
            _calendarsGrid.Columns = 1;
            _calendarsGrid.HorizontalAlignment = HorizontalAlignment.Left;
            var idRow = 0;
            foreach (var employee in employees)
            {
                var CalendarOfEmployee = new CalendarOfEmployee(this, employee.KeyId, firstDay, nbWeeks);
                Grid.SetRow(CalendarOfEmployee, idRow++);
                _calendarsGrid.Children.Add(CalendarOfEmployee);
            }

            _stackPanel.Children.Add(_calendarsGrid);
        }

        private void CreateTimeLines(DateTime firstDay)
        {
            var nbWeeks = Constantes._nbWeeksMaxDisplayable;

            _timeLine = new TimeLine.CalendarTimeLineDisplayer(this, firstDay, nbWeeks)
            {
                Height = Constantes._heightOfTimeLine,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            _stackPanel.Children.Add(_timeLine);
        }

        private void CreateGlobalTimeLine()
        {
            _globalTimeLine = new TimeLine.CGlobalTimeLine(this);
            _globalTimeLine.Width = Width;
            _globalTimeLine.HorizontalAlignment = HorizontalAlignment.Left;
            _stackPanel.Children.Add(_globalTimeLine);

            _separatorUnderGlobalTimeLine = new Canvas();
            _separatorUnderGlobalTimeLine.Width = Width;
            _separatorUnderGlobalTimeLine.Height = Constantes._separatorUnderGlobalTimelineThickness;
            _separatorUnderGlobalTimeLine.HorizontalAlignment = HorizontalAlignment.Left;
            _separatorUnderGlobalTimeLine.Background = new SolidColorBrush(Colors.Black);
            _stackPanel.Children.Add(_separatorUnderGlobalTimeLine);
        }

        private void UpdateSizeOfCalendarsGrid(double width, double height)
        {
            foreach (var children in _calendarsGrid.Children)
            {
                var calendarOfEmployee = (CalendarOfEmployee)children;
                calendarOfEmployee.Width = width;
                calendarOfEmployee.Height = height / _calendarsGrid.Children.Count;
                calendarOfEmployee.OnSizeChanged(WidthOfWeek);
            }
        }

        private void OnFirstDayChanged()
        {
            foreach (var children in _calendarsGrid.Children)
            {
                var calendarOfEmployee = (CalendarOfEmployee)children;
                calendarOfEmployee.OnFirstDayChanged(FirstDay);
            }

            _timeLine.OnFirstDayChanged(FirstDay);
            _globalTimeLine.OnFirstDayChanged();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;
                WidthOfWeek += e.Delta;
                if (WidthOfWeek < WidthMinByWeek)
                {
                    WidthOfWeek = WidthMinByWeek;
                }
                else if (WidthOfWeek > WidthMaxByWeek)
                {
                    WidthOfWeek = WidthMaxByWeek;
                }

                NbWeekVisibles = Width / WidthOfWeek;

                OnWindowSizeChanged();
                UpdateDisplay();
            }
            else
            {
                if (e.Delta > 0)
                {
                    FirstDay = CJoursOuvrablesSuccessifs.GetFirstJourOuvrableAfter(FirstDay + new TimeSpan(1, 0, 0, 0, 0));
                }
                else if (e.Delta < 0)
                {
                    FirstDay = CJoursOuvrablesSuccessifs.GetFirstJourOuvrableBefore(FirstDay - new TimeSpan(1, 0, 0, 0, 0));
                }

                OnFirstDayChanged();
            }

            CenterDay = FirstDay + new TimeSpan((int)(NbWeekVisibles * 7 / 2), 0, 0, 0, 0);

            ScrollToVerticalOffset(0);
        }

        private void Timer_1s(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void Timer_100ms(object sender, EventArgs e)
        {
            Model.Instance.OnTimer_100ms();
        }
    }
}
