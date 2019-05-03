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

        //BestellingLijnen importeren van SQL en tonen
        public void Initialize()
        {
            List<Bestelling> bestellingLijnen = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnen().ToList();
            LstBestellingLijnen.ItemsSource = bestellingLijnen;
        }

        //Product + Aantal toevoegen = BestellingLijn toevoegen
        private void BtnBestellingLijnToevoegen_Click(object sender, RoutedEventArgs e)
        {
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
            
            
          
            //if (!DatabaseManager.Instance.ProductRepository.IsValidProduct(productid) || string.IsNullOrEmpty(TxtArtikelId.Text))
            //{
            //    MessageBox.Show("Artikel Nr. is niet geldig");
            //}

            //else
            //{

            //}

            TxtArtikelId.Text = string.Empty;
            TxtHoeveelheid.Text = string.Empty;

            Initialize();

            //Toon de "Te Betalen totaal"
            List<double> tebetalen = DatabaseManager.Instance.BestellingRepository.GetTeBetalenBedrag().ToList();
            foreach (var item in tebetalen)
            {
                TxtTotaalTeBetalen.Text = tebetalen.First().ToString();
            }
        }


        //Te betalen bedrag
        private void TxtTotaalTeBetalen_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        //BestellingId aanmaken en tonen als FactuurNr
        private void KassiersterPage_OnLoaded(object sender, RoutedEventArgs e)
        {   
            //Datagrid resetten + 1 een nieuwe BestellingId aanmaken
            LstBestellingLijnen.Items.Clear();
            DatabaseManager.Instance.BestellingRepository.InsertBestelling();


            //FactuurNr = BestellingId opladen           
            List<string> factuurNr = DatabaseManager.Instance.BestellingRepository.GetBestellingId().ToList();

            foreach (var item in factuurNr)
            {
                TxtFactuurNummer.Text = factuurNr.First(); 
            }

        }

        private void BtnBetalen_Click(object sender, RoutedEventArgs e)
        {
            //Betaling uitvoeren
            double.TryParse(TxtBetaald.Text, out double betaald);
            double.TryParse(TxtTotaalTeBetalen.Text, out double totaalTeBetalen);
            double terugBetalen = (betaald - totaalTeBetalen);

            if (terugBetalen<0)
            {
                MessageBox.Show("Het betaalde bedrag is kleiner dan het te betalen bedrag!");
            }
            else
            {
                TxtTerugBetalen.Text = terugBetalen.ToString();
            }
           
        }
    }
}
