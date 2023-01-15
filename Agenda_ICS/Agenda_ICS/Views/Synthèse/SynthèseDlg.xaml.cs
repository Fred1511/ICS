using NOutils;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

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

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(OnTimer_250ms);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dispatcherTimer.Start();

            UpdateDisplayedDatas();
        }

        private void OnTimer_250ms(object sender, EventArgs e)
        {
            UpdateDisplayedDatas();
        }

        private void UpdateDisplayedDatas()
        {
            var dateDébutPériode = DateDébutPériode.Text;
            if (false == DatesExpert.IsDayValid(dateDébutPériode))
            {
                TotalPrixDeVenteHT.Content = "Date invalide";
                TotalHeuresAPlanifier.Content = "Date invalide";
                TotalHeuresPlanifiées.Content = "Date invalide";
                return;
            }

            var chantiers = Model.Instance.GetChantiers().OrderBy(x => x.Name).ToArray();
            var tasks = Model.Instance.GetTasks();
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            float totalPrixDeVenteHT = GetTotalPrixDeVenteHT(chantiers, dateDébutPériode);
            TotalPrixDeVenteHT.Content = string.Format("{0:N} €", totalPrixDeVenteHT);

            float totalHeuresAPlanifier = GetTotalHeuresAPlanifier(chantiers);
            TotalHeuresAPlanifier.Content = string.Format("{0:N}", totalHeuresAPlanifier);

            float totalHeuresPlanifiées = GetTotalHeuresPlanifiées(tasks);
            TotalHeuresPlanifiées.Content = string.Format("{0:N}", totalHeuresPlanifiées);
        }

        private static float GetTotalPrixDeVenteHT(NDatasModel.IChantier[] chantiers, string dateDébutPériode)
        {
            float totalPrixDeVenteHT = 0;
            dateDébutPériode = DatesExpert.FromFrenchDateToStandardDate(dateDébutPériode);
            foreach (var chantier in chantiers)
            {
                if (chantier.DateAcceptationDevis != string.Empty)
                {
                    var dateAcceptationDevis = DatesExpert.FromFrenchDateToStandardDate(chantier.DateAcceptationDevis);
                    if (string.Compare(dateAcceptationDevis, dateDébutPériode) < 0)
                    {
                        // ce chantier est hors période
                        continue;
                    }
                }
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
