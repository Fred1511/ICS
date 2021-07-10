using System;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.Calendar.TimeLine
{
    class CDayCustomMenuItem : MenuItem
    {
        public DateTime _date;
    }

    class CContextMenuOnTimeLineController
    {
        // *** PUBLIC ***************************

        public interface IParent
        {
            void OnStatutFériéChanged();
        }

        public CContextMenuOnTimeLineController(IParent parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
        
        public void ShowContextMenuOnDate(DateTime date)
        {
            _isJourFérié = Model.Instance.IsJourFérié(date);

            var menuModifyDateStatus = new CDayCustomMenuItem();
            menuModifyDateStatus._date = date;
            menuModifyDateStatus.Header = _isJourFérié ? MenuLeJourNEstPlusFérié : MenuLeJourEstFérié;
            menuModifyDateStatus.Click += OnSelectModifyDayStatut;

            var menu = new ContextMenu();
            menu.Items.Add(menuModifyDateStatus);
            menu.IsOpen = true;
        }

        public void OnSelectModifyDayStatut(object sender, RoutedEventArgs e)
        {
            var date = (sender as CDayCustomMenuItem)._date;

            if (_isJourFérié)
            {
                // Le jour n'est plus férié
                Model.Instance.RemoveJourFérié(date);
            }
            else
            {
                // Le jour est férié à présent
                Model.Instance.AddJourFérié(date);
            }

            _parent.OnStatutFériéChanged();
        }

        // *** RESTRICTED *************************

        private bool _isJourFérié;

        private const string MenuLeJourNEstPlusFérié = "N'est plus férié";

        private const string MenuLeJourEstFérié = "Est férié";

        private readonly IParent _parent;
    }
}
