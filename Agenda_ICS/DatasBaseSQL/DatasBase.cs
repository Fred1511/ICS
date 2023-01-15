using NDatasModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace NDatasBaseSQL
{
    public class DatasBase : IDatasBase
    {
        // *** PUBLIC **********************

        public DatasBase(string path)
        {
            _connexion.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; 
                            AttachDbFilename = " + path + @"; 
                            Integrated Security = True";
            _connexion.Open();
        }

        ~DatasBase()
        {
            try
            {
                _connexion?.Close();
            }
            catch
            {

            }
        }

        public long CreateEmployee(string employeeName)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"INSERT INTO Employes (nom)"
                    + " VALUES (@nom)"
                    + ";SELECT SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@nom", employeeName);

                var lastId = Convert.ToInt32(command.ExecuteScalar());
                return lastId;
            }
        }

        public void RemoveEmployee(long employeeKeyId)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"DELETE FROM Employes WHERE keyId = {employeeKeyId}";

                command.ExecuteNonQuery();
            }
        }

        public void ModifyEmployee(long employeeKeyId, string employeeName)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"UPDATE Employes SET nom = @nom" +
                    $" WHERE keyId = {employeeKeyId}";
                command.Parameters.AddWithValue("@nom", employeeName);

                var nbLignesAffectées = command.ExecuteNonQuery();
                if (1 != nbLignesAffectées)
                {
                    throw new System.NotImplementedException();
                }
            }
        }

        public long CreateChantier(
            string chantierName, 
            string refDevis, 
            string adresse, 
            int couleurId,
            EStatutChantier statut,
            string dateAcceptationDevis,
            string datePrevisionnelleTravaux,
            int nbDeTechniciens,
            int nbDHeuresAPlanifier,
            float prixDeVenteHT
            )
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = 
                    $"INSERT INTO Chantiers (nom, refDevis, adresse, couleurId, statut, " 
                    + "dateAcceptationDevis, datePrevisionnelleTravaux, nbDeTechniciens, nbDHeuresAPlanifier, prixDeVenteHT)"
                    + " VALUES (@nom, @refDevis, @adresse, @couleurId, @statut, @dateAcceptationDevis, @datePrevisionnelleTravaux, "
                    + "@nbDeTechniciens, @nbDHeuresAPlanifier, @prixDeVenteHT)"
                    + ";SELECT SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@nom", chantierName);
                command.Parameters.AddWithValue("@refDevis", refDevis);
                command.Parameters.AddWithValue("@adresse", adresse);
                command.Parameters.AddWithValue("@couleurId", couleurId);
                command.Parameters.AddWithValue("@statut", (int)statut);
                command.Parameters.AddWithValue("@dateAcceptationDevis", dateAcceptationDevis);
                command.Parameters.AddWithValue("@datePrevisionnelleTravaux", datePrevisionnelleTravaux);
                command.Parameters.AddWithValue("@nbDeTechniciens", nbDeTechniciens);
                command.Parameters.AddWithValue("@nbDHeuresAPlanifier", nbDHeuresAPlanifier);
                command.Parameters.AddWithValue("@prixDeVenteHT", prixDeVenteHT);

                var lastId = Convert.ToInt32(command.ExecuteScalar());
                return lastId;
            }
        }

        public void RemoveChantier(long chantierKeyId)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"DELETE FROM Chantiers WHERE keyId = {chantierKeyId}";

                command.ExecuteNonQuery();
            }
        }

        public void ModifyChantier(
            long chantierKeyId, 
            string chantierName, 
            string refDevis, 
            string adresse, 
            int couleurId, 
            EStatutChantier statut,
            string dateAcceptationDevis,
            string datePrevisionnelleTravaux,
            int nbDeTechniciens,
            int nbDHeuresAPlanifier,
            float prixDeVenteHT
            )
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"UPDATE Chantiers SET nom = @nom, refDevis = @refDevis, adresse = @adresse" 
                    + ", couleurId = @couleurId, statut = @statut"
                    + $" WHERE keyId = {chantierKeyId}";
                command.Parameters.AddWithValue("@nom", chantierName);
                command.Parameters.AddWithValue("@refDevis", refDevis);
                command.Parameters.AddWithValue("@adresse", adresse);
                command.Parameters.AddWithValue("@couleurId", couleurId);
                command.Parameters.AddWithValue("@statut", (int)statut);
                command.Parameters.AddWithValue("@adresse", dateAcceptationDevis);
                command.Parameters.AddWithValue("@adresse", datePrevisionnelleTravaux);
                command.Parameters.AddWithValue("@adresse", nbDeTechniciens);
                command.Parameters.AddWithValue("@adresse", nbDHeuresAPlanifier);
                command.Parameters.AddWithValue("@adresse", prixDeVenteHT);

                var nbLignesAffectées = command.ExecuteNonQuery();
                if (1 != nbLignesAffectées)
                {
                    throw new System.NotImplementedException();
                }
            }
        }
        
        public IEmployee[] GetEmployees()
        {
            return ReadEmployeesFromSql();
        }

        public IChantier[] GetChantiers()
        {
            return ReadChantiersFromSql();
        }

        public ITask[] GetTasksOfEmployee(long employeeKeyId, DateTime periodBeginsAt, int nbWeeks)
        {
            var tasks = GetTasksOfEmployeeFromSql(employeeKeyId, periodBeginsAt, nbWeeks);

            return tasks.ToArray();
        }

        public long AddTaskToEmployee(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt)
        {
            var task = AddTaskToEmployeeForSql(employeeKeyId, chantierKeyId, beginsAt, endsAt);
            return task.KeyId;
        }

        public ITask ModifyTask(CTask taskModified)
        {
            var originalTask = GetTaskFromSql(taskModified.KeyId);
            if (AreTaskEquals(originalTask, taskModified))
            {
                return originalTask;
            }

            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"UPDATE Tasks SET beginsAt = @beginsAt," +
                    $" endsAt = @endsAt, chantierId = @chantierId, employeId = @employeId," +
                    $" altitudeUi = @altitudeUi" +
                    $" WHERE keyId = {taskModified.KeyId}";
                command.Parameters.AddWithValue("@beginsAt", FormatDateTimeForSql(taskModified.BeginsAt));
                command.Parameters.AddWithValue("@endsAt", FormatDateTimeForSql(taskModified.EndsAt));
                command.Parameters.AddWithValue("@chantierId", taskModified.ChantierKeyId);
                command.Parameters.AddWithValue("@employeId", taskModified.EmployeeKeyId);
                command.Parameters.AddWithValue("@altitudeUi", taskModified.AltitudeUI);

                var nbLignesAffectées = command.ExecuteNonQuery();
                if (1 != nbLignesAffectées)
                {
                    throw new System.NotImplementedException();
                }
            }

            return taskModified;
        }

        public void DeleteTask(long taskKeyId)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"DELETE FROM Tasks WHERE keyId = {taskKeyId}";

                command.ExecuteNonQuery();
            }
        }

        public ITask GetTask(long keyId)
        {
            return GetTaskFromSql(keyId);
        }

        public IChantier GetChantier(long chantierKeyId)
        {
            var cmdText = $"SELECT * FROM Chantiers WHERE keyId = {chantierKeyId}";
            using (SqlCommand command = new SqlCommand(cmdText, _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return ReadChantierFromSql(reader);
                }
            }

            throw new NotImplementedException();
        }

        public IEmployee GetEmployee(long employeeKeyId)
        {
            var cmdText = $"SELECT * FROM Employes WHERE keyId = {employeeKeyId}";
            using (SqlCommand command = new SqlCommand(cmdText, _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var keyId = reader.GetInt32(0);
                    var nomEmploye = reader.GetString(1);

                    return new CEmployee(keyId, nomEmploye);
                }
            }

            throw new NotImplementedException();
        }

        public void AddJourFérié(DateTime jour)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"INSERT INTO JoursFeries (jour)"
                    + " VALUES (@jour)";
                command.Parameters.AddWithValue("@jour", FormatDateForSql(jour));

                command.ExecuteNonQuery();
            }
        }

        public void RemoveJourFérié(DateTime jour)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"DELETE FROM JoursFeries WHERE jour = @jour";
                command.Parameters.AddWithValue("@jour", FormatDateForSql(jour));

                command.ExecuteNonQuery();
            }
        }

        public IJourFérié[] GetJoursFériés()
        {
            var joursFériés = new List<IJourFérié>();
            var cmdText = $"SELECT * FROM JoursFeries";

            using (SqlCommand command = new SqlCommand(cmdText, _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var keyID = reader.GetInt32(0);
                    var jour = DateFromSql(reader.GetString(1));

                    joursFériés.Add(new CJourFérié(keyID, jour));
                }

                reader.Close();
            }

            return joursFériés.ToArray();
        }

        public ITask[] GetTasks()
        {
            throw new NotImplementedException();
        }

        // *** RESTRICTED ******************

        private SqlConnection _connexion = new SqlConnection();

        public CTask AddTaskToEmployeeForSql(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt)
        {
            using (SqlCommand command = _connexion.CreateCommand())
            {
                command.CommandText = $"INSERT INTO Tasks (beginsAt, endsAt, chantierId, employeId, altitudeUi)"
                    + " VALUES (@beginsAt, @endsAt, @chantierId, @employeId, @altitudeUi)"
                    + ";SELECT SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@beginsAt", FormatDateTimeForSql(beginsAt));
                command.Parameters.AddWithValue("@endsAt", FormatDateTimeForSql(endsAt));
                command.Parameters.AddWithValue("@chantierId", chantierKeyId);
                command.Parameters.AddWithValue("@employeId", employeeKeyId);
                command.Parameters.AddWithValue("@altitudeUi", 0);

                var lastId = Convert.ToInt32(command.ExecuteScalar());
                return GetTaskFromSql(lastId);
            }
        }

        private string FormatDateTimeForSql(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd;HH:mm:ss");
        }

        private DateTime DateTimeFromSql(string fromSQL)
        {
            //"yyyy-MM-dd;HH:mm:ss"
            var components = fromSQL.Split(';');
            var date = components[0];
            var time = components[1];

            var componentsDate = date.Split('-');
            var year = int.Parse(componentsDate[0]);
            var month = int.Parse(componentsDate[1]);
            var day = int.Parse(componentsDate[2]);

            var componentsTime = time.Split(':');
            var hours = int.Parse(componentsTime[0]);
            var minutes = int.Parse(componentsTime[1]);
            var secondes = int.Parse(componentsTime[2]);

            return new DateTime(year, month, day, hours, minutes, secondes);
        }

        private string FormatDateForSql(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        private DateTime DateFromSql(string fromSQL)
        {
            //"yyyy-MM-dd"
            var componentsDate = fromSQL.Split('-');
            var year = int.Parse(componentsDate[0]);
            var month = int.Parse(componentsDate[1]);
            var day = int.Parse(componentsDate[2]);

            return new DateTime(year, month, day, 0, 0, 0);
        }

        private CEmployee[] ReadEmployeesFromSql()
        {
            var output = new List<CEmployee>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Employes ORDER BY nom", _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var keyId = reader.GetInt32(0);
                    var nomEmploye = reader.GetString(1);

                    var employee = new CEmployee(keyId, nomEmploye);
                    output.Add(employee);
                }
            }

            return output.ToArray();
        }

        private CChantier[] ReadChantiersFromSql()
        {
            var output = new List<CChantier>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Chantiers", _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var chantier = ReadChantierFromSql(reader);
                    output.Add(chantier);
                }
            }

            return output.ToArray();
        }

        private CTask GetTaskFromSql(long keyId)
        {
            var cmdText = $"SELECT * FROM Tasks WHERE keyId = {keyId}";

            using (SqlCommand command = new SqlCommand(cmdText, _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (false == reader.HasRows)
                {
                    return null;
                }

                reader.Read();

                var keyID = reader.GetInt32(0);
                var beginsAt = reader.GetString(1);
                var endsAt = reader.GetString(2);
                var chantierId = reader.GetInt32(3);
                var employeId = reader.GetInt32(4);
                var altitudeUI = reader.GetDouble(5);

                reader.Close();

                var task = new CTask(keyID)
                {
                    _beginsAt = DateTimeFromSql(beginsAt),
                    _endsAt = DateTimeFromSql(endsAt),
                    _employeeKeyId = employeId,
                    _chantierKeyId = chantierId,
                    _altitudeUI = altitudeUI
                };

                return task;
            }
        }

        private CTask[] GetTasksOfEmployeeFromSql(long employeeKeyId, DateTime periodBeginsAt, int nbWeeks)
        {
            var tasks = new List<CTask>();
            var periodEndsAt = periodBeginsAt + new TimeSpan(nbWeeks * 7, 0, 0, 0);
            
            var periodBeginsAtForSql = FormatDateTimeForSql(periodBeginsAt);
            var periodEndsAtForSql = FormatDateTimeForSql(periodEndsAt);

            var cmdText = $"SELECT * FROM Tasks WHERE beginsAt < '{periodEndsAtForSql}' AND endsAt > '{periodBeginsAtForSql}'";
            cmdText += " AND employeId = " + employeeKeyId;
            cmdText += "ORDER BY beginsAt";

            using (SqlCommand command = new SqlCommand(cmdText, _connexion))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                var keyIDs = new List<int>();
                var beginsAts = new List<string>();
                var endsAts = new List<string>();
                var chantierIds = new List<int>();
                var employeIds = new List<int>();
                var altitudeUIs = new List<double>();

                while (reader.Read())
                {
                    keyIDs.Add(reader.GetInt32(0));
                    beginsAts.Add(reader.GetString(1));
                    endsAts.Add(reader.GetString(2));
                    chantierIds.Add(reader.GetInt32(3));
                    employeIds.Add(reader.GetInt32(4));
                    altitudeUIs.Add(reader.GetDouble(5));
                }

                reader.Close();

                for (var i = 0; i < keyIDs.Count; i++)
                {
                    var task = new CTask(keyIDs[i])
                    {
                        _beginsAt = DateTimeFromSql(beginsAts[i]),
                        _endsAt = DateTimeFromSql(endsAts[i]),
                        _employeeKeyId = employeIds[i],
                        _chantierKeyId = chantierIds[i],
                        _altitudeUI = altitudeUIs[i]
                    };

                    tasks.Add(task);
                }
            }

            return tasks.ToArray();
        }

        private bool AreTaskEquals(CTask a, CTask b)
        {
            return (a.KeyId == b.KeyId)
                && (a.BeginsAt == b.BeginsAt)
                && (a.EndsAt == b.EndsAt)
                && (a.ChantierKeyId == b.ChantierKeyId)
                && (a.EmployeeKeyId == b.EmployeeKeyId)
                && (a.AltitudeUI == b.AltitudeUI);
        }

        private static CChantier ReadChantierFromSql(SqlDataReader reader)
        {
            var keyId = reader.GetInt32(0);
            var nomChantier = reader.GetString(1);
            var refDevis = reader.GetString(2);
            var adresse = reader.GetString(3);
            var couleurId = reader.GetInt32(4);
            var statut = (EStatutChantier)(reader.GetInt32(5));
            var dateAcceptationDevis = reader.GetString(6);
            var datePrevisionnelleTravaux = reader.GetString(7);
            var nbDeTechniciens = reader.GetInt32(8);
            var nbDHeuresAPlanifier = reader.GetInt32(9);
            var prixDeVenteHT = (float)reader.GetDouble(10);

            return new CChantier(
                keyId, 
                nomChantier, 
                refDevis, 
                adresse, 
                couleurId, 
                statut,
                dateAcceptationDevis,
                datePrevisionnelleTravaux,
                nbDeTechniciens,
                nbDHeuresAPlanifier,
                prixDeVenteHT
                );
        }
    }
}
