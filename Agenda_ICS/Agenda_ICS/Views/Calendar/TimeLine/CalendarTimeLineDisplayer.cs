using NDatasModel;
using NOutils;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Agenda_ICS.Views.Calendar.TimeLine
{
    class CalendarTimeLineDisplayer : StackPanel
    {
        // *** PUBLIC ***********************

        public CalendarTimeLineDisplayer(DayTimeLineDisplayer.ICalendar calendar, DateTime firstDay, int nbWeeks)
        {
            Background = new SolidColorBrush(Colors.LightPink);

            var nbDays = nbWeeks * CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine;
            for (var idDay = 0; idDay < nbDays; idDay++)
            {
                var date = CJoursOuvrablesSuccessifs.GetDayAfterXJoursOuvrables(firstDay, idDay);

                var day = new DayTimeLineDisplayer(calendar, date);

                var border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new System.Windows.Thickness(0, 0, 1, 1);
                border.Child = day;

                Children.Add(border);
            }

            Orientation = Orientation.Horizontal;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_1s;
            timer.Start();
        }

        public void OnSizeChanged(double widthOfWeek)
        {
            foreach (Border border in Children)
            {
                border.Width = widthOfWeek / CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine;
                border.Height = Height;

                var day = (DayTimeLineDisplayer)(border.Child);
                day.Width = border.Width;
                day.Height = border.Height;

                day.OnSizeChanged();
            }
        }

        public void UpdateDisplay()
        {
            var joursFeries = Model.Instance.GetJoursFériés();
            foreach (Border border in Children)
            {
                var day = (DayTimeLineDisplayer)(border.Child);
                day.UpdateListeOfJoursFériés(joursFeries);
                day.UpdateDisplay();
            }
        }

        public void OnFirstDayChanged(DateTime firstDay)
        {
            var i = 0;
            foreach (Border border in Children)
            {
                var day = (DayTimeLineDisplayer)(border.Child);
                day.OnDayChanged(CJoursOuvrablesSuccessifs.GetDayAfterXJoursOuvrables(firstDay, i));

                i++;
            }
        }

        // *** RESTRICTED ******************

        private void Timer_1s(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
    }
}