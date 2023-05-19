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
    public partial class UserClaimHistory : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;
        private string firstName;
        private string lastName;
        public UserClaimHistory(string firstName, string lastName)
        {
            InitializeComponent();
            this.firstName = firstName;
            this.lastName = lastName;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Home home = new Home(firstName,lastName);
            home.Show();
            this.Hide();
        }

        private void LoadItems()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    string fullName = $"{firstName} {lastName}";
                    string query = $"SELECT [ItemId], [ItemName], [Landmark], [DateFound], [TimeFound], [ReportedBy], [Status] FROM Items WHERE [ClaimedBy] = '{fullName}'";

                    using (adapter = new OleDbDataAdapter(query, connection))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        tbluserhistory.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while loading items: " + ex.Message);
            }
        }



        private void RefreshData()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM Items ORDER BY ItemName ASC", connection))
                    {
                        dt = new DataTable();
                        using (adapter = new OleDbDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                            tbluserhistory.DataSource = dt.DefaultView;
                        }
                    }
                }

                if (tbluserhistory.Rows.Count != 0)
                {
                    tbluserhistory.CurrentRow.Selected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while refreshing data: " + ex.Message);
            }
        }

        private void UserClaimHistory_Load(object sender, EventArgs e)
        {
            LoadItems();
        }
    }
}
