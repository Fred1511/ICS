using NDatasModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDatasBaseOnFile
{
    public class DatasBase : IDatasBase
    {
        // *** PUBLIC **********************

        public DatasBase(string folderPath)
        {
            _folderPath = folderPath;

            if (false == File.Exists(PathToEmployeesFile) 
                || false == File.Exists(PathToChantiersFile)
                || false == File.Exists(PathToTasksFile)
                || false == File.Exists(PathToJoursFériésFile)
                )
            {
                CreateDefaultDataSet();
            }
        }

        public long CreateEmployee(string employeeName)
        {
            var employees = new List<CEmployee>(ReadEmployeesFromFile());
            if (null != employees.Find(x => x.Name == employeeName))
            {
                // Cet employé existe déjà
                return -1;
            }

            var keyId = GetAvailableKeyIdForNewEmployee(employees.ToArray());
            var newEmployee = new CEmployee(keyId, employeeName);
            employees.Add(newEmployee);

            WriteEmployeesToFile(employees.ToArray());

            return keyId;
        }

        public void RemoveEmployee(long employeeKeyId)
        {
            var tasks = new List<CTask>(ReadTasksFromFile());
            tasks.RemoveAll(x => x._employeeKeyId == employeeKeyId);
            WriteTasksToFile(tasks.ToArray());

            var employees = new List<CEmployee>(ReadEmployeesFromFile());
            employees.RemoveAll(x => x.KeyId == employeeKeyId);
            WriteEmployeesToFile(employees.ToArray());
        }

        public void ModifyEmployee(long employeeKeyId, string employeeName)
        {
            var employees = new List<CEmployee>(ReadEmployeesFromFile());
            var employeeToModify = employees.Find(x => x.KeyId == employeeKeyId);
            if (null == employeeToModify)
            {
                // Cet employé n'existe plus
                return;
            }

            employeeToModify._name = employeeName;

            WriteEmployeesToFile(employees.ToArray());
        }

        public long CreateChantier(string chantierName, string refDevis, string adresse,
            int couleurId, EStatutChantier statut)
        {
            var chantiers = new List<CChantier>(ReadChantiersFromFile());
            var keyId = GetAvailableKeyIdForNewChantier(chantiers.ToArray());
            var newChantier = new CChantier(keyId, chantierName, refDevis, adresse, couleurId, statut);
            chantiers.Add(newChantier);

            WriteChantiersToFile(chantiers.ToArray());

            return keyId;
        }

        public void RemoveChantier(long chantierKeyId)
        {
            var tasks = new List<CTask>(ReadTasksFromFile());
            tasks.RemoveAll(x => x._chantierKeyId == chantierKeyId);
            WriteTasksToFile(tasks.ToArray());

            var chantiers = new List<CChantier>(ReadChantiersFromFile());
            chantiers.RemoveAll(x => x.KeyId == chantierKeyId);
            WriteChantiersToFile(chantiers.ToArray());
        }

        public void ModifyChantier(long chantierKeyId, string chantierName, string refDevis,
            string adresse, int couleurId, EStatutChantier statut)
        {
            var chantiers = new List<CChantier>(ReadChantiersFromFile());
            var chantierToModify = chantiers.Find(x => x.KeyId == chantierKeyId);
            if (null == chantierToModify)
            {
                // Ce chantier n'existe plus
                return;
            }

            chantierToModify._name = chantierName;
            chantierToModify._refDevis = refDevis;
            chantierToModify._adresse = adresse;
            chantierToModify._couleurId = couleurId;
            chantierToModify._statut = statut;

            WriteChantiersToFile(chantiers.ToArray());
        }

        public IEmployee[] GetEmployees()
        {
            return ReadEmployeesFromFile();
        }

        public IChantier[] GetChantiers()
        {
            return ReadChantiersFromFile();
        }

        public ITask[] GetTasksOfEmployee(long employeeKeyId, DateTime periodBeginsAt, int nbWeeks)
        {
            var tasks = GetTasksOfEmployeeFromFile(employeeKeyId, periodBeginsAt, nbWeeks);

            return tasks.ToArray();
        }

        public long AddTaskToEmployee(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt)
        {
            var chantiers = new List<CChantier>(ReadChantiersFromFile());
            var employees = new List<CEmployee>(ReadEmployeesFromFile());
            var chantier = chantiers.Find(x => x.KeyId == chantierKeyId);
            var employee = employees.Find(x => x.KeyId == employeeKeyId);
            if (null == chantier || null == employee)
            {
                // Cet employé ou ce chantier n'existe plus
                return -1;
            }

            var tasks = new List<CTask>(ReadTasksFromFile());
            var keyId = GetAvailableKeyIdForNewTask(tasks.ToArray());
            var newTask = new CTask(keyId);
            newTask._employeeKeyId = employeeKeyId;
            newTask._chantierKeyId = chantierKeyId;
            newTask._beginsAt = beginsAt;
            newTask._endsAt = endsAt;

            tasks.Add(newTask);

            WriteTasksToFile(tasks.ToArray());

            return keyId;
        }

        public ITask ModifyTask(CTask taskModified)
        {
            var tasks = new List<CTask>(ReadTasksFromFile());
            var taskToModify = tasks.Find(x => x.KeyId == taskModified._keyId);
            if (null == taskToModify)
            {
                // Cette tâche n'existe plus
                return null;
            }

            taskToModify._employeeKeyId = taskModified.EmployeeKeyId;
            taskToModify._chantierKeyId = taskModified.ChantierKeyId;
            taskToModify._beginsAt = taskModified.BeginsAt;
            taskToModify._endsAt = taskModified.EndsAt;
            taskToModify._altitudeUI = taskModified.AltitudeUI;

            WriteTasksToFile(tasks.ToArray());

            return taskToModify;
        }

        public void DeleteTask(long taskKeyId)
        {
            var tasks = new List<CTask>(ReadTasksFromFile());
            tasks.RemoveAll(x => x.KeyId == taskKeyId);

            WriteTasksToFile(tasks.ToArray());
        }

        public ITask GetTask(long keyId)
        {
            var tasks = new List<CTask>(ReadTasksFromFile());
            return tasks.Find(x => x.KeyId == keyId);
        }

        public IChantier GetChantier(long chantierKeyId)
        {
            var chantiers = new List<CChantier>(ReadChantiersFromFile());
            return chantiers.Find(x => x.KeyId == chantierKeyId);
        }

        public IEmployee GetEmployee(long employeeKeyId)
        {
            var employees = new List<CEmployee>(ReadEmployeesFromFile());
            return employees.Find(x => x.KeyId == employeeKeyId);
        }

        public void AddJourFérié(DateTime jour)
        {
            var joursFériés = new List<CJourFérié>(ReadJoursFériésFromFile());
            var keyId = GetAvailableKeyIdForNewJourFérié(joursFériés.ToArray());
            var newJourFérié = new CJourFérié(keyId, jour);

            joursFériés.Add(newJourFérié);
            WriteJoursFériésToFile(joursFériés.ToArray());
        }

        public void RemoveJourFérié(DateTime jour)
        {
            var joursFériés = new List<CJourFérié>(ReadJoursFériésFromFile());
            joursFériés.RemoveAll(x => x.Jour == jour);

            WriteJoursFériésToFile(joursFériés.ToArray());
        }

        public IJourFérié[] GetJoursFériés()
        {
            var joursFériés = new List<CJourFérié>(ReadJoursFériésFromFile());

            return joursFériés.ToArray();
        }

        // *** RESTRICTED ******************

        private const int MaximumTimeToWait_ms = 1000;

        private string _folderPath;

        private string PathToEmployeesFile => _folderPath + @"\Employees.fic";

        private string PathToChantiersFile => _folderPath + @"\Chantiers.fic";

        private string PathToTasksFile => _folderPath + @"\Tasks.fic";

        private string PathToJoursFériésFile => _folderPath + @"\JoursFériés.fic";

        private void CreateDefaultDataSet()
        {
            var employees = new CEmployee[9]
                {
                    new CEmployee(0, "Adi"),
                    new CEmployee(1, "Baptiste"),
                    new CEmployee(2, "Cédric"),
                    new CEmployee(3, "Dominique"),
                    new CEmployee(4, "Gwendal"),
                    new CEmployee(5, "Kevin"),
                    new CEmployee(6, "Paul-Henry"),
                    new CEmployee(7, "Reky"),
                    new CEmployee(8, "Sylvain"),
                };
            WriteEmployeesToFile(employees);

            var chantiers = new CChantier[2]
                {
                    new CChantier(0, "NAVAL-GROUP (test)", "R0001", "11 rue Claude CHAPPE 29200 Brest", 0, EStatutChantier.A_CONFIRMER),
                    new CChantier(1, "APPLE (test)", "R0002", "39 allée des acacias 29280 Plouzane", 1, EStatutChantier.CLOS),
                };
            WriteChantiersToFile(chantiers);

            var tasks = new CTask[3]
                {
                    new CTask(1, 0, new DateTime(2021, 6, 21, 10, 0, 0), new DateTime(2021, 6, 22, 16, 0, 0), 0),
                    new CTask(3, 1, new DateTime(2021, 6, 22, 10, 0, 0), new DateTime(2021, 6, 24, 16, 0, 0), 1),
                    new CTask(6, 1, new DateTime(2021, 6, 22, 10, 0, 0), new DateTime(2021, 6, 25, 16, 0, 0), 2),
                };
            WriteTasksToFile(tasks);

            var joursFériés = new CJourFérié[2]
                {
                    new CJourFérié(0, new DateTime(2021, 6, 18, 10, 0, 0)),
                    new CJourFérié(1, new DateTime(2021, 6, 29, 10, 0, 0)),
                };
            WriteJoursFériésToFile(joursFériés);
        }

        private string FormatDateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd;HH:mm:ss");
        }

        private DateTime DateTimeFromString(string fromSQL)
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

        private string FormatDateToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        private DateTime DateFromString(string fromSQL)
        {
            //"yyyy-MM-dd"
            var componentsDate = fromSQL.Split('-');
            var year = int.Parse(componentsDate[0]);
            var month = int.Parse(componentsDate[1]);
            var day = int.Parse(componentsDate[2]);

            return new DateTime(year, month, day, 0, 0, 0);
        }
        
        private CJourFérié[] ReadJoursFériésFromFile()
        {
            var output = new List<CJourFérié>();

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToJoursFériésFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = new BinaryReader(fileStream))
                        {
                            var nbJoursFériés = reader.ReadInt32();

                            for (var i = 0; i < nbJoursFériés; i++)
                            {
                                var jourFérié = ReadJourFériéFromFile(reader);
                                output.Add(jourFérié);
                            }
                        }
                    }
                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }

            return output.ToArray();

        }

        private void WriteJoursFériésToFile(CJourFérié[] joursFériés)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToJoursFériésFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            writer.Write(joursFériés.Length);

                            foreach (var jourFérié in joursFériés)
                            {
                                WriteJourFériéToFile(writer, jourFérié);
                            }
                        }
                    }

                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }
        }

        private CEmployee[] ReadEmployeesFromFile()
        {
            var output = new List<CEmployee>();

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToEmployeesFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = new BinaryReader(fileStream))
                        {
                            var nbEmployees = reader.ReadInt32();

                            for (var i = 0; i < nbEmployees; i++)
                            {
                                var employee = ReadEmployeeFromFile(reader);
                                output.Add(employee);
                            }
                        }
                    }
                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }

            return output.ToArray();

        }

        private void WriteEmployeesToFile(CEmployee[] employees)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToEmployeesFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            writer.Write(employees.Length);

                            foreach (var employee in employees)
                            {
                                WriteEmployeeToFile(writer, employee);
                            }
                        }
                    }

                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }
        }

        private CChantier[] ReadChantiersFromFile()
        {
            var output = new List<CChantier>();

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToChantiersFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = new BinaryReader(fileStream))
                        {
                            var nbChantiers = reader.ReadInt32();

                            for (var i = 0; i < nbChantiers; i++)
                            {
                                var chantier = ReadChantierFromFile(reader);
                                output.Add(chantier);
                            }
                        }
                    }
                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }

            return output.ToArray();

        }

        private void WriteChantiersToFile(CChantier[] chantiers)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToChantiersFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            writer.Write(chantiers.Length);

                            foreach (var chantier in chantiers)
                            {
                                WriteChantierToFile(writer, chantier);
                            }
                        }
                    }

                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }
        }

        private CTask[] ReadTasksFromFile()
        {
            var output = new List<CTask>();

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToTasksFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = new BinaryReader(fileStream))
                        {
                            var nbTasks = reader.ReadInt32();

                            for (var i = 0; i < nbTasks; i++)
                            {
                                var task = ReadTaskFromFile(reader);
                                output.Add(task);
                            }
                        }
                    }
                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }

            return output.ToArray();

        }

        private void WriteTasksToFile(CTask[] tasks)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var isFileAccessSucceeded = false;
            while (!isFileAccessSucceeded)
            {
                try
                {
                    using (var fileStream = new FileStream(PathToTasksFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            writer.Write(tasks.Length);

                            foreach (var task in tasks)
                            {
                                WriteTaskToFile(writer, task);
                            }
                        }
                    }

                    isFileAccessSucceeded = true;
                }
                catch (IOException)
                {
                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }
        }

        private CTask[] GetTasksOfEmployeeFromFile(long employeeKeyId, DateTime periodBeginsAt, int nbWeeks)
        {
            var tasks = new List<CTask>(ReadTasksFromFile());
            var periodEndsAt = periodBeginsAt + new TimeSpan(nbWeeks * 7, 0, 0, 0);

            var tasksOfEmployee = tasks.FindAll(x => x._employeeKeyId == employeeKeyId
                && x.BeginsAt < periodEndsAt && x.EndsAt > periodBeginsAt);

            return tasksOfEmployee.ToArray();
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

        private CEmployee ReadEmployeeFromFile(BinaryReader reader)
        {
            var keyid = reader.ReadInt64();
            var name = reader.ReadString();

            return new CEmployee(keyid, name);
        }

        private void WriteEmployeeToFile(BinaryWriter writer, IEmployee employee)
        {
            writer.Write(employee.KeyId);
            writer.Write(employee.Name);
        }

        private CChantier ReadChantierFromFile(BinaryReader reader)
        {
            var keyid = reader.ReadInt64();
            var name = reader.ReadString();
            var refDevis = reader.ReadString();
            var adresse = reader.ReadString();
            var couleurId = reader.ReadInt32();
            var statut = (EStatutChantier)(reader.ReadInt32());

            return new CChantier(keyid, name, refDevis, adresse, couleurId, statut);
        }

        private void WriteChantierToFile(BinaryWriter writer, IChantier chantier)
        {
            writer.Write(chantier.KeyId);
            writer.Write(chantier.Name);
            writer.Write(chantier.RefDevis);
            writer.Write(chantier.Adresse);
            writer.Write(chantier.CouleurId);
            writer.Write((int)(chantier.Statut));
        }

        private CJourFérié ReadJourFériéFromFile(BinaryReader reader)
        {
            var keyid = reader.ReadInt64();
            var dateTime = DateTimeFromString(reader.ReadString());

            return new CJourFérié(keyid, dateTime);
        }

        private void WriteJourFériéToFile(BinaryWriter writer, IJourFérié chantier)
        {
            writer.Write(chantier.KeyId);
            writer.Write(FormatDateTimeToString(chantier.Jour));
        }

        private CTask ReadTaskFromFile(BinaryReader reader)
        {
            var keyid = reader.ReadInt64();
            var beginsAt = DateTimeFromString(reader.ReadString());
            var endsAt = DateTimeFromString(reader.ReadString());
            var chantierKeyId = reader.ReadInt64();
            var employeeKeyId = reader.ReadInt64();
            var altitudeUI = reader.ReadDouble();

            var task = new CTask(keyid);
            task._beginsAt = beginsAt;
            task._endsAt = endsAt;
            task._chantierKeyId = chantierKeyId;
            task._employeeKeyId = employeeKeyId;
            task._altitudeUI = altitudeUI;

            return task;
        }

        private void WriteTaskToFile(BinaryWriter writer, ITask task)
        {
            writer.Write(task.KeyId);
            writer.Write(FormatDateTimeToString(task.BeginsAt));
            writer.Write(FormatDateTimeToString(task.EndsAt));
            writer.Write(task.ChantierKeyId);
            writer.Write(task.EmployeeKeyId);
            writer.Write(task.AltitudeUI);
        }

        private static long GetAvailableKeyIdForNewEmployee(CEmployee[] employees)
        {
            long output = 0;
            foreach(var employee in employees)
            {
                if (output <= employee.KeyId)
                {
                    output = employee.KeyId + 1;
                }
            }

            return output;
        }

        private static long GetAvailableKeyIdForNewChantier(CChantier[] chantiers)
        {
            long output = 0;
            foreach (var chantier in chantiers)
            {
                if (output <= chantier.KeyId)
                {
                    output = chantier.KeyId + 1;
                }
            }

            return output;
        }

        private static long GetAvailableKeyIdForNewTask(CTask[] tasks)
        {
            long output = 0;
            foreach (var task in tasks)
            {
                if (output <= task.KeyId)
                {
                    output = task.KeyId + 1;
                }
            }

            return output;
        }

        private static long GetAvailableKeyIdForNewJourFérié(CJourFérié[] joursFériés)
        {
            long output = 0;
            foreach (var jourFérié in joursFériés)
            {
                if (output <= jourFérié.KeyId)
                {
                    output = jourFérié.KeyId + 1;
                }
            }

            return output;
        }
    }
}
