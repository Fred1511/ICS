using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar
{
    class Constantes
    {
        public static Color _colorOfTimeElementOverringByMouse = Colors.Aquamarine;

        public static Color _colorOfTimeElementSelected = Colors.Orange;

        public static Color _colorOfTimeElementWithTask = Colors.Red;

        public static Color _backgroundColorOfDayOfEvenWeek = Colors.Azure;

        public static Color _backgroundColorOfDayOfOddWeek = Colors.Aqua;

        public static Brush _jourFériéBrush = Brushes.LightGray;

        public static Color _jourFériéColor = Colors.LightGray;

        public static int _firstHourOfTheDay = 8;

        public static int _timeElementDuration_h = 2;

        public static int _timeElementsPerDay = 5;

        public static int _widthOfNameOfEmployeeLabel = 120;

        public static int _heightOfTimeLine = 50;

        public static int _heightOfGlobalTimeLine = 25;

        public static double _proportionMargin = 0.5;

        public static int _nbWeeksMaxDisplayable = 3;

        public static int _nbWeeksMinDisplayable = 1;

        public static double _separatorUnderGlobalTimelineThickness = 5;
    }
}
