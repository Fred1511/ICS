using NDatasModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.EditorDialogs
{
    /// <summary>
    /// Logique d'interaction pour ImportFromBatigest.xaml
    /// </summary>
    public partial class ImportFromBatigest : Window
    {
        // *** PUBLIC **************************

        public interface IParent
        {
            void OnImportChantier(long keyIdOfImportedChantier);
        }

        public ImportFromBatigest(IParent parent)
        {
            InitializeComponent();

            _chantiersBatigest = Model.Instance.GetBatigestChantiers();

            UpdateChantiersListbox();
            this._parent = parent;
        }

        // *** RESTRICTED **********************

        private IChantier[] _chantiersBatigest;
        private IChantier[] _displayedChantiers;
        private readonly IParent _parent;

        private void UpdateChantiersListbox()
        {
            var filtre = Filter.Text.Trim().ToLower();

            Chantiers.Items.Clear();

            var orderedChantiers = GetOrderedChantiers(); // tous les chantiers classés par Nom ou par n° de devis
            var displayedChantiers = new List<IChantier>();
            foreach (var chantier in orderedChantiers)
            {
                var libellé = ChantierToString(chantier);
                if (filtre == string.Empty || libellé.ToLower().Contains(filtre))
                {
                    Chantiers.Items.Add(libellé);
                    displayedChantiers.Add(chantier);
                }
            }

            _displayedChantiers = displayedChantiers.ToArray();
        }

        private IChantier[] GetOrderedChantiers()
        {
            if (ClassementNom_radioBtn.IsChecked == true)
            {
                return _chantiersBatigest.OrderBy(x => x.Name).ToArray();
            }
            else
            {
                return _chantiersBatigest.OrderBy(x => x.RefDevis).ToArray();
            }
        }

        private string ChantierToString(IChantier chantier)
        {

            if (ClassementNom_radioBtn.IsChecked == true)
            {
                return $"{chantier.Name} - Devis n°{chantier.RefDevis} -  Prix : {chantier.PrixDeVenteHT:0.0} € -  Adresse : {chantier.Adresse}";
            }
            else
            {
                return $"Devis n°{chantier.RefDevis} - {chantier.Name} -  Prix : {chantier.PrixDeVenteHT:0.0} € - Adresse : {chantier.Adresse}";
            }
        }

        private void ImportChantierClick(object sender, RoutedEventArgs e)
        {
            if (Chantiers.SelectedIndex < 0)
            {
                return;
            }

            var chantier = _displayedChantiers[Chantiers.SelectedIndex];
            var idChantier = Model.Instance.CreateChantier(
                chantier.Name,
                chantier.RefDevis,
                chantier.Adresse,
                0,
                EStatutChantier.A_CONFIRMER,
                string.Empty,
                string.Empty,
                0,
                0,
                chantier.PrixDeVenteHT
                );

            _parent.OnImportChantier(idChantier);
        }

        private void ClassementRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateChantiersListbox();
        }

        private void OnFilterChantierChanged(object sender, TextChangedEventArgs e)
        {
            UpdateChantiersListbox();
        }
    }
}
