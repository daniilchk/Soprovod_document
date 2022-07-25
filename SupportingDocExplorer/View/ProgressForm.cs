using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupportingDocExplorer.View
{
    public partial class ProgressForm : Form
    {
        int _delay = 0;
        public ProgressForm(int delay)
        {
            InitializeComponent();
            _delay = delay;
            progressBar1.Maximum = delay;
            progressBar1.Step = 1;
            tmrProgress.Start();
        }

        private void tmrProgress_Tick(object sender, EventArgs e)
        {
            progressBar1.PerformStep();
            if (progressBar1.Value >= progressBar1.Maximum)
            {
                tmrProgress.Stop();
                Close();
            }
                
        }
    }
}
