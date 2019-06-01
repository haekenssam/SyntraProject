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
    /// Interaction logic for KlantPage.xaml
    /// </summary>
    public partial class KlantPage : Page
    {
        public KlantPage()
        {
            InitializeComponent();            
        }

        //WinkelwagenId aanmaken en tonen + Datagrid en textboxes resetten
        private void KlantPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Tabellen WinkelwagenLijnen en Winkelwagen leegmaken van de rijen waar "Totaal = 0" in de tabel Winkelwagen
            DatabaseManager.Instance.WinkelwagenRepository.DeleteEmptyWinkelwagenLijnen();
            DatabaseManager.Instance.WinkelwagenRepository.DeleteEmptyWinkelwagen();


            //Reset 
            TxtTotaalTeBetalen.Text = "0,00";
            TxtArtikelId.Text = string.Empty;
            TxtHoeveelheid.Text = string.Empty;

            //Datagrid resetten 
            LstWinkelwagenLijnen.Items.Clear();

            //Nieuwe WinkelwagenId aanmaken
            DatabaseManager.Instance.WinkelwagenRepository.InsertWinkelwagen();

            //WinkelwagenId opladen           
            Winkelwagen winkelwagenNr = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelWagenId();
            TxtWinkelwagenNr.Text = winkelwagenNr.Id.ToString();
        }

        //WinkelwagenLijnen importeren en tonen
        public void Initialize()
        {
            //WinkelwagenLijnen importeren en tonen
            List<Winkelwagen> winkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen().ToList();
            LstWinkelwagenLijnen.ItemsSource = winkelwagenLijnen;
        }

        //(Product + hoeveelheid) toevoegen  + Toon "Totaal te betalen bedrag"
        private void BtnWinkelwagenLijnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            //BestellingLijn toevoegen
            int.TryParse(TxtArtikelId.Text, out int productid);
            float.TryParse(TxtHoeveelheid.Text, out float aantal);

            try
            {
                DatabaseManager.Instance.WinkelwagenRepository.InsertWinkelwagenLijnen(productid, aantal);
            }
            catch (BusinessException excp)
            {
                MessageBox.Show(excp.ToString());
            }

            //TxtArtikelId + TxtHoeveelheid resetten (leegmaken)
            TxtArtikelId.Text = string.Empty;
            TxtHoeveelheid.Text = string.Empty;

            Initialize();


            //Toon de "Te Betalen totaal"
            Winkelwagen totaaltebetalen = DatabaseManager.Instance.WinkelwagenRepository.GetTotaalTeBetalen();
            Math.Round(totaaltebetalen.Totaal, 2);
            TxtTotaalTeBetalen.Text = totaaltebetalen.Totaal.ToString("0.00");

        }

        //BestellingLijn verwijderen
        private void BtnWinkelwagenLijnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            //De geselecteerde winkelwagenLijn uit de database verwijderen
            Winkelwagen winkelwagenLijn = GetSelectedWinkelwagenLijn();
            DatabaseManager.Instance.WinkelwagenRepository.DeletewinkelwagenLijn(winkelwagenLijn.LijnId, winkelwagenLijn.ProductId, winkelwagenLijn.Aantal);

            //WinkelwagenLijnen importeren en tonen
            List<Winkelwagen> winkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen().ToList();
            LstWinkelwagenLijnen.ItemsSource = winkelwagenLijnen;

            //Toon de "Te Betalen totaal"
            Winkelwagen tebetalen = DatabaseManager.Instance.WinkelwagenRepository.GetTotaalTeBetalen();
            TxtTotaalTeBetalen.Text = tebetalen.Totaal.ToString("0.00");
        }

        //WinkelwagenLijn Selecteren
        private Winkelwagen GetSelectedWinkelwagenLijn()
        {
            Winkelwagen selectedwinkelwagenLijn = LstWinkelwagenLijnen.SelectedItem as Winkelwagen;

            return selectedwinkelwagenLijn;
        }

        //Naar De kassa + Update veld Totaal in de tabel Winkelwagen
        private void BtnNaarDeKassa_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TxtWinkelwagenNr.Text, out int winkelwagenid);
            float.TryParse(TxtTotaalTeBetalen.Text, out float totaaltebetalen);
            DatabaseManager.Instance.WinkelwagenRepository.UpdateTotaalWinkelWagen(winkelwagenid, totaaltebetalen);

            NavigationService.Navigate(new KlantBestellingPage());
            
        }
    }
}
