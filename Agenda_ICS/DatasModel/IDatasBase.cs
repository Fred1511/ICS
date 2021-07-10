﻿using System;

namespace NDatasModel
{
    public interface IDatasBase
    {
        long CreateEmployee(string employeeName);

        void RemoveEmployee(long employeeKeyId);

        void ModifyEmployee(long employeeKeyId, string employeeName);

        long CreateChantier(string chantierName, string refDevis, string adresse, 
            int couleur, EStatutChantier statut);

        void RemoveChantier(long chantierKeyId);

        void ModifyChantier(long chantierKeyId, string chantierName, string refDevis, 
            string adresse, int couleur, EStatutChantier statut);

        IEmployee[] GetEmployees();

        IChantier[] GetChantiers();

        ITask[] GetTasksOfEmployee(long employeeKeyId, DateTime firstMonday, int nbWeeks);

        ITask GetTask(long keyId);

        long AddTaskToEmployee(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt);

        ITask ModifyTask(CTask taskModified);

        void DeleteTask(long taskKeyId);

        IChantier GetChantier(long chantierKeyId);

        IEmployee GetEmployee(long employeeKeyId);

        void AddJourFérié(DateTime jour);

        void RemoveJourFérié(DateTime jour);

        IJourFérié[] GetJoursFériés();
    }
}