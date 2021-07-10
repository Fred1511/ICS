using NDatasModel;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar
{
    class DayOfEmployee : StackPanel
    {
        // *** PUBLIC ***************************

        public DayOfEmployee(long employeeKeyId, DateTime date)
        {
            Orientation = Orientation.Horizontal;

            _employeeKeyId = employeeKeyId;
            Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

            ClipToBounds = true;

            Children.Add(new Label()); // Marge de gauche

            _timeElements = new TimeElementOfEmployee[Constantes._timeElementsPerDay];
            for (var i = 0; i < Constantes._timeElementsPerDay; i++)
            {
                var moment = CalculateMomentOfElement(date, i);
                _timeElements[i] = new TimeElementOfEmployee(employeeKeyId, moment);

                var border = new Border();
                border.BorderBrush = Brushes.Gray;
                border.BorderThickness = new System.Windows.Thickness((i == 0) ? 1 : 0, 1, 1, 0);
                border.Child = _timeElements[i];
                Children.Add(border);
                Grid.SetColumn(border, i);
            }

            Children.Add(new Label()); // Marge de droite
        }

        public double L_TimeElement { get; private set; }

        public double L_margin { get; private set; }

        public void OnSizeChanged()
        {
            if (Width == _lastWidth && Height == _lastHeight)
            {
                return;
            }

            L_TimeElement = Width / (2 * Constantes._proportionMargin + Constantes._timeElementsPerDay);
            L_margin = Constantes._proportionMargin * L_TimeElement;

            foreach (var child in Children)
            {
                if (child is Border border) 
                {
                    border.Width = L_TimeElement;
                    border.Height = Height;
                    var timeElement = (TimeElementOfEmployee)(border.Child);
                    timeElement.Width = border.Width;
                    timeElement.Height = Height;
                }
                else
                {
                    ((Label)child).Width = L_margin;
                }
            }

            _lastWidth = Width;
            _lastHeight = Height;
        }

        public void OnDateChanged(DateTime date)
        {
            Date = date;
            UpdateIsJourFérié();

            for (var i = 0; i < _timeElements.Length; i++)
            {
                _timeElements[i].OnMomentChanged(CalculateMomentOfElement(Date, i));
            }
        }

        public void UpdateDisplay()
        {

        }

        public DateTime Date { get; private set; }

        public DateTime BeginningOfDay => _timeElements[0].BeginsAt;

        public DateTime EndOfDay => _timeElements[_timeElements.Length - 1].BeginsAt + new TimeSpan(Constantes._timeElementDuration_h, 0, 0);

        public int GetIndexOfHourOfDate(DateTime date)
        {
            for (var i = 0; i < _timeElements.Length; i++)
            {
                if (_timeElements[i].BeginsAt <= date && _timeElements[i].BeginsAt + new TimeSpan(Constantes._timeElementDuration_h, 0, 0) > date)
                {
                    return i;
                }
            }

            throw new System.ArgumentException();
        }

        public TimeElementOfEmployee GetTimeElementFromDate(DateTime date)
        {
            for (var i = 0; i < _timeElements.Length; i++)
            {
                if (_timeElements[i].BeginsAt <= date && 
                    _timeElements[i].BeginsAt + new TimeSpan(Constantes._timeElementDuration_h, 0, 0) > date)
                {
                    return _timeElements[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Fournir la liste des éléments qui sont dans la période définie par les limites en argument.
        /// </summary>
        public TimeElementOfEmployee[] ArrayOfElementsBetweenTwoDateTimes(DateTime a, DateTime b)
        {
            var list = new List<TimeElementOfEmployee>();

            var limiteInf = a;
            var limiteSup = b;
            if (b < a)
            {
                limiteInf = b;
                limiteSup = a;
            }

            if (limiteSup < BeginningOfDay || limiteInf > EndOfDay)
            {
                return list.ToArray(); // liste vide
            }

            for (var i = 0; i < _timeElements.Length; i++)
            {
                if (_timeElements[i].EndsAt >= limiteInf &&
                    _timeElements[i].BeginsAt <= limiteSup)
                {
                    list.Add(_timeElements[i]);
                }
            }

            return list.ToArray();
        }

        public void SetBackgroundColor(Color bkColor)
        {
            if (_isJourFérié)
            {
                bkColor = Constantes._jourFériéColor;
            }

            foreach (var timeElement in _timeElements)
            {
                timeElement.SetBackgroundColor(bkColor);
            }
        }

        public void UpdateListeOfJoursFériés(IJourFérié[] joursFériés)
        {
            var oldIsJourFérié = _isJourFérié;

            _joursFériés = joursFériés;
            UpdateIsJourFérié();

            if (oldIsJourFérié != _isJourFérié)
            {
                UpdateDisplay();
            }
        }

        // *** RESTRICTED ***********************

        private bool _isJourFérié;

        private IJourFérié[] _joursFériés;

        private double _lastWidth;

        private double _lastHeight;

        TimeElementOfEmployee[] _timeElements;

        long _employeeKeyId;

        private static DateTime CalculateMomentOfElement(DateTime date, int i)
        {
            var hour = Constantes._firstHourOfTheDay + i * Constantes._timeElementDuration_h;
            var moment = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
            return moment;
        }

        private void UpdateIsJourFérié()
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
    }
}
