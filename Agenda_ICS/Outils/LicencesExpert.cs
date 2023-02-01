using System;
using System.IO;

namespace NOutils
{
    public struct Licence
    {
        public string nom;
        public string sha;

        public Licence(string nom, string sha)
        {
            this.nom = nom ?? throw new ArgumentNullException(nameof(nom));
            this.sha = sha ?? throw new ArgumentNullException(nameof(sha));
        }
    }

    public class LicencesExpert
    {
        public const int NbJoursDeRetardMax = 30;

        public enum EResult
        {
            LICENCE_OK,
            LICENCE_NOT_FOUND_BUT_OK,
            LICENCE_FAILED
        }

        public static EResult LicenceVerificationResult(string pathToDatasFile, out int nbJoursRestants)
        {
            nbJoursRestants = int.MaxValue;

            var driveLetter = AdministrationDatasExpert.ReadLine(pathToDatasFile, 0).Substring(0, 1);
            var hddDatas = DrivesExpert.GetHddDatas(driveLetter);
            var sha = ShaExpert.ComputeSHA256(hddDatas);
            var today = DatesExpert.FromFrenchDateToStandardDate(DatesExpert.FormatDay(DateTime.Now));

            var licences = WebExpert.ReadLicencesFromWeb_Synchrone();
            if (null != licences)
            {
                foreach (var licence in licences)
                {
                    if (licence.sha == sha)
                    {
                        WriteLastDayWhenLicenceWasOk(today, pathToDatasFile);
                        return EResult.LICENCE_OK;
                    }
                }
            }

            var day = today;
            var shaOfLastDayWhenLicenceWasOk = AdministrationDatasExpert.ReadLine(
                pathToDatasFile, 
                AdministrationDatasExpert.ID_LINE_LASTDATEWHENLICENCEWASOK
                );
            for (var i = 1; i <= NbJoursDeRetardMax; i++)
            {
                day = DatesExpert.GetDayBefore(day);
                var shaOfThisDay = ShaExpert.ComputeSHA256(day);
                if (shaOfLastDayWhenLicenceWasOk == shaOfThisDay)
                {
                    nbJoursRestants = NbJoursDeRetardMax - i;
                    return EResult.LICENCE_NOT_FOUND_BUT_OK;
                }
            }

            return EResult.LICENCE_FAILED;
        }

        private static void WriteLastDayWhenLicenceWasOk(string day, string pathToDatasFile)
        {
            var sha = ShaExpert.ComputeSHA256(day);
            AdministrationDatasExpert.ModifyLine(
                pathToDatasFile, 
                AdministrationDatasExpert.ID_LINE_LASTDATEWHENLICENCEWASOK, 
                sha
                );
        }
    }
}
