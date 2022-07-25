using SupportingDocExplorer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupportingDocExplorer.View
{
    
    public partial class AddDocumentForm : Form
    {
        public event EventHandler OnAddDoc;
        DocBaseDB.FilesDataTable _table;
        ServerManager _mngr = new ServerManager();
        BindingSource dataSource = new BindingSource();
        string _path = "";
        string _name = "";
        int _fileSize = 0;
        private const int port = 8888;
        private const string server = "127.0.0.1";
        string _temp = Path.GetTempPath();
        int _projID = -1;
        public AddDocumentForm(int projID)
        {
            InitializeComponent();
            InitCombobox();
            _projID = projID;
        }

        #region ComboBox
        private void InitCombobox()
        {
            cmbTypes.DrawMode = DrawMode.OwnerDrawFixed;
            toolTip1.IsBalloon = true;
            cmbTypes.DrawItem += comboBox1_DrawItem;
            cmbTypes.DropDownClosed += comboBox1_DropDownClosed;
            List<ListItem> types = _mngr.CreateTypeList();
            cmbTypes.DataSource = types;
            cmbTypes.DisplayMember = "Value";
            cmbTypes.ValueMember = "Key";
        }
        ToolTip toolTip1 = new ToolTip();

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            toolTip1.Hide(cmbTypes);
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; } // added this line thanks to Andrew's comment
            string text = cmbTypes.GetItemText(cmbTypes.Items[e.Index]);
            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, br, e.Bounds); 
            }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { toolTip1.Show(text, cmbTypes, e.Bounds.Right, e.Bounds.Bottom); }
            e.DrawFocusRectangle();
        }
        #endregion

        private void btnPickFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txbFileName.Text = ofd.FileName;
                    _path = ofd.FileName;
                    _name = ofd.SafeFileName;
                    _fileSize = (int)new System.IO.FileInfo(_path).Length;
                }
            }
        }
        string _filesFolder = ".\\Storage\\";
        private void btnAddFile_Click(object sender, EventArgs e)
        {
            var selected = (ListItem)cmbTypes.SelectedItem;
            if (!string.IsNullOrEmpty(_path))
            {
                if (selected != null)
                {
                    _mngr.InsertFullDoc(selected.Key,_name,_filesFolder+_name,_projID);
                    SendData();
                }

            }
            else
            {
                
                if (selected != null)
                    _mngr.InsertEmptyDoc(selected.Key,_projID);
            }

            if (OnAddDoc != null)
                OnAddDoc.Invoke(this, new EventArgs());

            Close();
        }

        void SendData()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);
                byte[] nameBuffer = new byte[256];
                byte operType = (byte)(1);
                //nameBytes= Encoding.UTF8.GetBytes(_name);
                for (int i = 0; i < nameBuffer.Length; i++)
                {
                    nameBuffer[i] = (byte)'\0';
                }
                nameBuffer[0] = operType;
                byte[] bytesName = Encoding.UTF8.GetBytes(_name);
                for (int i = 0; i < bytesName.Length; i++)
                {
                    nameBuffer[i + 1] = bytesName[i];
                }

                byte[] data = File.ReadAllBytes(_path);
                byte[] allInfo = nameBuffer.Concat(data).ToArray();
                //StringBuilder response = new StringBuilder();
                NetworkStream stream = client.GetStream();

                stream.Write(allInfo, 0, allInfo.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }


    }
}
