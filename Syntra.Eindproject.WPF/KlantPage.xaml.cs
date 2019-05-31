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
using Syntra.Eindproject.BL.Models;
using Syntra.Eindproject.Dapper;
using Syntra.Eindproject.Dapper.Repositories;

namespace Syntra.Eindproject.WPF
{
    /// <summary>
    /// Interaction logic for BestellingPage.xaml
    /// </summary>
    public partial class KlantPage : Page
    {
        public KlantPage()
        {
            InitializeComponent();
        }

        //Lijnen importeren en tonen
        public void Initialize()
        {
            List<Winkelwagen> winkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen().ToList();
            WinkelwagenLijnen.ItemsSource = winkelwagenLijnen;

            List<Winkelwagen> winkelwagenLijnen2 = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen2().ToList();
            WinkelwagenLijnen2.ItemsSource = winkelwagenLijnen2;
        }

        // Winkelwagen nummer maken
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseManager.Instance.WinkelwagenRepository.InsertWinkelwagen();
        }

        // Button product inscannen
        private void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            bool checktB = int.TryParse(TxtProductId.Text, out int productid);
            bool checktB1 = float.TryParse(TxtAantal.Text, out float aantal);
            if (checktB == false)
            { MessageBox.Show("'Product' is niet correct ingegeven"); }
            else if (checktB1 == false)
            { MessageBox.Show("'Aantal' is niet correct ingegeven"); }
            else { 
            try
            {
                DatabaseManager.Instance.WinkelwagenRepository.InsertWinkelwagenLijnen(productid, aantal);
                Initialize();
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            }

            TxtProductId.Text = string.Empty;
            TxtAantal.Text = string.Empty;
        }

        //Button naar kassa
        private void BtnNrKassa_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainMenu());
        }


        // Button verwijderen van de lijn
        private void BtnVerwijder_Click(object sender, RoutedEventArgs e)
        {

            Winkelwagen winkelwagenLijn = GetSelectedWinkelwagenLijn();
            if (winkelwagenLijn == null)
            {
                MessageBox.Show("Verwijderen niet mogelijk");
            }
            else
            {
                DatabaseManager.Instance.WinkelwagenRepository.DeleteWinkelwagenLijnen(winkelwagenLijn.ID);
                Initialize();
            }
        }

        // Button update Lijn
        private void btnUpdateLijn(object sender, RoutedEventArgs e)
        {

            bool check1tB = float.TryParse(TxtAantal.Text, out float AantalProdWinkelwagen);
            Winkelwagen winkelwagenLijn = GetSelectedWinkelwagenLijn();
            if (winkelwagenLijn == null)
            {
                //throw new BusinessException("Niet alle velden zijn ingevuld!");
                MessageBox.Show("Gelieve een lijn te selecteren");
            }
            else if (check1tB == false)
            {
                //throw new BusinessException("Niet alle velden zijn ingevuld!");
                MessageBox.Show("'aantal' niet correct ingegeven");
            }
            else
            {
                try
                {
                    DatabaseManager.Instance.WinkelwagenRepository.UpdateWinkelwagenLijnen(winkelwagenLijn.ID, AantalProdWinkelwagen);
                    Initialize();
                }
                catch (BusinessException ex)
                {
                    MessageBox.Show(ex.Message);

                }
            }

        }

        // Geselecteerde lijn importeren
        private Winkelwagen GetSelectedWinkelwagenLijn()
        {
            Winkelwagen current = WinkelwagenLijnen.SelectedItem as Winkelwagen;

            return current;
        }
    }
}
