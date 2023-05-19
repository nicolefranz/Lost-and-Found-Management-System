using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lost_and_Found_System
{
    public partial class ClaimForm : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;

        // Declare variables to store the transferred data
        private string itemId;
        private string itemName;
        private string landmark;
        private string dateFound;
        private string timeFound;
        private string reportedBy;
        private string claimedBy;
        private byte[] imageData;

        public ClaimForm(string itemId, string itemName, string landmark, string dateFound, string timeFound, string reportedBy, string claimedBy, byte[] imageData)
        {
            InitializeComponent();
            this.itemId = itemId;
            this.itemName = itemName;
            this.landmark = landmark;
            this.dateFound = dateFound;
            this.timeFound = timeFound;
            this.reportedBy = reportedBy;
            this.claimedBy = claimedBy;
            this.imageData = imageData;

            if (imageData != null && imageData.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    itempicture.Image = Image.FromStream(ms);
                }
            }

            txtItemName.Text = itemName;
            txtLandmark.Text = landmark;
            txtDate.Text = dateFound;
            txtTime.Text = timeFound;
            txtReported.Text = reportedBy;
            txtClaimedby.Text = claimedBy;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            RequestClaimed request = new RequestClaimed();
            request.Show();
            this.Hide();
        }

        private void btnClaimed_Click(object sender, EventArgs e)
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    string dateClaimed = DateTime.Now.ToString("MMMM d, yyyy");
                    // Update the status column to "Claimed" and set the claimed date
                    string query = "UPDATE Items SET [Status] = 'Claimed', [DateClaimed] = @DateClaimed WHERE [ItemId] = @ItemId";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        

                        command.Parameters.AddWithValue("@DateClaimed", dateClaimed);
                        command.Parameters.AddWithValue("@ItemId", itemId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Item claimed successfully!");

                            // Close the ClaimForm and show the AdminHome form
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

                            // Close the RequestClaimed form if it is open
                            Form requestClaimedForm = Application.OpenForms["RequestClaimed"];
                            if (requestClaimedForm != null)
                            {
                                requestClaimedForm.Close();
                            }

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to claim the item.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while claiming the item: " + ex.Message);
            }
        }



    }
}
