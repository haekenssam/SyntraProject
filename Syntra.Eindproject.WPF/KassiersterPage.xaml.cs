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
            TxtBestellingNrToevoegen.Text = string.Empty;

            //Datagrid resetten + 1 een nieuwe BestellingId aanmaken
            LstBestellingLijnen.Items.Clear();
            DatabaseManager.Instance.BestellingRepository.InsertBestelling();

            //FactuurNr = BestellingId opladen           
            Bestelling factuurNr = DatabaseManager.Instance.BestellingRepository.GetBestellingId();
            TxtFactuurNummer.Text = factuurNr.Id.ToString();
        }

        //BestellingLijnen importeren en tonen
        public void Initialize()
        {
            //BestellingLijnen importeren en tonen
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen().ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;
        }

        //(Product + hoeveelheid) toevoegen = BestellingLijn toevoegen + Toon "Totaal te betalen bedrag"
        private void BtnBestellingLijnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            //BestellingLijn toevoegen
            bool ProductId = int.TryParse(TxtArtikelId.Text, out int productid);
            bool Aantal = float.TryParse(TxtHoeveelheid.Text, out float aantal);

            if (Aantal && ProductId)
            {
                try
                {
                    aantal = (float)DatabaseManager.Instance.ProductRepository.ControleStock(productid, aantal);
                    DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(productid, aantal);
                }
                catch (BusinessException excp)
                {

                    MessageBox.Show(excp.Message);
                }
                DatabaseManager.Instance.ProductRepository.UpdateStockProduct(aantal);
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
            Bestelling totaaltebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen();
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

            //Product Stock aanpassen
            //DatabaseManager.Instance.ProductRepository.UpdateStockProduct(); //Nakijken!!!!!!!!


        }

        //Volgende klant
        private void BtnVolgendeKlant_Click(object sender, RoutedEventArgs e)
        {
            //FactuurNr Textbox resetten
            TxtFactuurNummer.Text = string.Empty;

            ////Een nieuwe FactuurNr (BestellingId) aanmaken
            DatabaseManager.Instance.BestellingRepository.InsertBestelling();

            //Datagrid resetten
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen().ToList();
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
            //De geselecteerde bestellinglijn door de functie GetSelectedBestellingLijn() uit de database verwijderen
            Bestelling bestellingLijn = GetSelectedBestellingLijn();
            if (bestellingLijn != null)
            {
                DatabaseManager.Instance.BestellingRepository.DeleteBestellingLijn(bestellingLijn.LijnId, bestellingLijn.ProductId, bestellingLijn.Aantal);
                DatabaseManager.Instance.ProductRepository.AddStock(bestellingLijn.ProductId, bestellingLijn.Aantal);
            }

            //BestellingLijnen importeren en tonen
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen().ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;

            //Toon de "Te Betalen totaal"
            Bestelling tebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen();
            TxtTotaalTeBetalen.Text = tebetalen.Totaal.ToString("0.00");

        }

        //BestellingLijn Selecteren
        private Bestelling GetSelectedBestellingLijn()
        {
            Bestelling selectedBestellingLijn = LstBestellingLijnen.SelectedItem as Bestelling;

            return selectedBestellingLijn;
        }

        //KassaTicket aanmaken+(printen)
        private void BtnKassaTicket_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new KassaTicketPage());
        }

        //Terug naar MainMenu
        private void TerugNaarHoofdMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenu());
        }

        //Bestelling van de klant intypen
        private void BtnBestellingNrToevoegen_Click(object sender, RoutedEventArgs e)
        {
            LstBestellingLijnen.Items.Clear();
            
            int.TryParse(TxtBestellingNrToevoegen.Text, out int winkelwagenNr);

            //WinkelwagenLijnen kopieren naar BestellingLijnen
            List<Winkelwagen> winkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen2(winkelwagenNr).ToList();                      
            foreach (Winkelwagen  item in winkelwagenLijnen)
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
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen().ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;

            //Toon de "Te Betalen totaal"
            Bestelling totaaltebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen();
            Math.Round(totaaltebetalen.Totaal, 2);
            TxtTotaalTeBetalen.Text = totaaltebetalen.Totaal.ToString("0.00");

            //Reset TxtBestellingNrToevoegen.Text
            TxtBestellingNrToevoegen.Text = string.Empty;
        }
    }
}
