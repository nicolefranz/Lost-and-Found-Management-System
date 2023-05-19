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
    public partial class Home : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;
        private string firstName;
        private string lastName;


        public Home(string firstName, string lastName)
        {
            InitializeComponent();
            this.firstName = firstName;
            this.lastName = lastName;
        }

        private void LoadItems()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    using (adapter = new OleDbDataAdapter("SELECT [ItemId],[ItemName], [Landmark], [DateFound], [TimeFound], [ReportedBy] FROM Items WHERE [Status] = 'Unclaimed'", connection))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        tblUnclaimedItems.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while loading items: " + ex.Message);
            }
        }



        public void RefreshData()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    using (adapter = new OleDbDataAdapter("SELECT [ItemId],[ItemName], [Landmark], [DateFound], [TimeFound], [ReportedBy] FROM Items WHERE [Status] = 'Unclaimed'", connection))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        tblUnclaimedItems.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while refreshing data: " + ex.Message);
            }
        }



        private void btnLogout_Click(object sender, EventArgs e)
        {
            string message = "Are you sure you want to logout?";
            string title = "Logout";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            Report report = new Report(firstName, lastName);
            report.Show();
            this.Hide();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            LoadItems();
            tblUnclaimedItems.CellClick += tblUnclaimedItems_CellClick;

        }

        private void tblUnclaimedItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tblUnclaimedItems.Rows[e.RowIndex];

                int itemId = Convert.ToInt32(row.Cells["ItemId"].Value);
                string itemName = row.Cells["ItemName"].Value.ToString();
                string landmark = row.Cells["Landmark"].Value.ToString();
                string dateFound = row.Cells["DateFound"].Value.ToString();
                string timeFound = row.Cells["TimeFound"].Value.ToString();
                string reportedBy = row.Cells["ReportedBy"].Value.ToString();

                byte[] imageData = GetImageData(itemId);

                Unclaimed unclaimedForm = new Unclaimed(itemId, itemName, landmark, dateFound, timeFound, reportedBy, imageData, firstName, lastName);
                unclaimedForm.Show();
            }
        }

        private byte[] GetImageData(int itemId)
        {
            byte[] imageData = null;

            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // Query the database to fetch the image data
                    string query = "SELECT [Picture] FROM Items WHERE [ItemId] = @ItemId";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", itemId);
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    // Check if the image data is not null
                                    imageData = (byte[])reader["Picture"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while fetching image data: " + ex.Message);
            }

            return imageData;
        }

        private void btnClaim_Click(object sender, EventArgs e)
        {
            Claimed claimed = new Claimed();
            claimed.Show();
            this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchtext = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchtext))
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = $"ItemName LIKE '%{searchtext}%'";
                tblUnclaimedItems.DataSource = dv;
            }
            else
            {
                tblUnclaimedItems.DataSource = dt;
            }
        }

        private void btnClaimHistory_Click(object sender, EventArgs e)
        {
            UserClaimHistory claimhistory = new UserClaimHistory(firstName, lastName);
            claimhistory.Show();
            this.Hide();
        }
    }
}
