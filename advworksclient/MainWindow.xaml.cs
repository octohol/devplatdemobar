using advworksdto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace advworksclient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnLogin_ClickAsync(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtPassword.IsEnabled = false;
            lblStatus.Visibility = Visibility.Visible;

            bool r = await LoadData();

            if (r)
            {
                this.Close();
            }
        }

        private async Task<bool> LoadData()
        {
            // call http://localhost:7201/api/GetAllCustomers and return a list of customers and store it in a List<Customer>
            List<Customer> customers = new List<Customer>();
            // open an HTTP Connection to http://localhost:7201/api/GetAllCustomers
            // read the response and store it in a List<Customer>

            // display the list of customers in the DataGrid
            try
            {
                string urlToUse = "http://localhost:7201/";
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    urlToUse = "https://advworkssvcs.azurewebsites.net";
                }
                var client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri(urlToUse);
                var response = await client.GetAsync("/api/GetAllCustomers");
                if (response.IsSuccessStatusCode)
                {
                    var customersJson = response.Content.ReadAsStringAsync().Result;
                    customers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer>>(customersJson);
                }

                int count = customers.Count;
                if (count > 0)
                {
                    CustomersWindow customersWindow = new CustomersWindow(customers);
                    customersWindow.Title = $"Customer Count: {count}";
                    customersWindow.Show();
                    return true;
                }
                else
                {
                    MessageBox.Show("No customers found.");
                    return false;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Error retrieving customers.");
                return false;
            }


            
        }

    }
}
