using Agenda_ICS.Views.Configuration.Pages;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_ICS.Views.Configuration
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationWnd.xaml
    /// </summary>
    public partial class ConfigurationWnd : Window
    {
        // *** PUBLIC **********************************

        public ConfigurationWnd()
        {
            InitializeComponent();
        }

        // *** RESTRICTED ******************************

        private IList<UserControl> _listOfPages = new List<UserControl>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _listOfPages.Add(new ChantiersConfig());
            _listOfPages.Add(new EmployeesConfig());

            PageContent.Content = _listOfPages[0];
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var index = int.Parse(((Button)e.Source).Uid);
            GridCursor.Margin = new Thickness(10 + (150 * index), 0, 0, 0);

            PageContent.Content = _listOfPages[index];
        }
    }
}
