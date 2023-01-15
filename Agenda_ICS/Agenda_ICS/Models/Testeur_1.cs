using NDatasModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda_ICS
{
    interface ITesteur
    {
        void OnTick();
    }

    class Testeur_1 : ITesteur
    {
        public void OnTick()
        {
            switch(_stepCounter)
            {
                case 0:
                    {
                        _keyIdTask = Model.Instance.AddTaskToEmployee(_employeeKeyId, 0, new DateTime(2021, 6, 21, 10, 0, 0), new DateTime(2021, 6, 22, 16, 0, 0));
                        _stepCounter = 1;
                    }
                    break;
                case 1:
                    {
                        var task = new CTask(Model.Instance.GetTask(_keyIdTask));
                        task._beginsAt = new DateTime(2021, 6, 21, 08, 0, 0);
                        Model.Instance.ModifyTask(task);
                        _stepCounter = 2;
                    }
                    break;
                case 2:
                    {
                        Model.Instance.DeleteTask(_keyIdTask);
                        _stepCounter = 0;
                    }
                    break;
            }
        }

        private long _employeeKeyId = 7;

        private long _keyIdTask;

        private int _stepCounter;
    }

    class Testeur_2 : ITesteur
    {
        public void OnTick()
        {
            //switch (_stepCounter)
            //{
            //    case 0:
            //        {
            //            _keyIdTask = Model.Instance.AddTaskToEmployee(_employeeKeyId, 0, new DateTime(2021, 6, 21, 10, 0, 0), new DateTime(2021, 6, 22, 16, 0, 0));
            //            _stepCounter = 1;
            //        }
            //        break;
            //    case 1:
            //        {
            //            var task = new CTask(Model.Instance.GetTask(_keyIdTask));
            //            task._beginsAt = new DateTime(2021, 6, 21, 08, 0, 0);
            //            Model.Instance.ModifyTask(task);
            //            _stepCounter = 2;
            //        }
            //        break;
            //    case 2:
            //        {
            //            Model.Instance.DeleteTask(_keyIdTask);
            //            _stepCounter = 0;
            //        }
            //        break;
            //}
        }

        //private long _employeeKeyId = 8;

        //private long _keyIdTask;

        //private int _stepCounter;
    }
}
