using NDatasModel;
using System;

namespace Agenda_ICS
{
    public interface IModel
    {
        long CreateEmployee(string employeeName);

        void RemoveEmployee(long employeeKeyId);

        void ModifyEmployee(long employeeKeyId, string employeeName);

        long CreateChantier(
            string chantierName, 
            string refDevis, 
            string adresse, 
            int couleurId, 
            EStatutChantier statut,
            string dateAcceptationDevis,
            string datePrevisionnelleTravaux,
            int nbDHeuresADeuxTechniciens,
            int nbDHeuresAUnTechnicien,
            float prixDeVenteHT
            );

        void RemoveChantier(long chantierKeyId);

        void ModifyChantier(
            long chantierKeyId, 
            string chantierName, 
            string refDevis, 
            string adresse, 
            int couleurId, 
            EStatutChantier statut,
            string dateAcceptationDevis,
            string datePrevisionnelleTravaux,
            int nbDHeuresADeuxTechniciens,
            int nbDHeuresAUnTechnicien,
            float prixDeVenteHT
            );

        void AddJourFérié(DateTime jour);

        void RemoveJourFérié(DateTime jour);

        bool IsJourFérié(DateTime date);

        IJourFérié[] GetJoursFériés();

        IEmployee[] GetEmployees();

        IChantier[] GetChantiers();

        IChantier[] GetBatigestChantiers();

        ITask[] GetTasksOfEmployee(long employeeKeyId, DateTime firstMonday, int nbWeeks);

        long AddTaskToEmployee(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt);

        ITask ModifyTask(CTask taskModified);

        void DeleteTask(long taskKeyId);

        CTask AskCopyOfTaskForModification(long keyID);

        void InformThatCopyIsNoLongerNecessary(long keyID);

        IChantier GetChantier(long chantierKeyId);

        IEmployee GetEmployee(long employeeKeyId);

        ITask GetTask(long taskKeyId);

        ITask[] GetTasks(long chantierKeyId = long.MinValue);

        void OnTimer_100ms();

        bool IsTestMode { get; }
    }
}