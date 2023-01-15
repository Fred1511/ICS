using NDatasModel;
using NOutils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Agenda_ICS.Views.Calendar
{
    enum EMouseMode
    {
        OUT,
        FLYING_FREELY,
        DRAGGING_FOR_NEW_TASK,
        DRAGGING_LIMIT_OF_AN_OLD_TASK,
        MOVING_TASK_VERTICALY
    }

    class CalendarOfEmployee : Canvas, CalendarOfEmployee.IOwnerOfTaskUI, CContextMenuOnTaskController.IParent
    {
        // *** PUBLIC ***************************

        public interface IOwnerOfTaskUI
        {
            void OnMouseUpOnTaskUI(TaskUI taskUI, MouseButtonEventArgs e);
        }

        public CalendarOfEmployee(ICalendarDisplayerForChildrens calendar, long employeeKeyId, DateTime firstDay, int nbWeeks)
        {
            _calendar = calendar;
            _employeeKeyId = employeeKeyId;
            ClipToBounds = true;

            _editTaskController = new CEditTaskController(_calendar);
            _dockPanel = new DockPanel();
            _firstDay = new DateTime(firstDay.Year, firstDay.Month, firstDay.Day, 0, 0, 0);

            var nbDays = nbWeeks * CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine;
            _days = new DayOfEmployee[nbDays];

            for (var idDay = 0; idDay < nbDays; idDay++)
            {
                var date = CJoursOuvrablesSuccessifs.GetDayAfterXJoursOuvrables(_firstDay, idDay);
                var day = new DayOfEmployee(employeeKeyId, date);

                var border = new Border();
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new System.Windows.Thickness(0, 0, 1, 0);
                border.Child = day;

                _dockPanel.Children.Add(border);

                _days[idDay] = day;
            }

            UpdateBackgroundColorOfDays();

            Children.Add(_dockPanel);

            MouseEnter += new MouseEventHandler(OnMouseEnter);
            MouseLeave += new MouseEventHandler(OnMouseLeave);
            MouseMove += new MouseEventHandler(OnMouseMove);
            MouseDown += new MouseButtonEventHandler(OnMouseDown);
            MouseUp += new MouseButtonEventHandler(OnMouseUp);

            _hatchedBrush = CHatchedBrushCreator.Create();
        }

        public void OnFirstDayChanged(DateTime firstDay)
        {
            _firstDay = firstDay;
            for(var i = 0; i < NbOfDays; i++)
            {
                _days[i].OnDateChanged(CJoursOuvrablesSuccessifs.GetDayAfterXJoursOuvrables(_firstDay, i));
            }

            UpdateBackgroundColorOfDays();
            UpdateDisplayOfTasks();
        }

        public void OnSizeChanged(double widthOfWeek)
        {
            if (Width == _lastWidth && Height == _lastHeight && _lastWidthOfWeek == widthOfWeek)
            {
                return;
            }

            _widthOfWeek = widthOfWeek;

            _dockPanel.Width = Width;
            _dockPanel.Height = Height;
            foreach (var children in _dockPanel.Children)
            {
                var border = (Border)children;
                border.Width = _widthOfWeek / CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine;
                border.Height = Height;

                var day = (DayOfEmployee)(border.Child);
                day.Width = border.Width;
                day.Height = border.Height;

                day.OnSizeChanged();
            }

            _lastWidth = Width;
            _lastHeight = Height;
            _lastWidthOfWeek = widthOfWeek;
        }

        public void UpdateDisplayOfTasks()
        {
            var model = Model.Instance;
            //var tasks = new List<ITask>( model.GetTasksOfEmployee(_nameOfEmployee, _firstMonday, _weeks.Length));

            //// *** Mise à jour des tâches UI associées à des tâches valides
            //var listOfTasksUiToRemove = new List<TaskUI>();
            //foreach(var itemTaskUI in _dictionaryOfTaskUI)
            //{
            //    var task = itemTaskUI.Key;
            //    var taskUI = itemTaskUI.Value;
            //    if (true == tasks.Contains(task))
            //    {
            //        UpdateTaskUI(taskUI);
            //        continue;
            //    }

            //    listOfTasksUiToRemove.Add(taskUI);
            //}

            //// *** Suppression des tâches UI qui ne sont plus associées à des tâches valides
            //foreach(var taskUiToRemove in listOfTasksUiToRemove)
            //{
            //    RemoveTaskUI(taskUiToRemove);
            //}

            //// *** Création des tâches UI manquantes
            //foreach(var task in tasks)
            //{
            //    if (false == _dictionaryOfTaskUI.ContainsKey(task))
            //    {
            //        var taskUI = CreateNewTaskUI(task);
            //        UpdateTaskUI(taskUI);
            //    }
            //}

            while (Children.Count > 1)
            {
                Children.RemoveAt(1);
            }

            _dictionaryOfTaskUI.Clear();

            var tasks = model.GetTasksOfEmployee(_employeeKeyId, _firstDay, Constantes._nbWeeksMaxDisplayable);

            foreach (var task in tasks)
            {
                UpdateTaskUI(task);
            }
        }

        public void UpdateVisual()
        {
            UpdateBackgroundColorOfDays();
            UpdateDisplayOfTasks();
        }

        public void OnMouseUpOnTaskUI(TaskUI taskUI, MouseButtonEventArgs e)
        {
            OnMouseUp(null, e);
        }

        public CEditTaskController EditTaskController => _editTaskController;

        public void ResetDisplayOfCalendar()
        {
            _calendar.ResetDisplayOfCalendar();
        }

        public void UpdateListeOfJoursFériés(IJourFérié[] joursFériés)
        {
            foreach (var day in _days)
            {
                day.UpdateListeOfJoursFériés(joursFériés);
            }
        }

        // *** RESTRICTED ***********************

        private Brush _hatchedBrush;

        private double _lastWidth;

        private double _lastHeight;

        private double _lastWidthOfWeek;

        private ICalendarDisplayerForChildrens _calendar;

        private double _widthOfWeek;

        private EMouseMode _mouseMode = EMouseMode.OUT;

        private DockPanel _dockPanel;

        private DayOfEmployee[] _days;

        private DateTime _firstDay;

        private int NbOfDays => _days.Length;

        private IDictionary<ITask, TaskUI> _dictionaryOfTaskUI = new Dictionary<ITask, TaskUI>();

        private DatasForDraggingLimitOfAnOldTask _datasForDraggingLimitOfAnOldTask;

        private DatasForDraggingANewTask _datasForDraggingANewTask;

        private DatasForMovingAnOldTask _datasForMovingAnOldTask;

        private CEditTaskController _editTaskController;

        private long _employeeKeyId;

        private double TimeElementWidth => _days[0].L_TimeElement;

        private double DayMargin => _days[0].L_margin;

        private CTask[] TasksOverlapping(CTask[] tasks, CTask taskToAnalyse)
        {
            var output = new List<CTask>();
            foreach(var task in tasks)
            {
                if (task == taskToAnalyse)
                {
                    continue;
                }
                if (task.BeginsAt > taskToAnalyse.EndsAt)
                {
                    break; // Comme les tâches sont rangés chronologiquement, cette condition suffit à clore l'analyse des chevauchements de tâches
                }
                if (true == AreTasksOverlapping(task, taskToAnalyse))
                {
                    output.Add(task);
                }
            }

            return output.ToArray();
        }

        private bool AreTasksOverlapping(CTask A, CTask B)
        {
            if (A.BeginsAt >= B.BeginsAt && A.BeginsAt <= B.EndsAt)
            {
                return true;
            }
            if (A.EndsAt >= B.BeginsAt && A.EndsAt <= B.EndsAt)
            {
                return true;
            }
            if (B.BeginsAt >= A.BeginsAt && B.BeginsAt <= A.EndsAt)
            {
                return true;
            }
            if (B.EndsAt >= A.BeginsAt && B.EndsAt <= A.EndsAt)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// L'élément qui est survolé par la souris
        /// </summary>
        private TimeElementOfEmployee _highlightedElementWhenMouseFlyingFreely;

        /// <summary>
        /// Fournir le moment de l'absisse en paramètre dans le repère du calendrier de l'employé
        /// </summary>
        private DateTime GetDateTimefromX(double x)
        {
            if (x < 0)
            {
                x = 0;
            }
            var widthOfDay = _widthOfWeek / CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine;
            var nbDays = (int)(x / widthOfDay);
            var x_in_lastDay = x - nbDays * widthOfDay;
            var timeInHoursInLastDay = Constantes._firstHourOfTheDay
                + Constantes._timeElementDuration_h * (x_in_lastDay - DayMargin) / TimeElementWidth;
            var nbHours = (int)timeInHoursInLastDay;
            var nbMinutes = (int)((timeInHoursInLastDay - nbHours) * 60.0);
            var dateTime = CJoursOuvrablesSuccessifs.GetDayAfterXJoursOuvrables(_firstDay, nbDays) + new TimeSpan(nbHours, nbMinutes, 0);

            return dateTime;
        }

        /// <summary>
        /// Fournir le moment de l'absisse en paramètre dans le repère du calendrier de l'employé
        /// </summary>
        private DateTime GetDateTimefromMouse()
        {
            var xMouse = Mouse.GetPosition(this).X;
            return GetDateTimefromX(xMouse);
        }

        private TimeElementOfEmployee GetElementUnderMouse()
        {
            var xMouse = Mouse.GetPosition(this).X;
            var elementUnderMouse = GetTimeElementFromDate(GetDateTimefromX(xMouse));
            return elementUnderMouse;
        }

        private bool GetTaskUnderMouse(out ITask task)
        {
            var mousePos = Mouse.GetPosition(this);
            foreach (var taskUI in _dictionaryOfTaskUI.Values)
            {
                var label = (Label)taskUI;
                var left = ((TranslateTransform)label.RenderTransform).X;
                var right = ((TranslateTransform)label.RenderTransform).X + label.Width;
                var top = ((TranslateTransform)label.RenderTransform).Y;
                var bottom = ((TranslateTransform)label.RenderTransform).Y + label.Height;

                if (mousePos.X >= left && mousePos.X <= right && mousePos.Y >= top && mousePos.Y <= bottom)
                {
                    task = taskUI._task;
                    return true;
                }
            }

            task = null;
            return false;
        }

        /// <summary>
        /// Fournir l'absisse du moment en paramètre dans le repère du calendrier de l'employé
        /// </summary>
        private double GetXfromDateTime(DateTime dateTime)
        {
            if (dateTime < _firstDay)
            {
                return DayMargin;
            }

            var nbDays = CJoursOuvrablesSuccessifs.GetNbJoursOuvrableEntre(_firstDay, dateTime);
            var x = (_widthOfWeek / CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine) * nbDays
                + DayMargin
                + TimeElementWidth * (dateTime.Hour - Constantes._firstHourOfTheDay) / Constantes._timeElementDuration_h;
            return x;
        }

        private double Marge => Height / 20;

        private double HeightOfTask => (Height - 2 * Marge) / 2.5;

        private void UpdateTaskUI(ITask task)
        {
            var margin = Marge;
            var heightOfTask = HeightOfTask;

            var Xbeginning = GetXfromDateTime(task.BeginsAt);
            var Xending = GetXfromDateTime(task.EndsAt);

            var chantier = Model.Instance.GetChantier(task.ChantierKeyId);
            if (null == chantier)
            {
                // Une anomalie est survenue
                return;
            }

            var toolTips = chantier.ToString();

            var taskRectangleUI = new TaskUI(this)
            {
                _task = task,
                ToolTip = toolTips,
                Height = HeightOfTask,
                Width = Xending - Xbeginning,
                Background = (chantier.Statut == EStatutChantier.CLOS) ? _hatchedBrush : new SolidColorBrush(Model.GetBackgroundColor(chantier.CouleurId)),
            };
            var taskTitleUI = new TaskUI(this)
            {
                Content = (chantier.Statut == EStatutChantier.A_CONFIRMER) ? chantier.Name + " ???" : chantier.Name,
                ToolTip = toolTips,
                Height = HeightOfTask,
                Width = 200,
                FontSize = Math.Min(20, heightOfTask * 0.5),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Model.GetTextColor(chantier.CouleurId)),
                HorizontalContentAlignment = HorizontalAlignment.Left,
            };
            Children.Add(taskRectangleUI);
            Children.Add(taskTitleUI);
            _dictionaryOfTaskUI.Add(task, taskRectangleUI);

            taskRectangleUI.RenderTransform = new TranslateTransform
            {
                X = Xbeginning,
                Y = margin + task.AltitudeUI * (Height - 2 * margin - heightOfTask)
            };
            taskTitleUI.RenderTransform = new TranslateTransform
            {
                X = Xbeginning,
                Y = margin + task.AltitudeUI * (Height - 2 * margin - heightOfTask)
            };
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            _mouseMode = EMouseMode.FLYING_FREELY;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _mouseMode = EMouseMode.OUT;
            InformLastHighlightedElementThatMouseLeaved();
            SwitchOffAllSelectedElements();
            _datasForDraggingANewTask = null;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            switch(_mouseMode)
            {
                case EMouseMode.FLYING_FREELY:
                    {
                        if (false == ChangeMousePointerWhenReadyToModifyLimitsOfAnOldTask()
                            && false == ChangeMousePointerWhenReadyToMoveAnOldTask())
                        {
                            Mouse.OverrideCursor = null;
                        }

                        HighlightTheElementUnderMouseWhenFlyingFreely();

                        break;
                    }
                case EMouseMode.DRAGGING_FOR_NEW_TASK:
                    {
                        var elementUnderMouse = GetElementUnderMouse();
                        if (null == elementUnderMouse)
                        {
                            break;
                        }

                        if (null == _datasForDraggingANewTask)
                        {
                            _datasForDraggingANewTask = new DatasForDraggingANewTask()
                            {
                                _beginDragging = GetDateTimefromMouse(),
                                _endDragging = GetDateTimefromMouse()
                            };
                        }
                        else
                        {
                            var old_datasForDraggingANewTask = _datasForDraggingANewTask;
                            _datasForDraggingANewTask = new DatasForDraggingANewTask()
                            {
                                _beginDragging = old_datasForDraggingANewTask._beginDragging,
                                _endDragging = GetDateTimefromMouse()
                            };

                            var listOfOldElementsToSwitchOff
                                = ArrayOfElementsBetweenToDateTimes(old_datasForDraggingANewTask._beginDragging,
                                    old_datasForDraggingANewTask._endDragging);
                            foreach(var element in listOfOldElementsToSwitchOff)
                            {
                                element.IsSelected = false;
                            }

                            var listOfOldElementsToHighlight
                                = ArrayOfElementsBetweenToDateTimes(_datasForDraggingANewTask._beginDragging,
                                    _datasForDraggingANewTask._endDragging);
                            foreach (var element in listOfOldElementsToHighlight)
                            {
                                element.IsSelected = true;
                            }
                        }
                        break;
                    }
                case EMouseMode.DRAGGING_LIMIT_OF_AN_OLD_TASK:
                    {
                        var elementUnderMouse = GetElementUnderMouse();
                        if (null == elementUnderMouse)
                        {
                            break;
                        }

                        switch (_datasForDraggingLimitOfAnOldTask._typeLimite)
                        {
                            case DatasForDraggingLimitOfAnOldTask.ELimit.BEGINNING:
                                _datasForDraggingLimitOfAnOldTask.UpdateNewLimit(elementUnderMouse.BeginsAt);
                                break;
                            case DatasForDraggingLimitOfAnOldTask.ELimit.ENDING:
                                _datasForDraggingLimitOfAnOldTask.UpdateNewLimit(elementUnderMouse.EndsAt);
                                break;
                        }

                        if (_datasForDraggingLimitOfAnOldTask.IsValid())
                        {
                            _calendar.ResetDisplayOfCalendar();
                        }
                        break;
                    }
                case EMouseMode.MOVING_TASK_VERTICALY:
                    {
                        var mousePos = Mouse.GetPosition(this);
                        var deltaMousePos = mousePos - _datasForMovingAnOldTask._initialMousePosition;
                        var dy = deltaMousePos.Y;
                        var heightOfTask = HeightOfTask;
                        var newAltitudeUI = _datasForMovingAnOldTask._initialTaskAltitudeUI + dy / (Height - 2 * Marge - heightOfTask);
                        if (newAltitudeUI < 0)
                        {
                            newAltitudeUI = 0;
                        }
                        else if (newAltitudeUI > 1)
                        {
                            newAltitudeUI = 1;
                        }
                        _datasForMovingAnOldTask._task._altitudeUI = newAltitudeUI;
                        _calendar.ResetDisplayOfCalendar();
                        break;
                    }
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && true == GetTaskUnderMouse(out ITask taskDoubleClicked))
            {
                _editTaskController.OnModifyTask(taskDoubleClicked);
            }

            InformLastHighlightedElementThatMouseLeaved();

            if (null != _datasForDraggingLimitOfAnOldTask)
            {
                _mouseMode = EMouseMode.DRAGGING_LIMIT_OF_AN_OLD_TASK;
            }
            else if (null != _datasForMovingAnOldTask)
            {
                _mouseMode = EMouseMode.MOVING_TASK_VERTICALY;
            }
            else
            {
                _mouseMode = EMouseMode.DRAGGING_FOR_NEW_TASK;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                if (false == GetTaskUnderMouse(out ITask taskClicked))
                {
                    return;
                }

                new CContextMenuOnTaskController(this).ShowContextMenuOnTask(taskClicked);
                return;
            }

            switch (_mouseMode)
            {
                case EMouseMode.FLYING_FREELY:
                    {
                        HighlightTheElementUnderMouseWhenFlyingFreely();
                        break;
                    }
                case EMouseMode.DRAGGING_FOR_NEW_TASK:
                    {
                        // On éteint tous les éléments "éclairés" par la sélection
                        SwitchOffAllSelectedElements();
                        if (null == _datasForDraggingANewTask)
                        {
                            break;
                        }

                        var timeSlotBeginsAt = _datasForDraggingANewTask._beginDragging;
                        var timeSlotEndsAt = _datasForDraggingANewTask._endDragging;
                        if (timeSlotBeginsAt > timeSlotEndsAt)
                        {
                            timeSlotBeginsAt = _datasForDraggingANewTask._endDragging;
                            timeSlotEndsAt = _datasForDraggingANewTask._beginDragging;
                        }
                        if (timeSlotBeginsAt < timeSlotEndsAt)
                        {
                            var elementAtBeginning = GetTimeElementFromDate(timeSlotBeginsAt);
                            var elementAtEnding = GetTimeElementFromDate(timeSlotEndsAt);

                            var newTask = new CTask(-1/*KeyId*/)
                            {
                                _beginsAt = elementAtBeginning.BeginsAt,
                                _endsAt = elementAtEnding.EndsAt,
                                _chantierKeyId = -1,
                                _altitudeUI = 0
                            };
                            newTask._employeeKeyId = this._employeeKeyId;

                            _editTaskController.OnCreateTask(newTask);
                            UpdateDisplayOfTasks();
                        }

                        _datasForDraggingANewTask = null;
                        break;
                    }
                case EMouseMode.DRAGGING_LIMIT_OF_AN_OLD_TASK:
                    {
                        _datasForDraggingLimitOfAnOldTask.OnEndDraggingAndValidModifications();
                        UpdateDisplayOfTasks();
                        break;
                    }
                case EMouseMode.MOVING_TASK_VERTICALY:
                    {
                        _datasForMovingAnOldTask.OnEndMovingAndValidModifications();
                        UpdateDisplayOfTasks();
                        break;
                    }
            }

            _datasForDraggingLimitOfAnOldTask?.Release();
            _datasForMovingAnOldTask?.Release();
            _datasForDraggingLimitOfAnOldTask = null;
            _datasForMovingAnOldTask = null;
            _datasForDraggingANewTask = null;
            _mouseMode = EMouseMode.FLYING_FREELY;
        }

        private void SwitchOffAllSelectedElements()
        {
            if (null == _datasForDraggingANewTask)
            {
                return;
            }

            var listOfOldElementsToSwitchOff
               = ArrayOfElementsBetweenToDateTimes(_datasForDraggingANewTask._beginDragging,
                   _datasForDraggingANewTask._endDragging);
            foreach (var element in listOfOldElementsToSwitchOff)
            {
                element.IsSelected = false;
            }
        }

        private TimeElementOfEmployee[] ArrayOfElementsBetweenToDateTimes(DateTime a, DateTime b)
        {
            var list = new List<TimeElementOfEmployee>();

            var limiteInf = a;
            var limiteSup = b;
            if (b < a)
            {
                limiteInf = b;
                limiteSup = a;
            }

            for (var i = 0; i < _days.Length; i++)
            {
                list.AddRange(_days[i].ArrayOfElementsBetweenTwoDateTimes(a, b));
            }

            return list.ToArray();
        }

        private void HighlightTheElementUnderMouseWhenFlyingFreely()
        {
            var dateTimeOfMouse = GetDateTimefromMouse();
            if (dateTimeOfMouse.Hour > 10 && dateTimeOfMouse.Hour < 16)
            {
                //Debug.WriteLine("Under mouse : " + dateTimeOfMouse);
            }
            var elementUnderMouse = GetTimeElementFromDate(dateTimeOfMouse);
            if (elementUnderMouse != _highlightedElementWhenMouseFlyingFreely)
            {
                InformLastHighlightedElementThatMouseLeaved();
                elementUnderMouse?.OnOverringByMouseBegins();
                _highlightedElementWhenMouseFlyingFreely = elementUnderMouse;
            }
        }

        private void InformLastHighlightedElementThatMouseLeaved()
        {
            _highlightedElementWhenMouseFlyingFreely?.OnOverringByMouseEnds();
            _highlightedElementWhenMouseFlyingFreely = null;
        }

        private bool ChangeMousePointerWhenReadyToModifyLimitsOfAnOldTask()
        {
            bool isMatch = false;
            _datasForDraggingLimitOfAnOldTask?.Release();
            _datasForDraggingLimitOfAnOldTask = null;
            foreach (var taskUI in _dictionaryOfTaskUI.Values)
            {
                var label = (Label)taskUI;
                var dx_left = Math.Abs(((TranslateTransform)label.RenderTransform).X - Mouse.GetPosition(this).X);
                var dx_right = Math.Abs(((TranslateTransform)label.RenderTransform).X + label.Width - Mouse.GetPosition(this).X);
                try
                {
                    if (dx_left < 5)
                    {
                        _datasForDraggingLimitOfAnOldTask = new DatasForDraggingLimitOfAnOldTask(taskUI,
                            /*_typeLimit*/DatasForDraggingLimitOfAnOldTask.ELimit.BEGINNING);
                        isMatch = true;
                    }
                    else if (dx_right < 5)
                    {
                        _datasForDraggingLimitOfAnOldTask = new DatasForDraggingLimitOfAnOldTask(taskUI,
                            /*_typeLimit*/DatasForDraggingLimitOfAnOldTask.ELimit.ENDING);
                        isMatch = true;
                    }
                }
                catch
                {
                    break;
                }

                if (isMatch)
                {
                    Mouse.OverrideCursor = Cursors.ScrollWE;
                    break;
                }
            }

            return isMatch;
        }

        private bool ChangeMousePointerWhenReadyToMoveAnOldTask()
        {
            var mousePos = Mouse.GetPosition(this);
            bool isMatch = false;
            _datasForMovingAnOldTask?.Release();
            _datasForMovingAnOldTask = null;
            foreach (var taskUI in _dictionaryOfTaskUI.Values)
            {
                var label = (Label)taskUI;
                var isMouseInsideTask = 
                    mousePos.X > ((TranslateTransform)label.RenderTransform).X 
                    && mousePos.X < ((TranslateTransform)label.RenderTransform).X + label.Width
                    && mousePos.Y > ((TranslateTransform)label.RenderTransform).Y
                    && mousePos.Y < ((TranslateTransform)label.RenderTransform).Y + label.Height;
                
                if (false == isMouseInsideTask)
                {
                    continue;
                }

                var dx_left = Math.Abs(((TranslateTransform)label.RenderTransform).X - mousePos.X);
                var dx_right = Math.Abs(((TranslateTransform)label.RenderTransform).X + label.Width - mousePos.X);
                if (dx_left < 5)
                {
                    continue;
                }
                else if (dx_right < 5)
                {
                    continue;
                }

                try
                {
                    _datasForMovingAnOldTask = new DatasForMovingAnOldTask(
                        taskUI,
                        /*_initialMousePosition*/mousePos
                        );
                    isMatch = true;
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                catch
                {
                    // Echec sans doute dû à une interférence entre 2 instance 
                    // de l'agenda
                    return false;
                }
                break;
            }

            return isMatch;
        }

        private TimeElementOfEmployee GetTimeElementFromDate(DateTime date)
        {
            for (var i = 0; i < _days.Length; i++)
            {
                if (_days[i].Date <= date && _days[i].Date + new TimeSpan(1, 0, 0, 0) > date)
                {
                    return _days[i].GetTimeElementFromDate(date);
                }
            }

            return null;
        }
         
        private void UpdateBackgroundColorOfDays()
        {
            var firstDayOfWeek = (int)(_firstDay.DayOfWeek);
            var firstWeekNb  = GetNumberOfWeek(_firstDay);

            var nbOfMondaysSeen = 0;
            for(var i = 0; i < NbOfDays; i++)
            {
                var dayOfWeek = (firstDayOfWeek + i) % CJoursOuvrablesSuccessifs._nbJoursOuvrablesParSemaine;
                if (1 == dayOfWeek && i > 0)
                {
                    nbOfMondaysSeen++;
                }
                var weekNb = firstWeekNb + nbOfMondaysSeen;
                _days[i].SetBackgroundColor(GetBkColorOfWeekFromWeekNumber(weekNb));
            }
        }

        private static int GetNumberOfWeek(DateTime date)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            var cal = dfi.Calendar;
            return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        private static Color GetBkColorOfWeekFromDateOfMonday(DateTime date)
        {
            var nb = GetNumberOfWeek(date);
            return GetBkColorOfWeekFromWeekNumber(nb);
        }

        private static Color GetBkColorOfWeekFromWeekNumber(int nb)
        {
            if (nb % 2 != 0)
            {
                return Constantes._backgroundColorOfDayOfOddWeek;
            }
            else
            {
                return Constantes._backgroundColorOfDayOfEvenWeek;
            }
        }
    }
}
