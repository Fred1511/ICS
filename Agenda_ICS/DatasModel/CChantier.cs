using System;

namespace NDatasModel
{
    public enum EStatutChantier
    {
        A_CONFIRMER = 0,
        CONFIRMé = 1,
        CLOS = 2
    }

    public class CChantier : IChantier
    {
        // *** INTERFACE **********************

        public long KeyId => _keyId;

        public string Name => _name;

        public string RefDevis => _refDevis;

        public string Adresse => _adresse;

        public int CouleurId => _couleurId;

        public EStatutChantier Statut => _statut;

        // *** DATAS ***************************

        public long _keyId;

        public string _name;

        public string _refDevis;

        public string _adresse;

        public int _couleurId;

        public EStatutChantier _statut;

        public CChantier(long keyId, string name, string refDevis, 
            string adresse, int couleurId, EStatutChantier statut)
        {
            _keyId = keyId;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _refDevis = refDevis ?? throw new ArgumentNullException(nameof(refDevis));
            _adresse = adresse ?? throw new ArgumentNullException(nameof(adresse));
            _couleurId = couleurId;
            _statut = statut;
        }

        public override string ToString()
        {
            return _name + " - " + _refDevis + " - " + _adresse;
        }
    }
}
