namespace NDatasModel
{
    public interface IChantier
    {
        long KeyId { get; }

        string Name { get; }

        string RefDevis { get; }

        string Adresse { get; }

        int CouleurId { get; }

        EStatutChantier Statut { get; }

        string DateAcceptationDevis { get; }

        string DatePrevisionnelleTravaux { get; }

        int NbDeTechniciens { get; }

        int NbDHeuresAPlanifier { get; }

        float PrixDeVenteHT { get; }
    }
}
