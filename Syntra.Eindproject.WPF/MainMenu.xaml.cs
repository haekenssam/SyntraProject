using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using Syntra.Eindproject.BL.Models;

namespace Syntra.Eindproject.WPF
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            LbUsers.Items.Clear();

            List<User> Gebruikers = DatabaseManager.Instance.GebruikerRepository.GetGebruikers().ToList();
            foreach (User user in Gebruikers)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Tag = user,
                    Content = user.Gebruiker
                };
                LbUsers.Items.Add(item);
            }
        }

        private void MainMenu_OnLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string gebruiker = ((ListBoxItem) LbUsers.SelectedItem).Content.ToString();
            string paswoord = TxtPaswoord.Password.ToString();

            bool test = DatabaseManager.Instance.GebruikerRepository.IsUserValid(gebruiker, paswoord);

            if (test == true)
            {
                if (gebruiker == "Magazijnier")
                {
                    NavigationService.Navigate(new MagazijnierPage());
                }

                if (gebruiker == "Kassierster")
                {
                    NavigationService.Navigate(new KassiersterPage());
                }

                if (gebruiker == "Klant")
                {
                    NavigationService.Navigate(new KlantPage());
                    //NavigationService.Navigate(new BestellingPage());
                }
            
            }
            else
            {
                MessageBox.Show("verkeerde login");
            }

            // Oude manier van login --> nu met code behind in Gebruikerrepository 5/05/2019

            //switch (gebruiker)
            //{
            //    case "Magazijnier":
            //        if (!DatabaseManager.Instance.GebruikerRepository.IsValid(gebruiker, paswoord))
            //        {
            //            MessageBox.Show("Foute login");
            //            TxtPaswoord.Password = string.Empty;
            //        }
            //        else
            //        {
            //            NavigationService.Navigate(new MagazijnierPage());
            //        }
            //        break;
            //    case "Kassierster":
            //        if (!DatabaseManager.Instance.GebruikerRepository.IsValid(gebruiker, paswoord))
            //        {
            //            MessageBox.Show("Foute login");
            //            TxtPaswoord.Password = string.Empty;
            //        }
            //        else
            //        {
            //            NavigationService.Navigate(new KassiersterPage());
            //        }

            //        break;
            //    case "Klant":
            //        if (!DatabaseManager.Instance.GebruikerRepository.IsValid(gebruiker, paswoord))
            //        {
            //            MessageBox.Show("Foute login");
            //            TxtPaswoord.Password = string.Empty;
            //        }
            //        else
            //        {
            //            NavigationService.Navigate(new KlantPage());
            //        }

            //        break;
            //}

        }


        private void BtnCancelLogin_OnClick(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
