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
    public partial class ListUser : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;

        public ListUser()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            AdminHome admin = new AdminHome();
            admin.Show();
            this.Hide();
        }

        private void LoadUser()
        {
            try
            {
                using (connection = new OleDbConnection(connectionString))
                {
                    using (adapter = new OleDbDataAdapter("SELECT [First_Name],[Last_Name], [Username] FROM Users ", connection))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                        tblUser.DataSource = dt;
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
                    using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM Users ORDER BY First_Name ASC", connection))
                    {
                        dt = new DataTable();
                        using (adapter = new OleDbDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                            tblUser.DataSource = dt.DefaultView;
                        }
                    }
                }

                if (tblUser.Rows.Count != 0)
                {
                    tblUser.CurrentRow.Selected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while refreshing data: " + ex.Message);
            }
        }

        private void ListUser_Load(object sender, EventArgs e)
        {
            LoadUser();
        }
    }
}
