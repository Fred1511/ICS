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

        public string DateAcceptationDevis => _dateAcceptationDevis;

        public string DatePrevisionnelleTravaux => _datePrevisionnelleTravaux;

        public int NbDHeuresADeuxTechniciens => _nbDHeuresADeuxTechniciens;

        public int NbDHeuresAUnTechnicien => _nbDHeuresAUnTechnicien;

        public float PrixDeVenteHT => _prixDeVenteHT;

        // *** DATAS ***************************

        public long _keyId;

        public string _name;

        public string _refDevis;

        public string _adresse;

        public int _couleurId;

        public EStatutChantier _statut;

        public string _dateAcceptationDevis;

        public string _datePrevisionnelleTravaux;

        public int _nbDHeuresADeuxTechniciens;

        public int _nbDHeuresAUnTechnicien;

        public float _prixDeVenteHT;

        public CChantier(
            long keyId, 
            string name, 
            string refDevis, 
            string adresse, 
            int couleurId, 
            EStatutChantier statut,
            string dateAcceptationDevis = "",
            string datePrevisionnelleTravaux = "",
            int nbDHeuresADeuxTechniciens = 0,
            int nbDHeuresAUnTechnicien = 0,
            float prixDeVenteHT = 0
            )
        {
            _keyId = keyId;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _refDevis = refDevis ?? throw new ArgumentNullException(nameof(refDevis));
            _adresse = adresse ?? throw new ArgumentNullException(nameof(adresse));
            _couleurId = couleurId;
            _statut = statut;
            _dateAcceptationDevis = dateAcceptationDevis ?? throw new ArgumentNullException(nameof(dateAcceptationDevis));
            _datePrevisionnelleTravaux = datePrevisionnelleTravaux ?? throw new ArgumentNullException(nameof(dateAcceptationDevis));
            _nbDHeuresADeuxTechniciens = nbDHeuresADeuxTechniciens;
            _nbDHeuresAUnTechnicien = nbDHeuresAUnTechnicien;
            _prixDeVenteHT = prixDeVenteHT;
        }

        public override string ToString()
        {
            return _name + " - " + _refDevis + " - " + _adresse;
        }
    }
}
