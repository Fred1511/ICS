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
                if (false == IsChantierNonClos(chantier))
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
                if (chantier.Name.StartsWith("Test"))
                {
                    bool t = true;
                }

                if (false == IsChantierNonClos(chantier))
                {
                    continue;
                }

                var nbHeuresPlanifiées = GetNbHeuresPlanifiéesParChantier(chantier);
                var nbHeuresAPlanifier = chantier.NbDHeuresAUnTechnicien + 2 * chantier.NbDHeuresADeuxTechniciens;
                var delta_h = nbHeuresAPlanifier - nbHeuresPlanifiées;
                // s'il y a + d'heures planifiées que prévues, on ne comptabilise pas en négatif
                totalHeuresAPlanifier += delta_h > 0 ? delta_h : 0;
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
                if (false == IsChantierNonClos(chantier))
                {
                    continue;
                }

                totalHeuresPlanifiées += task.GetNbDHeures();
            }

            return totalHeuresPlanifiées;
        }

        private static int GetNbHeuresPlanifiéesParChantier(NDatasModel.IChantier chantier)
        {
            int nbHeures = 0;
            var tasks = Model.Instance.GetTasks(chantier.KeyId);
            foreach(var task in tasks)
            {
                nbHeures += task.GetNbDHeures();
            }

            return nbHeures;
        }

        private static bool IsChantierNonClos(NDatasModel.IChantier chantier)
        {
            return chantier.Statut != NDatasModel.EStatutChantier.CLOS;
        }

        private void OnCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
