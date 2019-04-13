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
    /// Interaction logic for BestellingPage.xaml
    /// </summary>
    public partial class BestellingPage : Page
    {
        public BestellingPage()
        {
            InitializeComponent();
        }

        private void BtnVoegToe_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TbZoekArtikel.Text, out int id);
            DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(id);

            Initialize();
        }

        public void Initialize()
        {
            List<Bestelling> bestellingen = DatabaseManager.Instance.BestellingRepository.GetBestelling().ToList();
            DgBestellingen.ItemsSource = bestellingen;
        }
    }
}
