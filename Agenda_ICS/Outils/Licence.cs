using System;

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

    public class LicencesManager
    {
        public static bool LicenceVerificationResult(string pathToDatasFile)
        {
            var sha = DrivesExpert.
        }
    }
}
