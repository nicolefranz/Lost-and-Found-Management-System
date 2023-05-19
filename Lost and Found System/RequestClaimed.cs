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
    public partial class RequestClaimed : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;

        public RequestClaimed()
        {
            InitializeComponent();
        }

        private void LoadItems()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    using (adapter = new OleDbDataAdapter("SELECT [ItemId],[ItemName], [Landmark], [DateFound], [TimeFound], [ReportedBy], [ClaimedBy], [Status] FROM Items WHERE [Status] = 'Request Claimed'", connection))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        tblRequestClaimed.DataSource = dt;
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
                            tblRequestClaimed.DataSource = dt.DefaultView;
                        }
                    }
                }

                if (tblRequestClaimed.Rows.Count != 0)
                {
                    tblRequestClaimed.CurrentRow.Selected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while refreshing data: " + ex.Message);
            }
        }
        private void RequestClaimed_Load(object sender, EventArgs e)
        {
            LoadItems();
            tblRequestClaimed.CellClick += tblRequestClaimed_CellClick;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            AdminHome admin = Application.OpenForms.OfType<AdminHome>().FirstOrDefault();
            if (admin != null)
            {
                admin.Show();
                
            }
            else
            {
                admin = new AdminHome();
                admin.Show();
            }

            this.Close();
        }


        private void tblRequestClaimed_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = tblRequestClaimed.Rows[e.RowIndex];

                // Retrieve the data from the clicked row
                int itemId = (int)row.Cells["ItemId"].Value;
                string itemName = row.Cells["ItemName"].Value.ToString();
                string landmark = row.Cells["Landmark"].Value.ToString();
                string dateFound = row.Cells["DateFound"].Value.ToString();
                string timeFound = row.Cells["TimeFound"].Value.ToString();
                string reportedBy = row.Cells["ReportedBy"].Value.ToString();
                string claimedBy = row.Cells["ClaimedBy"].Value.ToString();

                byte[] imageData = GetImageData(itemId);

                // Open the ClaimForm and pass the data
                ClaimForm claimForm = new ClaimForm(itemId.ToString(), itemName, landmark, dateFound, timeFound, reportedBy, claimedBy, imageData);
                claimForm.ShowDialog();

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
    }
}
