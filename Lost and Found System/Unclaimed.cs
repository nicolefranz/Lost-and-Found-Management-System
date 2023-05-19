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
    public partial class Unclaimed : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";

        private int itemId;
        private string itemName;
        private string landmark;
        private string dateFound;
        private string timeFound;
        private string reportedBy;
        private byte[] imageData;
        private string loggedInUserFirstName;
        private string loggedInUserLastName;

        public Unclaimed(int itemId, string itemName, string landmark, string dateFound, string timeFound, string reportedBy, byte[] imageData, string firstName, string lastName)
        {
            InitializeComponent();

            // Set the values in the respective controls
            this.itemId = itemId;
            this.itemName = itemName;
            this.landmark = landmark;
            this.dateFound = dateFound;
            this.timeFound = timeFound;
            this.reportedBy = reportedBy;
            this.imageData = imageData;
            this.loggedInUserFirstName = firstName;  // Update variable name
            this.loggedInUserLastName = lastName;  // Update variable name

            // Display the image in the PictureBox
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
        }




        private void btnBack_Click(object sender, EventArgs e)
        {
            Home home = new Home(loggedInUserFirstName, loggedInUserLastName);
            home.Show();
            this.Hide();
        }


        private void btnClaim_Click(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Items SET [Status] = @Status, [ClaimedBy] = @ClaimedBy WHERE [ItemId] = @ItemId";

                    using (OleDbCommand command = new OleDbCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Status", "Request Claimed");
                        command.Parameters.AddWithValue("@ClaimedBy", $"{loggedInUserFirstName} {loggedInUserLastName}");
                        command.Parameters.AddWithValue("@ItemId", itemId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Request Sent! \n\n Please go to the admin office to claim your belongings.");

                            Home home = new Home(loggedInUserFirstName, loggedInUserLastName);
                            home.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Item claim failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while claiming item: " + ex.Message);
            }
        }






    }
}
