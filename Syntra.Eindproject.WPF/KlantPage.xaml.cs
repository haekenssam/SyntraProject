//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace Syntra.Eindproject.WPF
//{
//    /// <summary>
//    /// Interaction logic for KlantPage.xaml
//    /// </summary>
//    public partial class KlantPage : Page
//    {
//        public KlantPage()
//        {
//            InitializeComponent();
//        }
//    }
//}
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
using static Syntra.Eindproject.WPF.MoveablePopup;

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

        // List<Winkelwagen> listOfWinkelwagenLijnen = new List<Winkelwagen>();

        // Button product inscannen
        private void BtnScan_Click(object sender, RoutedEventArgs e)
        {
            bool checktB = int.TryParse(textBox.Text, out int productid);
            bool checktB1 = float.TryParse(textBox1.Text, out float aantal);

            List<int?> checkAvailableProductId = DatabaseManager.Instance.WinkelwagenRepository.CheckProductIdAvailable(productid).ToList();
            List<float> checkAvailableAantalMagazijn = DatabaseManager.Instance.WinkelwagenRepository.CheckAantalInMagazijn(productid).ToList();
            List<float?> checkAvailableAantalWinkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.CheckAantalInWinkelwagen(productid).ToList();

            if (checkAvailableProductId.Count == 0)
            {
                MessageBox.Show("Gekozen product is niet beschikbaar");
            }
            else
            {
                if (textBox.Text == (string.Empty) || textBox.Text == "" || textBox1.Text == string.Empty || textBox1.Text == "" || checktB == false || checktB1 == false || float.Parse(textBox1.Text) < 0)
                {
                    //throw new BusinessException("Niet alle velden zijn ingevuld!");
                    MessageBox.Show("Inhoud niet correct ingevuld");
                }
                else
                {
                    int sizeId = checkAvailableProductId.Count;
                    float? aantalMagazijn = checkAvailableAantalMagazijn[0];
                    float? aantalWinkelwagenLijnen = checkAvailableAantalWinkelwagenLijnen[0];
                    float? difference = aantalMagazijn - aantalWinkelwagenLijnen - float.Parse(textBox1.Text);
                    if (difference < 0)
                    {
                        MessageBox.Show("Te weinig in voorraad");
                    }
                    else
                    {
                        #region tekst2
                        //List<Winkelwagen> winkelwagenAantalProductId = DatabaseManager.Instance.WinkelwagenRepository.CountAantalWinkelwagenPerProductId( productid).ToList();
                        //List<Winkelwagen> productAantalProductId = DatabaseManager.Instance.WinkelwagenRepository.CountAantalProductPerProductId(productid).ToList();
                        // winkelwagenAantalProductId[0];
                        //float winkelwagenAantalProductId2 = DatabaseManager.Instance.WinkelwagenRepository.CountAantalWinkelwagenPerProductId2( productid);
                        //float productAantalProductId2 = DatabaseManager.Instance.WinkelwagenRepository.CountAantalWinkelwagenPerProductId2(productid);
                        //bool compareAantal = DatabaseManager.Instance.WinkelwagenRepository.CompareAantal(winkelwagenAantalProductId2, productAantalProductId2, productid);
                        //WinkelwagenRepository.CompareAantal2(int.Parse(winkelwagenAantalProductId2), productAantalProductId2, productid);
                        //if (compareAantal)
                        //{
                        //if (productid.ToString() == (string.Empty) || productid.ToString() == "" || aantal.ToString() == string.Empty || aantal.ToString() == "")
                        #endregion

                        try
                        {

                            DatabaseManager.Instance.WinkelwagenRepository.InsertWinkelwagenLijnen(productid, aantal);
                        }
                        catch (BusinessException excp)
                        {
                            MessageBox.Show(excp.ToString());
                        }

                        //DatabaseManager.Instance.WinkelwagenRepository.CountAantalPerProductId( productid );
                        Initialize();
                    }
                }
            }
            #region list

            //listBoxWinkelmand.Items.Add(textBox.Text);
            //int a;
            //float b;
            //if ((int.TryParse(textBox.Text, out a)) && (float.TryParse(textBox1.Text, out b)))
            //{
            //listBoxWinkelmand.Items.Clear();
            //Winkelwagen bestelling = new Winkelwagen(int.Parse(textBox.Text), int.Parse(textBox1.Text));
            //listOfWinkelwagenLijnen.Add(bestelling);
            //foreach (Winkelwagen best in listOfWinkelwagenLijnen)
            // {
            //     listBoxWinkelmand.Items.Add(best.ShowWinkelwagen());
            // }
            // //textBox.Text = "";
            //textBox1.Text = "";
            //}
            //else
            //{
            //textBox.Text = "";
            //textBox1.Text = "";
            //}

            //List<Winkelwagen> winkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.GetWinkelwagenLijnen().ToList();
            //ListWinkelwagenLijnen.ItemsSource = winkelwagenLijnen;
            //listBoxWinkelmand.ItemsSource = winkelwagenLijnen;

            //textBox.Text = string.Empty;
            //textBox1.Text = string.Empty;

            //listBoxWinkelmand.Items.Clear();
            //listBoxWinkelmand.Items.Add(DatabWinkelwagenLijnen);

            #endregion

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
                //throw new BusinessException("Niet alle velden zijn ingevuld!");
            }
            else
            {
                DatabaseManager.Instance.WinkelwagenRepository.DeleteWinkelwagenLijnen(winkelwagenLijn.ID);
            }

            Initialize();

        }

        // Button update Lijn
        private void btnUpdateLijn(object sender, RoutedEventArgs e)
        {

            bool check1tB = float.TryParse(textBox1.Text, out float AantalProdWinkelwagen);
            Winkelwagen winkelwagenLijn = GetSelectedWinkelwagenLijn();

            if (textBox1.Text == string.Empty || textBox1.Text == "" || winkelwagenLijn == null || check1tB == false || float.Parse(textBox1.Text) < 0)
            {
                //throw new BusinessException("Niet alle velden zijn ingevuld!");
                MessageBox.Show("Inhoud niet correct ingevuld");
            }
            else
            {
                List<float?> checkAvailableAantalMagazijn = DatabaseManager.Instance.WinkelwagenRepository.CheckProductIdAvailableUpdate(winkelwagenLijn.ID).ToList();
                List<float?> checkAvailableAantalWinkelwagenLijnen = DatabaseManager.Instance.WinkelwagenRepository.CheckAantalProductIdAvailableWinkelwagenUpdate(winkelwagenLijn.ID).ToList();

                float? aantalMagazijn = checkAvailableAantalMagazijn[0];
                float? aantalWinkelwagenLijnen = checkAvailableAantalWinkelwagenLijnen[0];
                float? difference = aantalMagazijn - aantalWinkelwagenLijnen - float.Parse(textBox1.Text);
                if (difference < 0)
                {
                    MessageBox.Show("Niet voldoende voorraad");
                }
                else
                {
                    try
                    {
                        DatabaseManager.Instance.WinkelwagenRepository.UpdateWinkelwagenLijnen(winkelwagenLijn.ID, AantalProdWinkelwagen);
                    }
                    catch (BusinessException excp)
                    {
                        MessageBox.Show(excp.ToString());
                    }

                    Initialize();
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
