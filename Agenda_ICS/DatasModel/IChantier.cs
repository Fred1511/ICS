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

        int NbDHeuresADeuxTechniciens { get; }

        int NbDHeuresAUnTechnicien { get; }

        float PrixDeVenteHT { get; }
    }
}
