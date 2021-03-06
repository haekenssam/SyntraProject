﻿using System;
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
    /// Interaction logic for KassaTicketPage.xaml
    /// </summary>
    public partial class KassaTicketPage : Page
    {
        public KassaTicketPage()
        {
            InitializeComponent();
        }

        private void KassaTicketPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            LstKassaTicket.Items.Clear();

            //1. FactuurNr 
            Bestelling factuurNr = DatabaseManager.Instance.BestellingRepository.GetBestellingId();
            TxtFactuurNummer.Text = factuurNr.Id.ToString();

            //2. Datum en tijd
                    //2.1 Datum
                    TxtDatum.Text = DateTime.Now.Date.ToShortDateString();

                    //2.2 Tijd
                    TxtTijd.Text = DateTime.Now.ToString("HH:mm:ss");

            //3. Datagrid invullen           
            List<Bestelling> lijst = DatabaseManager.Instance.BestellingRepository.GetBestellingLijnenKassaTicket(factuurNr.Id).ToList();
            LstKassaTicket.ItemsSource = lijst;

            //4. Textbox invullen
            TxtBetaling.Text = string.Empty;
            int.TryParse(TxtFactuurNummer.Text, out int bestellingId);
            Bestelling totaal = DatabaseManager.Instance.BestellingRepository.GetBetalingenTotaalTeBetalen(bestellingId);
            Bestelling betaald = DatabaseManager.Instance.BestellingRepository.GetBetalingenBetaald(bestellingId);
            Bestelling terug = DatabaseManager.Instance.BestellingRepository.GetBetalingenTerug(bestellingId);

            Math.Round(totaal.Totaal, 2);
            Math.Round(betaald.Betaald, 2);
            Math.Round(terug.Terug, 2);

            TxtBetaling.Text = "Totaal te betalen: " + "\t" + totaal.Totaal.ToString("0.00") + " €" + "\n" 
                             + "Betaald: " +"\t" + "\t" + betaald.Betaald.ToString("0.00") + " €" + "\n"
                             + "Terug: " + "\t" + "\t" + terug.Terug.ToString("0.00") + " €" + "\n" +"\n"
                             + "Dank u en tot ziens!";
            //5. Print kassaticket

            //6. Terug naar de KassiersterPage
            //NavigationService.Navigate(new KassiersterPage());

        }
    }
}
