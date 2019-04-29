using Syntra.Eindproject.BL;
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
using Syntra.Eindproject.Dapper;

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

        #region Initialize
        private void Initialize()
        {
            TxtNaam.Text = string.Empty;
            TxtSoort.Text = string.Empty;
            TxtOorsprong.Text = string.Empty;
            TxtId.Text = string.Empty;
            TxtPrijs.Text = string.Empty;
            TxtEenheid.Text = string.Empty;
            TxtVervalDatum.Text = string.Empty;
            TxtStock.Text = string.Empty;

            List<Product> products = DatabaseManager.Instance.ProductRepository.GetProducts().ToList();

            LbProducts.ItemsSource = products;                           //Listview DisplayMemberBinding

            //foreach (Product product in products)
            //{
            //    ListViewItem item = new ListViewItem
            //    {
            //        Tag = product,
            //        Content = product.Naam
            //    };
            //    LbProducts.Items.Add(item);
            //};
        }
        #endregion
        #region AddProduct
        private void BtnVoegProductToe_Click(object sender, RoutedEventArgs e)
        {
            double.TryParse(TxtPrijs.Text, out double prijs);
            int.TryParse(TxtId.Text, out int id);
            double.TryParse(TxtStock.Text, out double stock);
            
            try
            {
                DatabaseManager.Instance.ProductRepository.InsertProduct(id, TxtNaam.Text, TxtSoort.Text, TxtOorsprong.Text, prijs, TxtEenheid.Text, TxtVervalDatum.Text, stock);
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            

            Initialize();
        }
        #endregion
        #region GetCurrentProduct
        private void LbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = GetSelectedProduct();

            if (product != null)
            {
                TxtId.Text = product.Id.ToString();
                TxtNaam.Text = product.Naam;
                TxtSoort.Text = product.Soort;
                TxtOorsprong.Text = product.Oorsprong;
                TxtPrijs.Text = product.Prijs.ToString();
                TxtEenheid.Text = product.Eenheid;
                TxtVervalDatum.Text = product.VervalDatum;
                TxtStock.Text = product.Stock.ToString();
            }
        }
        #endregion
        #region GetSelectedProduct
        private Product GetSelectedProduct()
        {
            //if (LbProducts.SelectedItem != null)
            //{
            //    ListViewItem item = (ListViewItem)LbProducts.SelectedItem;
            //    Product product = (Product)item.Tag;

            //    return product;
            //}
            //return null;

            Product current = LbProducts.SelectedItem as Product;

            return current;
        }
        #endregion
        #region DeleteProduct
        private void BtnWisProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = GetSelectedProduct();

            if (product != null)
            {
                DatabaseManager.Instance.ProductRepository.DeleteProduct(product.Id, product.Naam);

                Initialize();
            }

        }
        #endregion

        #region UpdateProduct
        private void BtnUpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = GetSelectedProduct();

            double.TryParse(TxtPrijs.Text, out double prijs);
            int.TryParse(TxtId.Text, out int id);
            double.TryParse(TxtStock.Text, out double stock);

            try
            {
                DatabaseManager.Instance.ProductRepository.UpdateProduct(id, TxtNaam.Text, TxtSoort.Text,TxtOorsprong.Text, prijs, TxtEenheid.Text, TxtVervalDatum.Text, stock );
            }
            catch (BusinessException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            Initialize();
        }
        #endregion

        private void CbVervallen_Checked(object sender, RoutedEventArgs e)
        {
            CbAlles.IsChecked = false;
            List<Product> products = DatabaseManager.Instance.ProductRepository.GetBadProduct().ToList();
            LbProducts.ItemsSource = products;
        }

        private void CbAlles_Checked(object sender, RoutedEventArgs e)
        {
            CbVervallen.IsChecked = false;
            Initialize();
        }

        private void BtnGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void MagazijnierPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }
    }
}
