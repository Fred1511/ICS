using System.Globalization;
using System.Linq;
using System.Windows;

namespace Agenda_ICS.Views.Synthèse
{
    /// <summary>
    /// Logique d'interaction pour SynthèseDlg.xaml
    /// </summary>
    public partial class SynthèseDlg : Window
    {
        public SynthèseDlg()
        {
            InitializeComponent();

            var chantiers = Model.Instance.GetChantiers().OrderBy(x => x.Name).ToArray();
            var tasks = Model.Instance.GetTasks();
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            float totalPrixDeVenteHT = GetTotalPrixDeVenteHT(chantiers);
            TotalPrixDeVenteHT.Content = string.Format("{0:N} €", totalPrixDeVenteHT);

            float totalHeuresAPlanifier = GetTotalHeuresAPlanifier(chantiers);
            TotalHeuresAPlanifier.Content = string.Format("{0:N}", totalHeuresAPlanifier);

            float totalHeuresPlanifiées = GetTotalHeuresPlanifiées(tasks);
            TotalHeuresPlanifiées.Content = string.Format("{0:N}", totalHeuresPlanifiées);
        }

        private static float GetTotalPrixDeVenteHT(NDatasModel.IChantier[] chantiers)
        {
            float totalPrixDeVenteHT = 0;
            foreach (var chantier in chantiers)
            {
                totalPrixDeVenteHT += chantier.PrixDeVenteHT;
            }

            return totalPrixDeVenteHT;
        }

        private static int GetTotalHeuresAPlanifier(NDatasModel.IChantier[] chantiers)
        {
            int totalHeuresAPlanifier = 0;
            foreach (var chantier in chantiers)
            {
                totalHeuresAPlanifier += chantier.NbDHeuresAPlanifier;
            }

            return totalHeuresAPlanifier;
        }

        private static int GetTotalHeuresPlanifiées(NDatasModel.ITask[] tasks)
        {
            int totalHeuresPlanifiées = 0;
            foreach (var task in tasks)
            {
                totalHeuresPlanifiées += task.GetNbDHeures();
            }

            return totalHeuresPlanifiées;
        }

        private void OnCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
