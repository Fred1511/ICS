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
    }
}
