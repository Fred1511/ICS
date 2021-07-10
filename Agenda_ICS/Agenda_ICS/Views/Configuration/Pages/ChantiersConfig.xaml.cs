using Agenda_ICS.Views.Calendar;
using Agenda_ICS.Views.Editors;
using NDatasModel;
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
            ListOfChantiers.Items.Clear();
            var listOfChantiers = Model.Instance.GetChantiers();
            var index = 0;
            foreach (var chantier in listOfChantiers)
            {
                ListOfChantiers.Items.Add(chantier);

                if (chantier.KeyId == selectedChantierKeyId)
                {
                    ListOfChantiers.SelectedIndex = index;
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

        private void OnClick_RemoveChantier(object sender, RoutedEventArgs e)
        {

        }

        private void OnClick_ModifyChantier(object sender, RoutedEventArgs e)
        {
            var selectedChantier = (IChantier)ListOfChantiers.SelectedItem;
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
    }
}
