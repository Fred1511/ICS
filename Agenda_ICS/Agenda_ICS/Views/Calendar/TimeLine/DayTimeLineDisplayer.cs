using NDatasModel;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar.TimeLine
{
    class DayTimeLineDisplayer : UniformGrid, CContextMenuOnTimeLineController.IParent
    {
        // *** PUBLIC ***************************

        public interface ICalendar
        {
            bool IsNeedToShowYear();

            void OnStatutFériéOfADayChanged();
        }

        public DayTimeLineDisplayer(ICalendar calendar, DateTime date)
        {
            _calendar = calendar;

            this.Rows = 2;
            this.Columns = 1;
            ClipToBounds = true;

            Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

            _dayTitle = new Label
            {
                Content = GetDayContent(),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                Background = _standardTitleBrush,
            };
            Children.Add(_dayTitle);
            Grid.SetRow(_dayTitle, 0);

            _hoursLine = new Canvas() // new Grid()
            {
                Background = _standardHoursLineBrush,
            };
            Children.Add(_hoursLine);
            Grid.SetRow(_hoursLine, 1);

            // Les heures
            for (var i = 0; i <= Constantes._timeElementsPerDay; i++)
            {
                var hours = Constantes._firstHourOfTheDay + i * Constantes._timeElementDuration_h;
                _hoursLine.Children.Add
                    (
                        new TextBlock()
                        {
                            Text = hours.ToString(),
                            TextAlignment = System.Windows.TextAlignment.Center,
                            FontSize = 12
                        }
                    );
            }

            MouseUp += new MouseButtonEventHandler(OnMouseUp);

            UpdateIsJourFériéStatut();
        }

        private string GetDayContent()
        {
            if (Width < 120)
            {
                if (_calendar.IsNeedToShowYear())
                {
                    return TranslateDayOfWeek(Date.DayOfWeek).Substring(0, 3) + " - " + Date.ToString("dd/MM/yy");
                }
                else
                {
                    return TranslateDayOfWeek(Date.DayOfWeek).Substring(0, 3) + " - " + Date.ToString("dd/MM");
                }
            }
            else
            {
                return TranslateDayOfWeek(Date.DayOfWeek) + " - " + Date.ToString("dd/MM");
            }
        }

        public void OnDayChanged(DateTime date)
        {
            Date = date;
            UpdateIsJourFériéStatut();
            UpdateDisplay();
        }

        public DateTime Date { get; private set; }

        public void OnSizeChanged()
        {
            if (Width == _lastWidth && Height == _lastHeight)
            {
                return;
            }

            UpdateDisplay();

            _lastWidth = Width;
            _lastHeight = Height;
        }

        public void UpdateDisplay()
        {
            if (_isJourFérié)
            {
                _hoursLine.Background = Constantes._jourFériéBrush;
            }
            else
            {
                _hoursLine.Background = _standardHoursLineBrush;
            }

            if (_isJourFérié)
            {
                _dayTitle.Background = Constantes._jourFériéBrush;
                _dayTitle.Foreground = Brushes.Black;
            }
            else if (Date.Year == DateTime.Now.Year && Date.Month == DateTime.Now.Month && Date.Day == DateTime.Now.Day)
            {
                _dayTitle.Background = Brushes.Red;
                _dayTitle.Foreground = Brushes.White;
            }
            else
            {
                _dayTitle.Background = Brushes.LightBlue;
                _dayTitle.Foreground = Brushes.Black;
            }

            _dayTitle.Content = GetDayContent();
            _dayTitle.FontSize = (Width > 80) ? 12 : 10;
            if (_calendar.IsNeedToShowYear())
            {
                _dayTitle.FontSize -= 2;
            }

            UpdateSizeOfDayTitle();

            UpdateSizeOfHoursLine();
        }

        public void OnStatutFériéChanged()
        {
            _calendar.OnStatutFériéOfADayChanged();
        }

        public void UpdateListeOfJoursFériés(IJourFérié[] joursFériés)
        {
            _joursFériés = joursFériés;
        }

        // *** RESTRICTED ***********************

        private bool _isJourFérié;

        private IJourFérié[] _joursFériés = new IJourFérié[0];

        private ICalendar _calendar;

        private double _lastWidth;

        private double _lastHeight;

        private SolidColorBrush _standardTitleBrush = Brushes.LightBlue;

        private SolidColorBrush _standardHoursLineBrush = Brushes.LightGreen;

        private void UpdateIsJourFériéStatut()
        {
            foreach (var jourFérié in _joursFériés)
            {
                if (jourFérié.Jour.DayOfYear == Date.DayOfYear)
                {
                    _isJourFérié = true;
                    return;
                }
            }

            _isJourFérié = false;
        }

        private void UpdateSizeOfDayTitle()
        {
            _dayTitle.Width = Width;
            _dayTitle.Height = Height / 2;
        }

        private void UpdateSizeOfHoursLine()
        {
            var L_TimeElement = Width / (2 * Constantes._proportionMargin + Constantes._timeElementsPerDay);
            var L_margin = Constantes._proportionMargin * L_TimeElement;
            var L_textBlock = L_TimeElement;
            _hoursLine.Width = Width;
            _hoursLine.Height = Height / 2;
            int i = 0;
            foreach (var children in _hoursLine.Children)
            {
                var textBlock = (TextBlock)children;
                textBlock.Width = L_textBlock;
                var x = L_margin + i * L_TimeElement - L_textBlock / 2;
                var y = 5;
                textBlock.RenderTransform = new TranslateTransform(x, y);
                textBlock.FontSize = (Width > 80) ? 12 : 9;

                i++;
            }
        }

        private Label _dayTitle;

        private Canvas _hoursLine;

        private string TranslateDayOfWeek(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return "DIMANCHE";
                case DayOfWeek.Monday:
                    return "LUNDI";
                case DayOfWeek.Tuesday:
                    return "MARDI";
                case DayOfWeek.Wednesday:
                    return "MERCREDI";
                case DayOfWeek.Thursday:
                    return "JEUDI";
                case DayOfWeek.Friday:
                    return "VENDREDI";
                case DayOfWeek.Saturday:
                    return "SAMEDI";
            }

            throw new System.NotImplementedException();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                new CContextMenuOnTimeLineController(this).ShowContextMenuOnDate(Date);
                return;
            }
        }
    }
}
