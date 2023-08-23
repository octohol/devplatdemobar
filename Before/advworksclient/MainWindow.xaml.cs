using advworksdto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            List<Customer> customers = new List<Customer>();
            // Open a SqlConnection to (localdb)\MSQLLocalDB using the connection string in the App.config
            // The database name is AdvWorksDemoPrep
            string conString = ConfigurationManager.ConnectionStrings["AdvWorksDemoPrep"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string sql = @"SELECT CustomerID, FirstName, LastName, EmailAddress, Phone FROM SalesLT.Customer";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        con.Open();
                        // use a SqlDataReader to retrieve the list of customers from SalesLT.Customer
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // iterate through the rows, populating a Customer object for each row
                            while (await reader.ReadAsync())
                            {
                                Customer c = new Customer();
                                c.ID = reader.GetInt32(0);
                                c.FirstName = reader.GetString(1);
                                c.LastName = reader.GetString(2);
                                c.EmailAddress = reader.GetString(3);
                                c.Phone = reader.GetString(4);
                                customers.Add(c);
                            }
                        }
                    }
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
