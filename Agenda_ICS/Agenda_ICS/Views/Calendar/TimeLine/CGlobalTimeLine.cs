using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar.TimeLine
{
    class CGlobalTimeLine : Canvas
    {
        // *** PUBLIC ***********************

        public interface ICalendar
        {
            DateTime FirstDay { get; }

            DateTime CenterDay { get; }

            double NbWeekVisibles { get; }

            void OnDateSelectedOnGlobalTimeLine(DateTime dateSelected);

            void OnChangeYearOnGlobalTimeLine();
        }

        public CGlobalTimeLine(ICalendar calendar)
        {
            _calendar = calendar;

            _monthStrings = new string[12]
            {
                "Janv.",
                "Fev.",
                "Mars",
                "Avr.",
                "Mai",
                "Juin",
                "Juil.",
                "Aout",
                "Sept.",
                "Oct.",
                "Nov.",
                "Déc.",
            };

            Background = new SolidColorBrush((Model.Instance.IsTestMode) ? Colors.Red : Colors.BlanchedAlmond);

            Height = Constantes._heightOfGlobalTimeLine;

            Children.Add(_dockPanel = new DockPanel());

            _cursor = new Canvas()
            {
                Height = Constantes._heightOfGlobalTimeLine,
                Background = new SolidColorBrush(Colors.Red),
                Opacity = 0.25
            };
            Children.Add(_cursor);

            _dockPanel.Children.Add(_lastYearButton = new Button()
                    {
                        Content = "<",
                        Width = _widthOfButton,                       
                    }
                );

            _lastYearButton.Click += OnClick_lastYearButton;

            for (var idMonth = 0; idMonth < 12; idMonth++)
            {
                var border = new Border();
                border.BorderBrush = Brushes.Gray;
                border.BorderThickness = new System.Windows.Thickness((idMonth == 0) ? 1 : 0, 1, 1, 0);
                _monthPanels[idMonth] = new Canvas();
                border.Child = _monthPanels[idMonth];

                _monthTitles[idMonth] = new TextBlock()
                {
                    Text = GetMonthTitle(idMonth),
                    TextAlignment = TextAlignment.Center,
                    Padding = new Thickness(0, 3, 0, 0)
                };
                _monthPanels[idMonth].Children.Add(_monthTitles[idMonth]);
                _dockPanel.Children.Add(border);
            }

            DockPanel.SetDock(_dockPanel.Children[0], Dock.Left);

            _dockPanel.Children.Add(_nextYearButton = new Button()
                    {
                        Content = ">",
                        Width = _widthOfButton,
                    }
                );

            _nextYearButton.Click += OnClick_nextYearButton;

            MouseDown += new MouseButtonEventHandler(OnMouseDown);
        }

        public void OnSizeChanged()
        {
            UpdateMonthRectangles();

            UpdateCursor();
        }

        public void OnFirstDayChanged()
        {
            UpdateMonthTitles();

            UpdateCursor();
        }

        public int CurrentYear { get; private set; } = DateTime.Now.Year;

        // *** RESTRICTED ******************

        private DockPanel _dockPanel;

        private string[] _monthStrings = new string[12];

        private int _widthOfButton = 25;

        private Button _lastYearButton;

        private Button _nextYearButton;

        private Canvas _cursor;

        private Canvas[] _monthPanels = new Canvas[12];

        private TextBlock[] _monthTitles = new TextBlock[12];

        private ICalendar _calendar;

        private void OnClick_lastYearButton(object sender, RoutedEventArgs e)
        {
            CurrentYear--;

            UpdateMonthTitles();
            UpdateCursor();
            _calendar.OnChangeYearOnGlobalTimeLine();
        }

        private void OnClick_nextYearButton(object sender, RoutedEventArgs e)
        {
            CurrentYear++;

            UpdateMonthTitles();
            UpdateCursor();
            _calendar.OnChangeYearOnGlobalTimeLine();
        }

        private void UpdateMonthRectangles()
        {
            foreach (var children in _dockPanel.Children)
            {
                if (children is Border border)
                {
                    border.Width = (Width - 2 * _widthOfButton) / 12;
                }
            }

            for (var idMonth = 0; idMonth < 12; idMonth++)
            {
                var canvas = _monthPanels[idMonth];
                canvas.Width = (Width - 2 * _widthOfButton) / 12;
                canvas.Height = Height;

                _monthTitles[idMonth].Width = canvas.Width;
            }
        }

        private double GetXofDateOnTimeLine(DateTime date)
        {
            var monthRectangleLength = (Width - 2 * _widthOfButton) / 12;

            if (date.Year < CurrentYear)
            {
                return _widthOfButton;
            }
            else if (date.Year > CurrentYear)
            {
                return _widthOfButton + 12 * monthRectangleLength;
            }

            var idCurrentMonth = date.Month;
            var idCurrentDay = date.Day;
            var x = _widthOfButton 
                + (idCurrentMonth - 1) * monthRectangleLength 
                + (idCurrentDay / 31.0) * monthRectangleLength;
            x = Math.Max(_widthOfButton, x);
            x = Math.Min(_widthOfButton + 12 * monthRectangleLength, x);
            return x;
        }

        private DateTime GetDateTimefromMouse()
        {
            var xMouse = Mouse.GetPosition(this).X;

            var monthRectangleLength = (Width - 2 * _widthOfButton) / 12;
            var idMonth = 1 + (int)((xMouse - _widthOfButton) / monthRectangleLength);
            var xDay = (xMouse - _widthOfButton - (idMonth - 1)* monthRectangleLength) / monthRectangleLength;
            var idDay = 1 + ((idMonth != 2) ? (int)(29 * xDay) : (int)(27 * xDay));

            return new DateTime(CurrentYear, idMonth, idDay, 0, 0, 0);
        }

        private void UpdateCursor()
        {
            var x_firstDayVisible = GetXofDateOnTimeLine(_calendar.FirstDay);
            var nbDaysVisibles = (int)(_calendar.NbWeekVisibles * 7);
            var x_lastDayVisible = GetXofDateOnTimeLine(_calendar.FirstDay + new TimeSpan(nbDaysVisibles, 0, 0, 0));

            _cursor.Width = x_lastDayVisible - x_firstDayVisible;
            _cursor.RenderTransform = new TranslateTransform
            {
                X = x_firstDayVisible,
                Y = 0
            };
        }

        private void UpdateMonthTitles()
        {
            for (var idMonth = 0; idMonth < 12; idMonth++)
            {
                _monthTitles[idMonth].Text = GetMonthTitle(idMonth);
            }
        }

        private string GetMonthTitle(int idMonth)
        {
            return _monthStrings[idMonth] + " " + CurrentYear;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseDate = GetDateTimefromMouse();
            _calendar.OnDateSelectedOnGlobalTimeLine(mouseDate);
        }
    }
}
