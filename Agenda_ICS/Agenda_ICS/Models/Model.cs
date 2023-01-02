using NDatasModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Agenda_ICS
{
    class Model : IModel
    {
        // *** PUBLIC **********************

        public static IModel Instance;

        public Model()
        {
            Instance = this;

            var folderPath = GetFilePathToDatas();
            _db = new NDatasBaseOnFile.DatasBase(folderPath);

            InitiateTestModeIfNecessary(folderPath);
        }

        ~Model()
        {
            var folderPath = GetFilePathToDatas();
            File.Delete(folderPath + @"\TestId_" + _testeurId);
        }

        public long CreateEmployee(string employeeName)
        {
            return _db.CreateEmployee(employeeName);
        }

        public void RemoveEmployee(long employeeKeyId)
        {
            _db.RemoveEmployee(employeeKeyId);
        }

        public void ModifyEmployee(long employeeKeyId, string employeeName)
        {
            _db.ModifyEmployee(employeeKeyId, employeeName);
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
            return _db.CreateChantier(
                chantierName, 
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

        public void RemoveChantier(long chantierKeyId)
        {
            _db.RemoveChantier(chantierKeyId);
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
            _db.ModifyChantier(
                chantierKeyId, 
                chantierName,
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

        public IEmployee[] GetEmployees()
        {
            return _db.GetEmployees();
        }

        public IChantier[] GetChantiers()
        {
            return _db.GetChantiers();
        }

        public ITask[] GetTasksOfEmployee(long employeeKeyId, DateTime periodBeginsAt, int nbWeeks)
        {
            var tasks = _db.GetTasksOfEmployee(employeeKeyId, periodBeginsAt, nbWeeks);

            // On remplace les originaux par les copies en cours de modification le cas échéant
            for (var i = 0; i < tasks.Length; i++)
            {
                var task = tasks[i];
                if (_dictionaryOfTasksCopied.ContainsKey(task.KeyId))
                {
                    tasks[i] = _dictionaryOfTasksCopied[task.KeyId];
                }
            }

            return tasks.ToArray();
        }

        public long AddTaskToEmployee(long employeeKeyId, long chantierKeyId, DateTime beginsAt, DateTime endsAt)
        {
            return _db.AddTaskToEmployee(employeeKeyId, chantierKeyId, beginsAt, endsAt);
        }

        public ITask ModifyTask(CTask taskModified)
        {
            return _db.ModifyTask(taskModified);
        }

        public void DeleteTask(long taskKeyId)
        {
            _db.DeleteTask(taskKeyId);
        }

        public CTask AskCopyOfTaskForModification(long keyIdOfTask)
        {
            var original = _db.GetTask(keyIdOfTask);
            if (null == original)
            {
                throw new System.ArgumentException();
            }

            if (_dictionaryOfTasksCopied.ContainsKey(keyIdOfTask))
            {
                return _dictionaryOfTasksCopied[keyIdOfTask];
            }

            var copy = new CTask(original);
            _dictionaryOfTasksCopied.Add(keyIdOfTask, copy);

            return copy;
        }

        public void InformThatCopyIsNoLongerNecessary(long keyIdOfTask)
        {
            var original = _db.GetTask(keyIdOfTask);
            if (null == original)
            {
                return;
            }

            if (false == _dictionaryOfTasksCopied.ContainsKey(keyIdOfTask))
            {
                return;
            }

            _dictionaryOfTasksCopied.Remove(keyIdOfTask);
        }

        public static System.Windows.Media.Color GetBackgroundColor(int colorId)
        {
            switch(colorId)
            {
                case 0:
                default:
                    return System.Windows.Media.Colors.LightGreen;
                case 1:
                    return System.Windows.Media.Colors.LightPink;
                case 2:
                    return System.Windows.Media.Colors.Coral;
                case 3:
                    return System.Windows.Media.Colors.Yellow;
                case 4:
                    return System.Windows.Media.Colors.LightGray;
                case 5:
                    return System.Windows.Media.Colors.BurlyWood;
            }
        }

        public static System.Windows.Media.Color GetTextColor(int colorId)
        {
            switch (colorId)
            {
                case 0:
                default:
                    return System.Windows.Media.Colors.Black;
                case 1:
                    return System.Windows.Media.Colors.Black;
                case 2:
                    return System.Windows.Media.Colors.Black;
                case 3:
                    return System.Windows.Media.Colors.Black;
                case 4:
                    return System.Windows.Media.Colors.Black;
                case 5:
                    return System.Windows.Media.Colors.Black;
            }
        }

        public static int ConvertMediaColorToColorInt32(System.Windows.Media.Color color)
        {
            var bytes = new byte[4];
            bytes[0] = color.B;
            bytes[1] = color.G;
            bytes[2] = color.R;
            bytes[3] = 255;

            return BitConverter.ToInt32(bytes, 0);
        }

        public IChantier GetChantier(long chantierKeyId)
        {
            return _db.GetChantier(chantierKeyId);
        }

        public IEmployee GetEmployee(long employeeKeyId)
        {
            return _db.GetEmployee(employeeKeyId);
        }

        public ITask GetTask(long taskKeyId)
        {
            return _db.GetTask(taskKeyId);
        }

        public void AddJourFérié(DateTime jour)
        {
            _db.AddJourFérié(jour);
        }

        public void RemoveJourFérié(DateTime jour)
        {
            _db.RemoveJourFérié(jour);
        }

        public bool IsJourFérié(DateTime date)
        {
            var joursFeriés = GetJoursFériés();
            foreach(var jourFérié in joursFeriés)
            {
                if (jourFérié.Jour.DayOfYear == date.DayOfYear)
                {
                    return true;
                }
            }

            return false;
        }

        public IJourFérié[] GetJoursFériés()
        {
            return _db.GetJoursFériés();
        }

        public void OnTimer_100ms()
        {
            if (false == IsTestMode)
            {
                return;
            }

            _testeur.OnTick();
        }

        public bool IsTestMode { get; private set; }

        // *** RESTRICTED ******************

        private int _testeurId;

        private ITesteur _testeur;

        private IDatasBase _db;

        private Dictionary<long, CTask> _dictionaryOfTasksCopied = new Dictionary<long, CTask>();

        private string GetFilePathToDatas()
        {
            if (File.Exists("PathToDatas.txt"))
            {
                var lines = File.ReadAllLines("PathToDatas.txt");
                var path = lines[0];

                if (Directory.Exists(path))
                {
                        return path;
                }
            }

            var counter = 0;
            while (true)
            {
                if (MessageBox.Show("Merci d'indiquer la localisation du répertoire de données", "Configuration", MessageBoxButton.OK) == MessageBoxResult.OK)
                { 
                }

                using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
                {
                    var result = dlg.ShowDialog();
                    if (result != System.Windows.Forms.DialogResult.OK || string.IsNullOrWhiteSpace(dlg.SelectedPath))
                    {
                        counter++;
                        if (counter == 2)
                        {
                            Environment.Exit(0);
                        }
                        MessageBox.Show("Vous devez sélectionner un répertoire valide", "Erreur");
                        continue;
                    }

                    var content = new string[1];
                    content[0] = dlg.SelectedPath;
                    File.WriteAllLines("PathToDatas.txt", content);

                    return dlg.SelectedPath;
                }
            }
        }

        private void InitiateTestModeIfNecessary(string folderPath)
        {
            var filePath = folderPath + @"\TestMode";
            IsTestMode = File.Exists(filePath);

            if (false == IsTestMode)
            {
                return;
            }

            _testeurId = 1;
            while (true)
            {
                if (true == File.Exists(folderPath + @"\TestId_" + _testeurId))
                {
                    _testeurId++;
                    continue;
                }

                using (var fileStream = new FileStream(folderPath + @"\TestId_" + _testeurId, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        writer.Write(_testeurId);
                    }
                }
                break;
            }

            switch(_testeurId)
            {
                case 1:
                    _testeur = new Testeur_1();
                    break;
                case 2:
                    _testeur = new Testeur_2();
                    break;
            }
        }
    }
}
