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
    /// Interaction logic for KassiersterPage.xaml
    /// </summary>
    public partial class KassiersterPage : Page
    {
        public KassiersterPage()
        {
            InitializeComponent();
        }

        //BestellingId aanmaken en tonen als FactuurNr + Datagrid en textboxes resetten
        private void KassiersterPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            //Tabellen BestellingLijnen en Bestelling leegmaken van de rijen waar "Totaal = 0" in de tabel Bestelling
            DatabaseManager.Instance.BestellingRepository.DeleteEmptyBestellingLijnen();
            DatabaseManager.Instance.BestellingRepository.DeleteEmptyBestelling();

            // TxtTotaalTeBetalen, TxtBetaald en TxtTerugBetalen resetten naar 0
            TxtTotaalTeBetalen.Text = "0,00";
            TxtBetaald.Text = "0,00";
            TxtTerugBetalen.Text = "0,00";

            //Reset TxtBestellingNrToevoegen.Text
            TxtBestellingNrToevoegen.Text = string.Empty;


            //Datagrid resetten + 1 een nieuwe BestellingId aanmaken
            LstBestellingLijnen.Items.Clear();
            DatabaseManager.Instance.BestellingRepository.InsertBestelling();

            //FactuurNr = BestellingId opladen           
            Bestelling factuurNr = DatabaseManager.Instance.BestellingRepository.GetBestellingId();
            TxtFactuurNummer.Text = factuurNr.Id.ToString();
        }

        //BestellingLijnen (datagrid) importeren en tonen
        public void Initialize()
        {
            //BestellingLijnen importeren en tonen
            int.TryParse(TxtFactuurNummer.Text, out int bestellingId);
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen(bestellingId).ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;
        }

        //(Product + hoeveelheid) toevoegen = BestellingLijn toevoegen + Stock updaten + Toon "Totaal te betalen bedrag"
        private void BtnInscannen_Click(object sender, RoutedEventArgs e)
        {
            //BestellingLijn toevoegen + Stock update
            bool ProductId = int.TryParse(TxtArtikelId.Text, out int productid);
            bool Aantal = float.TryParse(TxtHoeveelheid.Text, out float aantal);

            if (Aantal && ProductId)
            {
                try
                {
                    aantal = (float)DatabaseManager.Instance.ProductRepository.ControleStock(productid, aantal);
                    DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(productid, aantal);
                    DatabaseManager.Instance.ProductRepository.UpdateStockProduct(aantal);
                }
                catch (BusinessException excp)
                {
                    MessageBox.Show(excp.Message);
                }

            }
            else
            {
                MessageBox.Show("Ongeldige invoer");
            }

            //TxtArtikelId + TxtHoeveelheid resetten (leegmaken)
            TxtArtikelId.Text = string.Empty;
            TxtHoeveelheid.Text = string.Empty;

            Initialize();


            //Toon de "Te Betalen totaal"
            int.TryParse(TxtFactuurNummer.Text, out int bestellingId);
            Bestelling totaaltebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen(bestellingId);
            Math.Round(totaaltebetalen.Totaal, 2);
            TxtTotaalTeBetalen.Text = totaaltebetalen.Totaal.ToString("0.00");

        }

        //Betaling uitvoeren
        private void BtnBetalen_Click(object sender, RoutedEventArgs e)
        {
            //Betaling uitvoeren
            bool Betaald = float.TryParse(TxtBetaald.Text, out float betaald);
            bool TotaalTeBetalen = float.TryParse(TxtTotaalTeBetalen.Text, out float totaalTeBetalen);
            if (Betaald && TotaalTeBetalen)
            {
                try
                {
                    float terugBetalen = DatabaseManager.Instance.BestellingRepository.TerugBetalenBedrag(betaald, totaalTeBetalen);

                    TxtTerugBetalen.Text = terugBetalen.ToString("0.00");
                    DatabaseManager.Instance.BestellingRepository.InsertBetaling(totaalTeBetalen, betaald, terugBetalen);
                    MessageBox.Show("Betaling uitgevoerd");
                }
                catch (BusinessException excp)
                {
                    MessageBox.Show(excp.Message);
                }
            }

            //update veld Totaal in de tabel Bestelling
            int.TryParse(TxtFactuurNummer.Text, out int bestellingid);
            DatabaseManager.Instance.BestellingRepository.UpdateTotaalBestelling(bestellingid, totaalTeBetalen);

        }

        //Volgende klant indien de klant geen kassaticket wenst te krijgen
        private void BtnVolgendeKlant_Click(object sender, RoutedEventArgs e)
        {
            //FactuurNr Textbox resetten
            TxtFactuurNummer.Text = string.Empty;

            ////Een nieuwe FactuurNr (BestellingId) aanmaken
            DatabaseManager.Instance.BestellingRepository.InsertBestelling();

            //Datagrid resetten
            int.TryParse(TxtFactuurNummer.Text, out int bestellingId);
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen(bestellingId).ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;

            //FactuurNr = BestellingId opladen           
            Bestelling factuurNr = DatabaseManager.Instance.BestellingRepository.GetBestellingId();
            TxtFactuurNummer.Text = factuurNr.Id.ToString();

            //// TxtTotaalTeBetalen, TxtBetaald en TxtTerugBetalen resetten naar 0
            TxtTotaalTeBetalen.Text = "0,00";
            TxtBetaald.Text = "0,00";
            TxtTerugBetalen.Text = "0,00";
        }

        //BestellingLijn verwijderen
        private void BestellingLijnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            //De geselecteerde bestellinglijn verwijderen en stock opnieuw updaten
            Bestelling bestellingLijn = GetSelectedBestellingLijn();
            if (bestellingLijn != null)
            {
                DatabaseManager.Instance.BestellingRepository.DeleteBestellingLijn(bestellingLijn.LijnId, bestellingLijn.ProductId, bestellingLijn.Aantal);
                DatabaseManager.Instance.ProductRepository.AddStock(bestellingLijn.ProductId, bestellingLijn.Aantal);
            }

            //BestellingLijnen importeren en tonen
            int.TryParse(TxtFactuurNummer.Text, out int bestellingId);
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen(bestellingId).ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;

            //Toon de "Te Betalen totaal"
            Bestelling tebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen(bestellingId);
            TxtTotaalTeBetalen.Text = tebetalen.Totaal.ToString("0.00");

        }

        //BestellingLijn Selecteren
        private Bestelling GetSelectedBestellingLijn()
        {
            Bestelling selectedBestellingLijn = LstBestellingLijnen.SelectedItem as Bestelling;

            return selectedBestellingLijn;
        }

        //KassaTicket aanmaken + (printen)
        private void BtnKassaTicket_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new KassaTicketPage());
        }

        //Terug naar MainMenu
        private void TerugNaarMainMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenu());
        }

        //BestellingNummer van de klant intypen en kopieren van WinkelwagenLijnen naar BestellingLijnen
        private void BtnBestellingNrToevoegen_Click(object sender, RoutedEventArgs e)
        {
            LstBestellingLijnen.Items.Clear();

            int.TryParse(TxtBestellingNrToevoegen.Text, out int winkelwagenNr);

            //WinkelwagenLijnen kopieren naar BestellingLijnen
            List<Winkelwagen> winkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen2(winkelwagenNr).ToList();
            foreach (Winkelwagen item in winkelwagenLijnen)
            {
                try
                {
                    DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(item.ProductId, item.Aantal);
                }
                catch (BusinessException excp)
                {
                    MessageBox.Show(excp.ToString());
                }
            }

            //BestellingLijnen importeren en tonen
            int.TryParse(TxtFactuurNummer.Text, out int bestellingId);
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen(bestellingId).ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;

            //Toon de "Te Betalen totaal"
            Bestelling totaaltebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen(bestellingId);
            Math.Round(totaaltebetalen.Totaal, 2);
            TxtTotaalTeBetalen.Text = totaaltebetalen.Totaal.ToString("0.00");

            //Reset TxtBestellingNrToevoegen.Text
            TxtBestellingNrToevoegen.Text = string.Empty;
        }
    }
}
