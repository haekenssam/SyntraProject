using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syntra.Eindproject.BL;
using Syntra.Eindproject.Dapper;
using Syntra.Eindproject.BL.Models;

namespace Syntra.Eindproject.WPF
{
    /// <summary>
    /// Interaction logic for KlantBestellingPage.xaml
    /// </summary>
    public partial class KlantBestellingPage : Page
    {
        public KlantBestellingPage()
        {
            InitializeComponent();
        }

        private void KlantBestellingPage_OnLoaded(object sender, RoutedEventArgs e)
        {   

            TxtBestellingNr.Text = string.Empty;

            //Hier moet de TxtBestellingNr.Text de waarde van de laatste WinkelwagenId nemen
            Winkelwagen winkelwagenNr = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelWagenId();
            TxtBestellingNr.Text = winkelwagenNr.Id.ToString();

        }

        private void BtnVolgendeBestelling_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new KlantPage());
        }

        private void BtnTerugLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenu());
        }
    }
}
