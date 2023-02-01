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

            float totalHeuresAPlanifier = GetTotalHeuresAPlanifier(chantiers, dateDébutPériode);
            TotalHeuresAPlanifier.Content = string.Format("{0:N}", totalHeuresAPlanifier);

            float totalHeuresPlanifiées = GetTotalHeuresPlanifiées(tasks, dateDébutPériode);
            TotalHeuresPlanifiées.Content = string.Format("{0:N}", totalHeuresPlanifiées);
        }

        private static float GetTotalPrixDeVenteHT(NDatasModel.IChantier[] chantiers, string dateDébutPériode)
        {
            float totalPrixDeVenteHT = 0;
            dateDébutPériode = DatesExpert.FromFrenchDateToStandardDate(dateDébutPériode);
            foreach (var chantier in chantiers)
            {
                if (false == IsChantierInPériode(chantier, dateDébutPériode))
                {
                    continue;
                }

                totalPrixDeVenteHT += chantier.PrixDeVenteHT;
            }

            return totalPrixDeVenteHT;
        }

        private static int GetTotalHeuresAPlanifier(NDatasModel.IChantier[] chantiers, string dateDébutPériode)
        {
            int totalHeuresAPlanifier = 0;
            dateDébutPériode = DatesExpert.FromFrenchDateToStandardDate(dateDébutPériode);
            foreach (var chantier in chantiers)
            {
                if (false == IsChantierInPériode(chantier, dateDébutPériode))
                {
                    continue;
                }

                totalHeuresAPlanifier += chantier.NbDHeuresAPlanifier;
            }

            return totalHeuresAPlanifier;
        }

        private static int GetTotalHeuresPlanifiées(NDatasModel.ITask[] tasks, string dateDébutPériode)
        {
            int totalHeuresPlanifiées = 0;
            foreach (var task in tasks)
            {
                var chantierKeyId = task.ChantierKeyId;
                var chantier = Model.Instance.GetChantier(chantierKeyId);
                if (false == IsChantierInPériode(chantier, dateDébutPériode))
                {
                    continue;
                }

                totalHeuresPlanifiées += task.GetNbDHeures();
            }

            return totalHeuresPlanifiées;
        }

        private static bool IsChantierInPériode(NDatasModel.IChantier chantier, string dateDébutPériode)
        {
            if (chantier.DateAcceptationDevis != string.Empty)
            {
                var dateAcceptationDevis = DatesExpert.FromFrenchDateToStandardDate(chantier.DateAcceptationDevis);
                return (string.Compare(dateAcceptationDevis, dateDébutPériode) < 0);
            }

            return true;
        }

        private void OnCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DateDébutPériode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}
