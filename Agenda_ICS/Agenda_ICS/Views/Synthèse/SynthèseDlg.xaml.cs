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
            var chantiers = Model.Instance.GetChantiers().OrderBy(x => x.Name).ToArray();
            var tasks = Model.Instance.GetTasks();
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            float totalPrixDeVenteHT = GetTotalPrixDeVenteHT(chantiers);
            TotalPrixDeVenteHT.Content = string.Format("{0:N} €", totalPrixDeVenteHT);

            float totalHeuresAPlanifier = GetTotalHeuresAPlanifier(chantiers);
            TotalHeuresAPlanifier.Content = string.Format("{0:N0} h", totalHeuresAPlanifier);

            float totalHeuresPlanifiées = GetTotalHeuresPlanifiées(tasks);
            TotalHeuresPlanifiées.Content = string.Format("{0:N0} h", totalHeuresPlanifiées);
        }

        private static float GetTotalPrixDeVenteHT(NDatasModel.IChantier[] chantiers)
        {
            float totalPrixDeVenteHT = 0;
            foreach (var chantier in chantiers)
            {
                if (false == IsChantierInPériode(chantier))
                {
                    continue;
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
                if (false == IsChantierInPériode(chantier))
                {
                    continue;
                }

                totalHeuresAPlanifier += chantier.NbDHeuresAPlanifier;
            }

            return totalHeuresAPlanifier;
        }

        private static int GetTotalHeuresPlanifiées(NDatasModel.ITask[] tasks)
        {
            int totalHeuresPlanifiées = 0;
            foreach (var task in tasks)
            {
                var chantierKeyId = task.ChantierKeyId;
                var chantier = Model.Instance.GetChantier(chantierKeyId);
                if (false == IsChantierInPériode(chantier))
                {
                    continue;
                }

                totalHeuresPlanifiées += task.GetNbDHeures();
            }

            return totalHeuresPlanifiées;
        }

        private static bool IsChantierInPériode(NDatasModel.IChantier chantier)
        {
            return chantier.Statut != NDatasModel.EStatutChantier.CLOS;
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
