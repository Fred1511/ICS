using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar
{
    class TimeElementOfEmployee : Panel
    {
        // *** PUBLIC *************************

        public TimeElementOfEmployee(
            long employeeKeyId, 
            DateTime dateTime
            )
        {
            _employeeKeyId = employeeKeyId;
            BeginsAt = dateTime;

            ClipToBounds = true;
        }

        public bool IsSelected 
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                UpdateColor();
            }
        }

        public void OnMomentChanged(DateTime dateTime)
        {
            BeginsAt = dateTime;
        }

        /// <summary>
        ///  Le moment temporel de début de cet élément
        /// </summary>
        public DateTime BeginsAt { get; private set; }

        public DateTime EndsAt => BeginsAt + new TimeSpan(Constantes._timeElementDuration_h, 0, 0);

        public void OnOverringByMouseBegins()
        {
            _isMouseOver = true;
            UpdateColor();
        }

        public void OnOverringByMouseEnds()
        {
            _isMouseOver = false;
            UpdateColor();
        }

        public void SetBackgroundColor(Color bkColor)
        {
            Background = new SolidColorBrush(bkColor);
            _bkColor = bkColor;
        }

        // *** RESTRICTED *********************

        bool _isSelected;

        bool _isMouseOver;

        long _employeeKeyId;

        Color _bkColor;

        void UpdateColor()
        {
            if (_isSelected)
            {
                Background = new SolidColorBrush(Constantes._colorOfTimeElementSelected);
            }
            else if (_isMouseOver)
            {
                Background = new SolidColorBrush(Constantes._colorOfTimeElementOverringByMouse);
            }
            else
            {
                Background = new SolidColorBrush(_bkColor);
            }
        }
    }
}
