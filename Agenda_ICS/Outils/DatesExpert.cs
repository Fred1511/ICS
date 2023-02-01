using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOutils
{
    public class DatesExpert
    {
        public static string FormatDay(DateTime date)
        {
            return $"{date.Day:00}/{date.Month:00}/{date.Year}";
        }

        public static string FormatHour(DateTime date)
        {
            return $"{date.Hour:00}:{date.Minute:00}";
        }

        public static bool IsDayValid(string dateAsString/*Date sous la forme 15/01/2023*/)
        {
            return IsDayValid(dateAsString, out int day, out int month, out int year);
        }

        public static bool IsDayValid(string dateAsString/*Date sous la forme 15/01/2023*/, out int day, out int month, out int year)
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

        public static bool IsHourValid(string timeAsString)
        {
            return IsHourValid(timeAsString, out int hour, out int minutes);
        }

        public static bool IsHourValid(string timeAsString, out int hour, out int minutes)
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

        /// <summary>
        /// Retourne une date sous la forme 20230115
        /// </summary>
        /// <param name="standardDate">Date sous la forme 15/01/2023</param>
        /// <returns></returns>
        public static string FromFrenchDateToStandardDate(string standardDate)
        {
            var components = standardDate.Split('/');
            int année;
            if (2 == components[2].Length)
            {
                année = (2000 + int.Parse(components[2]));
            }
            else
            {
                année = int.Parse(components[2]);
            }

            int mois = int.Parse(components[1]);
            int jour = int.Parse(components[0]);

            return $"{année}{mois:00}{jour:00}";
        }

        public static DateTime GetFirstTimeOfTheDay(string dayAsString/*ex: 20230115 pour le 15/01/2023*/)
        {
            var year = int.Parse(dayAsString.Substring(0, 4));
            var month = int.Parse(dayAsString.Substring(4, 2));
            var day = int.Parse(dayAsString.Substring(6, 2));
            return new DateTime(year, month, day, 8, 0, 0);
        }

        public static DateTime GetLastTimeOfTheDay(string dayAsString/*ex: 20230115 pour le 15/01/2023*/)
        {
            var year = int.Parse(dayAsString.Substring(0, 4));
            var month = int.Parse(dayAsString.Substring(4, 2));
            var day = int.Parse(dayAsString.Substring(6, 2));
            return new DateTime(year, month, day, 18, 0, 0);
        }

        public static string GetNextDay(string dayAsString/*ex: 20230115 pour le 15/01/2023*/)
        {
            var day = GetFirstTimeOfTheDay(dayAsString);
            var newDay = day + new TimeSpan(1, 0, 0, 0);
            var newDayAsString = GetDayOfDateAsString(newDay);
            return newDayAsString;
        }

        public static string GetDayBefore(string dayAsString/*ex: 20230115 pour le 15/01/2023*/)
        {
            var day = GetFirstTimeOfTheDay(dayAsString);
            var newDay = day - new TimeSpan(1, 0, 0, 0);
            var newDayAsString = GetDayOfDateAsString(newDay);
            return newDayAsString;
        }

        public static string GetDayOfDateAsString(DateTime dateTime)
        {
            return dateTime.Year.ToString() + $"{dateTime.Month:00}" + $"{dateTime.Day:00}";
        }
    }
}
