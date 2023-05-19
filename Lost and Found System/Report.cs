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
    public partial class Report : Form
    {

        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private string firstName;
        private string lastName;

        public Report(string firstName, string lastName)
        {
            InitializeComponent();
            this.firstName = firstName;
            this.lastName = lastName;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Home home = new Home(firstName, lastName);
            home.Show();
            this.Hide();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.png, *.jpeg, *.gif, *.bmp)|*.jpg;*.png;*.jpeg;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;
                itempicture.Image = Image.FromFile(imagePath);
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            string itemName = txtItemName.Text;
            string landmark = txtLandmark.Text;
            string dateFound = txtDate.Text;
            string reportedby = txtReported.Text;
            string timeFound = txtTime.Text;
            string status = "Unclaimed";

            // Get the image data from the PictureBox
            Image image = itempicture.Image;
            byte[] imageData = ImageToByteArray(image);

            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(landmark) || string.IsNullOrEmpty(dateFound) ||
        string.IsNullOrEmpty(timeFound) || string.IsNullOrEmpty(reportedby) || imageData == null)
            {
                MessageBox.Show("Please fill in all the fields and upload an image.");
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO Items (ItemName, Landmark, DateFound, TimeFound, ReportedBy, Picture, Status) " +
                                   "VALUES (@ItemName, @Landmark, @DateFound, @TimeFound, @ReportedBy, @Status, @ImageData)";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemName", itemName);
                        command.Parameters.AddWithValue("@Landmark", landmark);
                        command.Parameters.AddWithValue("@DateFound", dateFound);
                        command.Parameters.AddWithValue("@TimeFound", timeFound);
                        command.Parameters.AddWithValue("@ReportedBy", reportedby);
                        command.Parameters.AddWithValue("@Picture", imageData);
                        command.Parameters.AddWithValue("@Status", status);

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Report saved successfully!");

                    // Clear the input fields and picture box
                    txtItemName.Clear();
                    txtLandmark.Clear();
                    txtDate.Clear();
                    txtTime.Clear();
                    txtReported.Clear();
                    itempicture.Image = null;

                    Home home = new Home(firstName, lastName);
                    home.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg); 
                return ms.ToArray();
            }
        }

    }
}
