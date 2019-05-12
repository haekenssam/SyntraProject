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
            // TxtTotaalTeBetalen, TxtBetaald en TxtTerugBetalen resetten naar 0
            TxtTotaalTeBetalen.Text = "0,00";
            TxtBetaald.Text = "0,00";
            TxtTerugBetalen.Text = "0,00";

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
            int.TryParse(TxtArtikelId.Text, out int productid);
            float.TryParse(TxtHoeveelheid.Text, out float aantal);

                try
                {
                    DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(productid, aantal);
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
            Bestelling totaaltebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen();
            Math.Round(totaaltebetalen.Totaal, 2);           
            TxtTotaalTeBetalen.Text = totaaltebetalen.Totaal.ToString();

        }

        //Betaling uitvoeren
        private void BtnBetalen_Click(object sender, RoutedEventArgs e)
        {
            //Betaling uitvoeren
            float.TryParse(TxtBetaald.Text, out float betaald);
            float.TryParse(TxtTotaalTeBetalen.Text, out float totaalTeBetalen);
            float terugBetalen = (betaald - totaalTeBetalen);
            Math.Round(terugBetalen, 2);

            if (terugBetalen<0)
            {
                MessageBox.Show("Het betaalde bedrag is kleiner dan het totaal te betalen. De betaling is niet uitgevoerd!");            
            }
            else
            {
                TxtTerugBetalen.Text = terugBetalen.ToString();

                //Product Stock aanpassen


                //Betaling Database tabel invullen
                try
                {
                    DatabaseManager.Instance.BestellingRepository.InsertBetaling(totaalTeBetalen, betaald, terugBetalen);
                }
                catch (BusinessException excp)
                {
                    MessageBox.Show(excp.ToString());
                }

            }
           
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
            DatabaseManager.Instance.BestellingRepository.DeleteBestellingLijn(bestellingLijn.BestellingLijnenId, bestellingLijn.ProductId, bestellingLijn.Aantal);

            //BestellingLijnen importeren en tonen
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen().ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;

            //Toon de "Te Betalen totaal"
            Bestelling tebetalen = DatabaseManager.Instance.BestellingRepository.GetTotaalTeBetalen();
            TxtTotaalTeBetalen.Text = tebetalen.Totaal.ToString();




        }

        //BestellingLijn Selecteren
        private Bestelling GetSelectedBestellingLijn()
        {
            Bestelling selectedBestellingLijn = LstBestellingLijnen.SelectedItem as Bestelling;

            return selectedBestellingLijn;
        }

        //KassaTicket aanmaken+printen
        private void BtnKassaTicket_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new KassaTicketPage());
        }
    }
}
