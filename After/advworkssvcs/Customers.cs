using advworksdto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace advworkssvcs
{
    public static class Customers
    {

        [FunctionName("GetAllCustomers")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                // access SQL Database
                // string connectionString = "Server=tcp:octobrian-wus3-sqlsvr.database.windows.net,1433;Initial Catalog=advworksprod;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
                string connectionString = "Server=tcp:octobrian-dev-env1-sqlsvr-wus3.database.windows.net,1433;Initial Catalog=advworksdev;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    connectionString = "Server=tcp:octobrian-dev-env1-sqlsvr-wus3.database.windows.net,1433;Authentication=Active Directory Managed Identity;Database=advworksdev";
                }

                // return list of customers
                List<Customer> customers = new List<Customer>();

                // Create a SqlConnection from the provided connection string.
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Formulate the command.
                    SqlCommand command = new SqlCommand("SELECT TOP 100 CustomerID, FirstName, LastName, EmailAddress, Phone FROM SalesLT.Customer", connection);

                    // Open the connection.
                    await connection.OpenAsync();

                    // Read from the database.
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        // get the results of each column
                        int customerID = reader.GetInt32(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);
                        string emailAddress = reader.GetString(3);
                        string phone = reader.GetString(4);

                        // create a new Customer object
                        Customer customer = new Customer();
                        customer.ID = customerID;
                        customer.FirstName = firstName;
                        customer.LastName = lastName;
                        customer.EmailAddress = emailAddress;
                        customer.Phone = phone;

                        // add the customer to the list
                        customers.Add(customer);
                    }
                    // Close the connection.
                    await connection.CloseAsync();
                }

                return new OkObjectResult(customers);

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);              

                return new BadRequestObjectResult(ex.Message);
            }        
        }
    }
}
