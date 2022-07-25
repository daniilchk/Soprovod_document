using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SupportingDocExplorer.Server;

namespace SupportingDocExplorer.View
{
    public partial class MainForm : Form
    {
        DocBaseDB.ProjectsDataTable _table;
        ServerManager _mngr = new ServerManager();
        BindingSource dataSource = new BindingSource();
        public MainForm()
        {
            InitializeComponent();
            InitGrid();
        }

        private void InitGrid()
        {
            _mngr.FillProjects(ref _table);
            dataSource.DataSource = _table;
            dgvProjects.Columns.Clear();
            dgvProjects.DataSource = dataSource;

            dgvProjects.Columns[0].DataPropertyName = _table.Columns[0].ColumnName;
            dgvProjects.Columns[0].HeaderText = "ID";
            dgvProjects.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvProjects.Columns[1].DataPropertyName = _table.Columns[1].ColumnName;
            dgvProjects.Columns[1].HeaderText = "Название";
            dgvProjects.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvProjects.Columns[2].DataPropertyName = _table.Columns[2].ColumnName;
            dgvProjects.Columns[2].HeaderText = "Подтвержден";
            dgvProjects.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //for (int i = 0; i < dgvProjects.Columns.Count; i++)
            //{
            //    dgvProjects.Columns[i].DataPropertyName = _table.Columns[i].ColumnName;
            //    dgvProjects.Columns[i].HeaderText = _table.Columns[i].Caption;
            //}
            dgvProjects.Enabled = true;
            dgvProjects.Refresh();
        }

        public void UpdateSource(object sender, EventArgs e)
        {
            _mngr.FillProjects(ref _table);
            dataSource.DataSource = _table;
            dgvProjects.DataSource = dataSource;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddProjectForm form = new AddProjectForm();
            form.OnAdd += UpdateSource;
            form.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProjects.SelectedRows.Count > 0)
            {
                int selectedRow = (int)dgvProjects.SelectedRows[0].Cells[0].Value;
                _mngr.DeleteProject(selectedRow);
                UpdateSource(this, new EventArgs());
            }
            
            
        }

        private void dgvProjects_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvProjects.ClearSelection();
        }

        private void dgvProjects_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dgvProjects.SelectedRows.Count > 0)
            {
                int selectedRow = (int)dgvProjects.SelectedRows[0].Cells[0].Value;
                EditProject form = new EditProject(selectedRow);
                form.OnApprove += UpdateSource;
                form.ShowDialog();
            }
        }
    }
}
