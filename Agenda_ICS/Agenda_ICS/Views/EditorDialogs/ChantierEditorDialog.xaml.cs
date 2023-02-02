using Agenda_ICS.Views.Calendar;
using NDatasModel;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.Editors
{
    public partial class ChantierEditorDialog : Window
    {
        // *** PUBLIC *********************************

        public enum EMode
        {
            CREATION,
            MODIFICATION
        }

        public ChantierEditorDialog(IDialogWndOwner owner, IChantier chantier)
        {
            _owner = owner;

            InitializeComponent();

            _chantier = new CChantier(
                chantier.KeyId, 
                chantier.Name, 
                chantier.RefDevis,
                chantier.Adresse,
                chantier.CouleurId,
                chantier.Statut,
                chantier.DateAcceptationDevis,
                chantier.DatePrevisionnelleTravaux,
                chantier.NbDHeuresADeuxTechniciens,
                chantier.NbDHeuresAUnTechnicien,
                chantier.PrixDeVenteHT
                );
            _mode = EMode.MODIFICATION;
        }

        public ChantierEditorDialog(IDialogWndOwner owner)
        {
            _owner = owner;

            InitializeComponent();

            _chantier = new CChantier(
                -1, 
                "Nouveau chantier", 
                string.Empty, 
                string.Empty, 
                0, 
                EStatutChantier.A_CONFIRMER,
                string.Empty,
                string.Empty,
                0,
                0,
                0
                );
            _mode = EMode.CREATION;
            this.Title = "Nouveau chantier";
        }

        public CChantier _chantier;

        public readonly EMode _mode;

        // *** RESTRICTED ****************************

        private IDialogWndOwner _owner;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _owner.OnCloseDialog(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NomChantier.Text = _chantier.Name;
            Adresse.Text = _chantier.Adresse;
            RefDevis.Text = _chantier.RefDevis;
            DateAcceptationDevis.Text= _chantier.DateAcceptationDevis;
            DatePrevisionnelleTravaux.Text = _chantier.DatePrevisionnelleTravaux;
            NbDHeuresADeuxTechniciens.Text = _chantier.NbDHeuresADeuxTechniciens > 0 ? _chantier.NbDHeuresADeuxTechniciens.ToString() : string.Empty;
            NbDHeuresAUnTechnicien.Text = _chantier.NbDHeuresAUnTechnicien > 0 ? _chantier.NbDHeuresAUnTechnicien.ToString() : string.Empty;
            PrixDeVenteHT.Text = _chantier.PrixDeVenteHT != 0 ? $"{_chantier.PrixDeVenteHT:0.00}" : string.Empty;

            UpdateSelectedColor(_chantier.CouleurId);

            ListOfStatutListbox.Items.Add("A confirmer");
            ListOfStatutListbox.Items.Add("Confirmé");
            ListOfStatutListbox.Items.Add("Clos");
            ListOfStatutListbox.SelectedIndex = (int)_chantier.Statut;
        }

        private void OnClickModifyColor(object sender, RoutedEventArgs e)
        {
            var couleurId = int.Parse(((Button)e.Source).Uid);

            UpdateSelectedColor(couleurId);
        }

        private void UpdateSelectedColor(int couleurId)
        {
            Couleur_0.Content = string.Empty;
            Couleur_1.Content = string.Empty;
            Couleur_2.Content = string.Empty;
            Couleur_3.Content = string.Empty;
            Couleur_4.Content = string.Empty;
            Couleur_5.Content = string.Empty;

            switch (couleurId)
            {
                case 0:
                    Couleur_0.Content = "X";
                    _chantier._couleurId = 0;
                    break;
                case 1:
                    Couleur_1.Content = "X";
                    _chantier._couleurId = 1;
                    break;
                case 2:
                    Couleur_2.Content = "X";
                    _chantier._couleurId = 2;
                    break;
                case 3:
                    Couleur_3.Content = "X";
                    _chantier._couleurId = 3;
                    break;
                case 4:
                    Couleur_4.Content = "X";
                    _chantier._couleurId = 4;
                    break;
                case 5:
                    Couleur_5.Content = "X";
                    _chantier._couleurId = 5;
                    break;
            }
        }

        private void OnClick_Valider(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomChantier.Text))
            {
                MessageBox.Show("Le nom du chantier n'a pas été renseigné", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _chantier._name = NomChantier.Text;
            _chantier._refDevis = RefDevis.Text;
            _chantier._adresse = Adresse.Text;
            _chantier._statut = (EStatutChantier)(ListOfStatutListbox.SelectedIndex);
            _chantier._dateAcceptationDevis = DateAcceptationDevis.Text;
            _chantier._datePrevisionnelleTravaux = DatePrevisionnelleTravaux.Text;

            if (false == int.TryParse(NbDHeuresADeuxTechniciens.Text.Trim(), out int nbDHeuresADeuxTechniciens))
            {
                if (NbDHeuresADeuxTechniciens.Text == string.Empty)
                {
                    nbDHeuresADeuxTechniciens = 0;
                }
                else
                {
                    MessageBox.Show("Le nombre de techniciens doit être un nombre entier", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            if (nbDHeuresADeuxTechniciens < 0)
            {
                MessageBox.Show("Le nombre de techniciens doit être un nombre entier POSITIF", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _chantier._nbDHeuresADeuxTechniciens = nbDHeuresADeuxTechniciens;

            if (false == int.TryParse(NbDHeuresAUnTechnicien.Text.Trim(), out int nbDHeuresAUnTechnicien))
            {
                if (NbDHeuresAUnTechnicien.Text == string.Empty)
                {
                    nbDHeuresAUnTechnicien = 0;
                }
                else
                {
                    MessageBox.Show("Le nombre d'heures à planifier doit être un nombre entier", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            if (nbDHeuresAUnTechnicien < 0)
            {
                MessageBox.Show("Le nombre d'heures à planifier doit être un nombre entier POSITIF", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _chantier._nbDHeuresAUnTechnicien = nbDHeuresAUnTechnicien;

            if (false == float.TryParse(PrixDeVenteHT.Text.Trim().Replace('.', ','), out float prixDeVenteHT))
            {
                if (PrixDeVenteHT.Text == string.Empty)
                {
                    prixDeVenteHT = 0;
                }
                else
                {
                    MessageBox.Show("Le prix de vente HT doit être un nombre", "Merci de corriger ...", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            _chantier._prixDeVenteHT = prixDeVenteHT;

            if (EMode.MODIFICATION == _mode)
            {
                Model.Instance.ModifyChantier(
                    _chantier.KeyId, 
                    _chantier.Name, 
                    _chantier.RefDevis, 
                    _chantier.Adresse, 
                    _chantier.CouleurId, 
                    _chantier.Statut,
                    _chantier.DateAcceptationDevis,
                    _chantier.DatePrevisionnelleTravaux,
                    _chantier.NbDHeuresADeuxTechniciens,
                    _chantier.NbDHeuresAUnTechnicien,
                    _chantier.PrixDeVenteHT
                    );
            }
            else
            {
                _chantier._keyId = Model.Instance.CreateChantier(
                    _chantier.Name, 
                    _chantier.RefDevis, 
                    _chantier.Adresse, 
                    _chantier.CouleurId, 
                    _chantier.Statut,
                    _chantier.DateAcceptationDevis,
                    _chantier.DatePrevisionnelleTravaux,
                    _chantier.NbDHeuresADeuxTechniciens,
                    _chantier.NbDHeuresAUnTechnicien,
                    _chantier.PrixDeVenteHT
                    );
            }

            this.Close();
        }
    }
}
