using advworksdto;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using System.Windows;

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
            UpdateControls(false);
            if (await ValidateLogin(txtUserName.Text, txtPassword.SecurePassword))
            {
                if (await LoadData())
                {
                    this.Close();
                }
                else
                {
                    UpdateControls(true);
                }
            }
            else
            {
                MessageBox.Show("Invalid login.");
                UpdateControls(true);
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

        private async Task<bool> ValidateLogin(string name, SecureString password)
        {
            // validate login using MSAL and Azure AD
            // see https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-wpf

            // do a Thread.sleep to simulate the login process for 500 milliseconds
            await Task.Delay(500);

            if (!string.IsNullOrEmpty(name) && password.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void UpdateControls(bool state)
        {
            btnLogin.IsEnabled = state;
            txtUserName.IsEnabled = state;
            txtPassword.IsEnabled = state;
            lblStatus.Visibility = state ? Visibility.Hidden : Visibility.Visible;
        }

    }
}
