using NOutils;
using System.Windows;
using System.Windows.Media;

namespace Agenda_ICS.Views
{
    public partial class Licence : Window
    {
        public Licence(LicencesExpert.EResult mode, string msg, string shaCode)
        {
            InitializeComponent();

            if (mode == LicencesExpert.EResult.LICENCE_NOT_FOUND_BUT_OK)
            {
                MainLabel.Content = "AVERTISSEMENT";
                MainLabel.Foreground = Brushes.Coral;
            }
            else
            {
                MainLabel.Content = "ERREUR";
                MainLabel.Foreground = Brushes.Red;
            }

            Message.Text = msg;
            CodeSha.Content = shaCode;
        }

        private void OnClick_Fermer(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClick_CopierCode(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText((string)CodeSha.Content);
        }
    }
}
