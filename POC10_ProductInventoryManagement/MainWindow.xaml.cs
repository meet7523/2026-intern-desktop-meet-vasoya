using POC10_ProductInventoryManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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

namespace POC10_ProductInventoryManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        public ObservableCollection<product> Products = new ObservableCollection<product>();
        private product _selectedProduct;

        public MainWindow()
        {
            InitializeComponent();

            dataGridProducts.ItemsSource = Products;
            DataContext = this;

        }

        private void UpdateGrandTotal()
        {
            txtTotalValue.Text = Products.Sum(p => p.Total).ToString();
        }

        public product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
            }
        }


        public void Button_ClickSave(object sender, RoutedEventArgs e)
        {
           

            if (!int.TryParse(txtId.Text, out int id) || string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            int productId = Convert.ToInt32(txtId.Text);

            if (Products.Any(p => p.Id == productId))
            {
                MessageBox.Show("Product ID already exists.", "Duplicate Product ID", MessageBoxButton.OK, MessageBoxImage.Warning);

                txtId.Focus();
                txtId.SelectAll();
                return;
            }


            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }


            product Product = new product
            {           
                Id = int.Parse(txtId.Text),
                Name = txtName.Text,
                Price = decimal.Parse(txtPrice.Text),
                Quantity = int.Parse(txtQuantity.Text)
            };

            Product.PropertyChanged += Product_PropertyChanged;
            this.Products.Add(Product);

            UpdateGrandTotal();

            MessageBox.Show("Product saved successfully!");
        }

        private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(product.Price) || e.PropertyName == nameof(product.Quantity))
            {
                UpdateGrandTotal();
            }
        }

        private void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateGrandTotal();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_ClickClear(object sender, RoutedEventArgs e)
        {
            txtId.Clear();
            txtName.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
        }

        private void Button_ClickRemove(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct != null)
            {
                Products.Remove(SelectedProduct);
                MessageBox.Show("Product removed successfully!");

                UpdateGrandTotal();
            }
        }
    }
}
