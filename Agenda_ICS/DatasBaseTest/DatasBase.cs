using NDatasModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace NDatasBaseTest
{
    public class DatasBase : IDatasBase
    {
        // *** PUBLIC **********************

        public DatasBase()
        {
            FillWithTestDataSet();
        }

        public IEmployee[] GetEmployees()
        {
            return _listOfEmployees.ToArray();
            //return ReadEmployeesFromSql();
        }

        public ITask[] GetTasksOfEmployee(string employee, DateTime periodBeginsAt, int nbWeeks)
        {
            var periodEndsAt = periodBeginsAt + new TimeSpan(nbWeeks * 7, 0, 0, 0);

            var tasks = _listOfTasks.Where(x => x._employeesName.Contains(employee) && x.BeginsAt < periodEndsAt && x.EndsAt > periodBeginsAt).OrderBy(x => x.BeginsAt).ToArray();

            return tasks.ToArray();
        }

        public ITask AddTaskToEmployee(string employeeName, string nomChantier, DateTime beginsAt, DateTime endsAt)
        {
            return InternalAddTaskToEmployee(employeeName, nomChantier, beginsAt, endsAt);
        }

        public ITask ModifyTask(CTask taskModified)
        {
            var task = _listOfTasks.Find(x => x.KeyId == taskModified.KeyId);
            task.CopyDatasFromTask(taskModified);

            return task;
        }

        public ITask GetTask(long keyId)
        {
            var task = _listOfTasks.Find(x => x.KeyId == keyId);
            return task;
        }

        // *** RESTRICTED ******************

        private List<CTask> _listOfTasks = new List<CTask>();

        private List<CEmployee> _listOfEmployees;

        private long _lastKeyId;

        public CTask InternalAddTaskToEmployee(string employeeName, string nomChantier, DateTime beginsAt, DateTime endsAt)
        {
            var task = new CTask(new List<string>() { employeeName }, nomChantier, beginsAt, endsAt, ++_lastKeyId);
            _listOfTasks.Add(task);

            return task;
        }

        public CTask InternalAddTaskToEmployee(List<string> employeesNames, string nomChantier, DateTime beginsAt, DateTime endsAt)
        {
            var task = new CTask(employeesNames, nomChantier, beginsAt, endsAt, ++_lastKeyId);
            _listOfTasks.Add(task);

            return task;
        }

        private void FillWithTestDataSet()
        {
            _listOfEmployees = new List<CEmployee>()
            {
                new CEmployee("Frédéric ZITTA"),
                new CEmployee("Sylvester STALLONE"),
                new CEmployee("Franck CAMDERA"),
                new CEmployee("Bruce LEE"),
                new CEmployee("Joel COLUMSY"),
                new CEmployee("Alan TURING"),
                new CEmployee("Brad PITT"),
                new CEmployee("Matt DAMON"),
            };

            _listOfTasks = new List<CTask>()
            {
                InternalAddTaskToEmployee( new List<string>() { "Frédéric ZITTA", "Brad PITT" }, "Chantier A", new DateTime(2021, 1, 5, 10, 00, 00), new DateTime(2021, 1, 7, 17, 00, 00)),
                InternalAddTaskToEmployee(new List<string>() { "Joel COLUMSY" }, "Chantier B", new DateTime(2021, 1, 8, 12, 00, 00), new DateTime(2021, 1, 9, 10, 00, 00)),
            };
        }
    }
}
