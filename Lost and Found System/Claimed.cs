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
    public partial class Claimed : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;
        private string firstName;
        private string lastName;

        public Claimed()
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

        private void LoadItems()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    using (adapter = new OleDbDataAdapter("SELECT [ItemId],[ItemName], [DateFound], [DateClaimed], [ClaimedBy] FROM Items WHERE [Status] = 'Claimed'", connection))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        tblClaimedItems.DataSource = dt;
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
                            tblClaimedItems.DataSource = dt.DefaultView;
                        }
                    }
                }

                if (tblClaimedItems.Rows.Count != 0)
                {
                    tblClaimedItems.CurrentRow.Selected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while refreshing data: " + ex.Message);
            }
        }

        private void Claimed_Load(object sender, EventArgs e)
        {
            LoadItems();
        }
    }
}
