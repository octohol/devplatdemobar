using advworksdto;
using System.Collections.Generic;
using System.Windows;

namespace advworksclient
{
    /// <summary>
    /// Interaction logic for CustomersWindow.xaml
    /// </summary>
    public partial class CustomersWindow : Window
    {
        public CustomersWindow(List<Customer> customers)
        {
            InitializeComponent();

            dgCustomers.ItemsSource = customers;
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("AdvWorks Client v1.0");
        }
    }
}
