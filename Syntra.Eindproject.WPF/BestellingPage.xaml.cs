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
using static Syntra.Eindproject.WPF.MoveablePopup;

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
            Popup.IsOpen = true;

        }

        public void Initialize()
        {
            List<Bestelling> bestellingen = DatabaseManager.Instance.BestellingRepository.GetBestelling().ToList();
            DgBestellingen.ItemsSource = bestellingen;
        }



        private void Popup_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MoveablePopup.POINT curPos;
            IntPtr hWndPopUp;

            GetCursorPos(out curPos);
            hWndPopUp = WindowFromPoint((curPos));

            ReleaseCapture();
            SendMessage(hWndPopUp, WM_NCLBUTTONDOWN, new IntPtr(HT_CAPTION), IntPtr.Zero);
        }

        private void BtnOkAantal_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TbZoekArtikel.Text, out int id);
            int.TryParse(TbAantal.Text, out int aantal);
            DatabaseManager.Instance.BestellingRepository.InsertBestellingLijn(id, aantal);

            Popup.IsOpen = false;

            Initialize();
        }

        private void BtnAnnuleerAantal_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = false;
            Initialize();
        }
    }
}
