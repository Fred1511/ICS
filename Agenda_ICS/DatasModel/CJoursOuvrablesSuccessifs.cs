using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDatasModel
{
    public class CJoursOuvrablesSuccessifs
    {
        public static int _nbJoursOuvrablesParSemaine = 5;

        public static DateTime GetDayAfterXJoursOuvrables(DateTime firstDayOuvrable, int nbJoursOuvrablesAprès)
        {
            if (0 == nbJoursOuvrablesAprès)
            {
                return firstDayOuvrable;
            }
            else if (7 == _nbJoursOuvrablesParSemaine)
            {
                return firstDayOuvrable + new TimeSpan(nbJoursOuvrablesAprès, 0, 0, 0);
            }

            var day = firstDayOuvrable;
            var dayOfWeek = (int)(firstDayOuvrable.DayOfWeek);
            var countJourOuvrables = 0;
            var countJours = 0;
            while (true)
            {
                countJours++;

                dayOfWeek = (dayOfWeek + 1) % 7;
                if (dayOfWeek >= 1 && dayOfWeek <= _nbJoursOuvrablesParSemaine)
                {
                    countJourOuvrables++;
                }

                if (countJourOuvrables == nbJoursOuvrablesAprès)
                {
                    return firstDayOuvrable + new TimeSpan(countJours, 0, 0, 0);
                }
            }
        }

        public static DateTime GetFirstJourOuvrableAfter(DateTime day)
        {
            if (7 == _nbJoursOuvrablesParSemaine)
            {
                return day;
            }

            var dayOfWeek = (int)(day.DayOfWeek);
            if (IsJourOuvrable(dayOfWeek))
            {
                return day;
            }

            var countJours = 0;
            while (true)
            {
                countJours++;

                dayOfWeek = (dayOfWeek + 1) % 7;
                if (dayOfWeek >= 1 && dayOfWeek <= _nbJoursOuvrablesParSemaine)
                {
                    return day + new TimeSpan(countJours, 0, 0, 0);
                }
            }
        }

        public static DateTime GetFirstJourOuvrableBefore(DateTime day)
        {
            if (7 == _nbJoursOuvrablesParSemaine)
            {
                return day;
            }

            var dayOfWeek = (int)(day.DayOfWeek);
            if (IsJourOuvrable(dayOfWeek))
            {
                return day;
            }
                
            var countJours = 0;
            while (true)
            {
                countJours++;

                dayOfWeek--;
                if (dayOfWeek < 0)
                {
                    dayOfWeek += 7;
                }

                if (IsJourOuvrable(dayOfWeek))
                {
                    return day - new TimeSpan(countJours, 0, 0, 0);
                }
            }
        }

        private static bool IsJourOuvrable(int dayOfWeek)
        {
            return dayOfWeek >= 1 && dayOfWeek <= _nbJoursOuvrablesParSemaine;
        }

        public static int GetNbJoursOuvrableEntre(DateTime firstDayOuvrable, DateTime lastDayOuvrable)
        {
            var nbDays = (lastDayOuvrable - firstDayOuvrable).Days;
            if (nbDays < 2)
            {
                return nbDays;
            }

            var dayOfWeek = (int)(firstDayOuvrable.DayOfWeek);
            var countJourOuvrables = nbDays;
            for (var i = 1; i < nbDays; i++)
            {
                dayOfWeek = (dayOfWeek + 1) % 7;

                if (dayOfWeek < 1 || dayOfWeek > _nbJoursOuvrablesParSemaine)
                {
                    countJourOuvrables--;
                }
            }

            return countJourOuvrables;
        }
    }
}
