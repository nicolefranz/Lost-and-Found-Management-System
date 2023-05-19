using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lost_and_Found_System
{
    public partial class Login : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";

        public Login()
        {
            InitializeComponent();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            Signup signup = new Signup();
            signup.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter a username and password.");
                return;
            }

            if (username == "admin" && password == "admin")
            {
                AdminHome adminHome = new AdminHome();
                adminHome.Show();
                this.Hide();
            }
            else
            {
                // Authenticate the user and retrieve the first name and last name
                string firstName;
                string lastName;
                bool isAuthenticated = AuthenticateUser(username, password, out firstName, out lastName);

                if (isAuthenticated)
                {
                    Home userHome = new Home(firstName, lastName);
                    userHome.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtUsername.Focus();

                }
            }
        }

        private bool AuthenticateUser(string username, string password, out string firstName, out string lastName)
        {
            firstName = string.Empty;
            lastName = string.Empty;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT First_Name, Last_Name FROM Users WHERE Username = @Username AND Password = @Password";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User is authenticated, retrieve the first name and last name
                                firstName = reader.GetString(reader.GetOrdinal("First_Name"));
                                lastName = reader.GetString(reader.GetOrdinal("Last_Name"));

                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return false;
        }
    }
}
