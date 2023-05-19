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
    public partial class AdminHome : Form
    {
        private string connectionString = $"Provider=Microsoft.ACE.OleDb.12.0;Data Source={Application.StartupPath}\\LostFound.accdb";
        private OleDbConnection connection;
        private OleDbDataAdapter adapter;
        private DataTable dt;

        public AdminHome()
        {
            InitializeComponent();
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
                        tblUnclaimed.DataSource = dt;
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
                            tblUnclaimed.DataSource = dt.DefaultView;
                        }
                    }
                }

                if (tblUnclaimed.Rows.Count != 0)
                {
                    tblUnclaimed.CurrentRow.Selected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while refreshing data: " + ex.Message);
            }
        }

        private void AdminHome_Load(object sender, EventArgs e)
        {
            LoadItems();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchtext = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchtext))
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = $"ItemName LIKE '%{searchtext}%'";
                tblUnclaimed.DataSource = dv;
            }
            else
            {
                tblUnclaimed.DataSource = dt;
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

        private void btnListUser_Click(object sender, EventArgs e)
        {
            ListUser list = new ListUser();
            list.Show();
            this.Hide();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            HistoryClaimed history = new HistoryClaimed();
            history.Show();
            this.Hide();
        }

        private void btnRequestClaimed_Click(object sender, EventArgs e)
        {
            RequestClaimed request = new RequestClaimed();
            request.Show();
            this.Hide();
        }
    }
}
