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
    public partial class AddProjectForm : Form
    {
        ServerManager _mngr = new ServerManager();
        public event EventHandler OnAdd;
        public AddProjectForm()
        {
            InitializeComponent();
        }

        private void btnAddProj_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txbName.Text))
            {
                _mngr.AddProject(txbName.Text);
                if (OnAdd != null)
                    OnAdd.Invoke(this, new EventArgs());
                Close();
            }
        }
    }
}
