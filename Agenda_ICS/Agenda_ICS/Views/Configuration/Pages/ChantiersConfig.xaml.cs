using Agenda_ICS.Views.Calendar;
using Agenda_ICS.Views.EditorDialogs;
using Agenda_ICS.Views.Editors;
using NDatasModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.Configuration.Pages
{
    /// <summary>
    /// Logique d'interaction pour ChantiersConfig.xaml
    /// </summary>
    public partial class ChantiersConfig : UserControl, IDialogWndOwner, ImportFromBatigest.IParent
    {
        // *** PUBLIC ****************************

        public enum EFiltre
        {
            TOUS,
            PLANIFIéS,
            FACTURéS,
            A_PLANIFIER
        }

        public ChantiersConfig()
        {
            InitializeComponent();
        }

        public void OnCloseDialog(Window wnd)
        {
            if (wnd == _editDialog)
            {
                var chantier = _editDialog._chantier;
                UpdateListOfChantiers(chantier.KeyId);

                _editDialog = null;
            }
        }

        public void OnImportChantier(long keyIdOfImportedChantier)
        {
            UpdateListOfChantiers(keyIdOfImportedChantier);
        }

        // *** RESTRICTED ************************

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateListOfChantiers();
        }

        private void UpdateListOfChantiers(long selectedChantierKeyId = -1)
        {
            Chantier.Items.Clear();
            _chantiers = Model.Instance.GetChantiers().OrderBy(x => x.Name).ToArray();
            var index = 0;
            foreach (var chantier in _chantiers)
            {
                Chantier.Items.Add(chantier);

                if (chantier.KeyId == selectedChantierKeyId)
                {
                    Chantier.SelectedIndex = index;
                }

                index++;
            }
        }

        ChantierEditorDialog _editDialog;

        private IChantier[] _chantiers;

        private void OnClick_RemoveChantier(object sender, RoutedEventArgs e)
        {

        }

        private void OnClick_ModifyChantier(object sender, RoutedEventArgs e)
        {
            var selectedChantier = (IChantier)Chantier.SelectedItem;
            if (null == selectedChantier)
            {
                MessageBox.Show("Veuillez sélectionner un chantier à modifier", "Impossible", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _editDialog = new ChantierEditorDialog(this, selectedChantier);
            _editDialog.ShowDialog();
        }

        private void OnClick_CreateChantier(object sender, RoutedEventArgs e)
        {
            _editDialog = new ChantierEditorDialog(this);
            _editDialog.ShowDialog();
        }

        private void OnFilterChantierChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListOfChantiersDisplayed();
        }

        private void FiltreRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateListOfChantiersDisplayed();
        }

        private void UpdateListOfChantiersDisplayed()
        {
            var isChantierNameContainsKeyword = false == string.IsNullOrWhiteSpace(Filter.Text);

            var filter = Filter.Text.ToLower();

            var tasks = Model.Instance.GetTasks();

            var filteredChantiers = new List<IChantier>();
            for (var i = 0; i < _chantiers.Length; i++)
            {
                var chantier = _chantiers[i].ToString().ToLower();
                if (isChantierNameContainsKeyword && false == chantier.Contains(filter))
                {
                    continue;
                }

                var filtre = SelectedFiltre();
                var isPlanifié = IsChantierPlanifié(_chantiers[i], tasks);
                if (filtre == EFiltre.TOUS
                    || (filtre == EFiltre.PLANIFIéS && isPlanifié)
                    || (filtre == EFiltre.FACTURéS && _chantiers[i].Statut == EStatutChantier.CLOS)
                    || (filtre == EFiltre.A_PLANIFIER && false == isPlanifié)
                   )
                {
                    filteredChantiers.Add(_chantiers[i]);
                }
            }

            filteredChantiers = filteredChantiers.OrderBy(x => x.Name).ToList();

            Chantier.Items.Clear();
            for (var i = 0; i < filteredChantiers.Count; i++)
            {
                Chantier.Items.Add(filteredChantiers[i]);
            }

            Chantier.SelectedIndex = (filteredChantiers.Count == 1) ? 0 : -1;
        }

        private bool IsChantierPlanifié(IChantier chantier, ITask[] allTasks)
        {
            foreach (var task in allTasks)
            {
                if (task.ChantierKeyId == chantier.KeyId)
                {
                    return true;
                }
            }

            return false;
        }

        private EFiltre SelectedFiltre()
        {
            if ((bool)Filtre_Tous_radioBtn.IsChecked)
            {
                return EFiltre.TOUS;
            }
            if ((bool)Filtre_Planifiés_radioBtn.IsChecked)
            {
                return EFiltre.PLANIFIéS;
            }
            if ((bool)Filtre_Facturés_radioBtn.IsChecked)
            {
                return EFiltre.FACTURéS;
            }
            if ((bool)Filtre_A_Planifier_radioBtn.IsChecked)
            {
                return EFiltre.A_PLANIFIER;
            }

            throw new System.Exception();
        }

        private void OnClick_ImporterDeBatigest(object sender, RoutedEventArgs e)
        {
            new ImportFromBatigest(this).Show();
        }
    }
}
