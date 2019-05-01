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

            if (!DatabaseManager.Instance.ProductRepository.IsValidProduct(productid) || string.IsNullOrEmpty(TxtArtikelId.Text))
            {
                MessageBox.Show("Artikel Nr. is niet geldig");
            }

            else
            {
                DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(productid, aantal);
            }

            TxtArtikelId.Text = string.Empty;
            TxtHoeveelheid.Text = string.Empty;
        }

        //Te betalen bedrag
        private void TxtTotaalTeBetalen_TextChanged(object sender, TextChangedEventArgs e)
        {
            //string totaal = DatabaseManager.Instance.BestellingRepository.CalculateTotaalBestelling().ToString();
            //TxtTotaalTeBetalen.Text = totaal;
        }



    }
}
