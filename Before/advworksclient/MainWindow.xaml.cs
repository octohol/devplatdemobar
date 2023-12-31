﻿using advworksdto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            List<Customer> customers = new List<Customer>();
            // Open a SqlConnection to (localdb)\MSQLLocalDB using the connection string in the App.config
            // and download the customer data from the database AdvWorksDemoPrep
            string conString = ConfigurationManager.ConnectionStrings["AdvWorksDemoPrep"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    string sql = @"SELECT TOP 100 CustomerID, FirstName, LastName, EmailAddress, Phone FROM SalesLT.Customer";
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
