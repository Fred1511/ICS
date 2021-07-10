using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public interface IEmployee
    {
        long KeyId { get; }

        string Name { get; }
    }

    public class CEmployee : IEmployee
    {
        public long KeyId => _keyId;

        public string Name => _name;

        public CEmployee(long keyId, string name)
        {
            _keyId = keyId;
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public long _keyId;

        public string _name;

        public override string ToString()
        {
            return _name;
        }
    }

    class ReadDatasOnFile
    {
        public ReadDatasOnFile()
        {
            if (false == File.Exists(PathToEmployeesFile))
            {
                var employees = new CEmployee[3]
                    {
                        new CEmployee(0, "Frédéric"),
                        new CEmployee(1, "Stéphan"),
                        new CEmployee(1, "Guillaume")
                    };

                ModifyEmployeesFile(employees);
            }
        }

        public void Core()
        {
            long counter_1 = 0;
            long counter_2 = 0;
            long counter = 0;
            while (false == System.Console.KeyAvailable)
            {
                var employees = ReadEmployeesFromFile();
                if (counter_1 > 5000)
                {
                    foreach (var employee in employees)
                    {
                        System.Console.WriteLine(employee);
                    }
                    counter_1 = 0;
                }

                if (counter_2 > 100)
                {
                    employees[0]._name = "Frédéric " + counter;
                    ModifyEmployeesFile(employees);
                    counter_2 = 0;
                }

                counter_2++;
                counter_1++;
                counter++;
            }
        }

        private const int MaximumTimeToWait_ms = 1000;

        private string PathToEmployeesFile => @"C:\Users\Utilisateur\SynologyDrive\Forsim\0 - Autres projets\ICS\Employees.fic";

        private long CreateEmployee(string employeeName)
        {
            using (var fileStream = new FileStream(PathToEmployeesFile, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                var keyId = 0;
                var newEmployee = new CEmployee(keyId, employeeName);

                using (var writer = new BinaryWriter(fileStream))
                {
                    WriteEmployeeToFile(writer, newEmployee);
                }

                return keyId;
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

        private void ModifyEmployeesFile(CEmployee[] employees)
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
                catch(IOException)
                {

                }

                if (watch.ElapsedMilliseconds > MaximumTimeToWait_ms)
                {
                    throw new IOException();
                }
            }
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
    }
}
