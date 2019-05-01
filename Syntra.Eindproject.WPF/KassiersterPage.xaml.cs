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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager.Instance.BestellingRepository.InsertBestelling();
            NavigationService.Navigate(new BestellingPage());
        }
    }
}