using Agenda_ICS.Views.Calendar;
using Agenda_ICS.Views.Editors;
using NDatasModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.Configuration.Pages
{
    /// <summary>
    /// Logique d'interaction pour ChantiersConfig.xaml
    /// </summary>
    public partial class ChantiersConfig : UserControl, IDialogWndOwner
    {
        public ChantiersConfig()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateListOfChantiers();
        }

        private void UpdateListOfChantiers(long selectedChantierKeyId = -1)
        {
            Chantier.Items.Clear();
            _chantiers = Model.Instance.GetChantiers();
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

        public void OnCloseDialog(Window wnd)
        {
            if (wnd == _editDialog)
            {
                var chantier = _editDialog._chantier;
                UpdateListOfChantiers(chantier.KeyId);

                _editDialog = null;
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
            if (string.IsNullOrWhiteSpace(Filter.Text))
            {
                Chantier.Items.Clear();
                for (var i = 0; i < _chantiers.Length; i++)
                {
                    Chantier.Items.Add(_chantiers[i]);
                }
                Chantier.SelectedIndex = -1;
                return;
            }

            var filter = Filter.Text.ToLower();

            var filteredChantiers = new List<IChantier>();
            for (var i = 0; i < _chantiers.Length; i++)
            {
                var chantier = _chantiers[i].ToString().ToLower();
                if (chantier.Contains(filter))
                {
                    filteredChantiers.Add(_chantiers[i]);
                }
            }

            Chantier.Items.Clear();
            for (var i = 0; i < filteredChantiers.Count; i++)
            {
                Chantier.Items.Add(filteredChantiers[i]);
            }

            Chantier.SelectedIndex = (filteredChantiers.Count == 1) ? 0 : -1;
        }
    }
}
