﻿using Syntra.Eindproject.BL;
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

namespace Syntra.Eindproject.WPF
{
    /// <summary>
    /// Interaction logic for MagazijnierPage.xaml
    /// </summary>
    public partial class MagazijnierPage : Page
    {
        public MagazijnierPage()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            TxtNaam.Text = string.Empty;
            TxtSoort.Text = string.Empty;
            TxtOorsprong.Text = string.Empty;

            List<Product> products = DatabaseManager.Instance.ProductRepository.GetProducts().ToList();

            foreach (Product product in products)
            {
                ListViewItem item = new ListViewItem
                {
                    Tag = product,
                    Content = product.Naam
                };
                LbProducts.Items.Add(item);
            };
        }

        private void BtnVoegProductToe_Click(object sender, RoutedEventArgs e)
        {
            Product product = new Product(TxtNaam.Text, TxtSoort.Text, TxtOorsprong.Text);
            DatabaseManager.Instance.ProductRepository.InsertProduct(product);

            Initialize();
        }

        private void LbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = GetSelectedProduct();

            if (product != null)
            {
                TxtNaam.Text = product.Naam;
                TxtSoort.Text = product.Soort;
                TxtOorsprong.Text = product.Oorsprong;
            }
        }

        private Product GetSelectedProduct()
        {
            if (LbProducts.SelectedItem != null)
            {
                ListViewItem item = (ListViewItem)LbProducts.SelectedItem;
                Product product = (Product)item.Tag;

                return product;
            }
            return null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
    }
}
