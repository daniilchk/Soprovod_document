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
    public partial class EditProject : Form
    {
        int _ID = -1;
        DocBaseDB.FilesDataTable _table;
        ServerManager _mngr = new ServerManager();
        BindingSource dataSource = new BindingSource();
        public event EventHandler OnApprove;
        private const int port = 8888;
        private const string server = "127.0.0.1";
        public EditProject(int id)
        {
            InitializeComponent();
            _ID = id;
            Text = "Проект #" + id;
            InitGrid();
        }

        private void InitGrid()
        {
            dgvDocuments.AutoGenerateColumns = false;
            _mngr.FillDocuments(_ID,ref _table);
            dataSource.DataSource = _table;
            dgvDocuments.Columns.Clear();
            dgvDocuments.DataSource = dataSource;

            dgvDocuments.Columns.Add(_table.Columns[0].ColumnName, "ID");
            dgvDocuments.Columns[_table.Columns[0].ColumnName].DataPropertyName = _table.Columns[0].ColumnName;

            dgvDocuments.Columns.Add(_table.Columns[1].ColumnName, "Название");
            dgvDocuments.Columns[_table.Columns[1].ColumnName].DataPropertyName = _table.Columns[1].ColumnName;

            dgvDocuments.Columns.Add(_table.Columns[3].ColumnName, "Тип документа");
            dgvDocuments.Columns[_table.Columns[3].ColumnName].DataPropertyName = _table.Columns[3].ColumnName;

            dgvDocuments.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    
            dgvDocuments.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvDocuments.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvDocuments.Enabled = true;
            dgvDocuments.Refresh();
        }

        public void UpdateSource(object sender, EventArgs e)
        {
            _mngr.FillDocuments(_ID,ref _table);
            dataSource.DataSource = _table;
            dgvDocuments.DataSource = dataSource;
        }

        private void dgvDocuments_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvDocuments.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddDocumentForm form = new AddDocumentForm(_ID);
            form.OnAddDoc += UpdateSource;
            form.Show();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count > 0)
            {
                int selectedRow = (int)dgvDocuments.SelectedRows[0].Cells[0].Value;

                if (dgvDocuments.SelectedRows[0].Cells[1].Value != null)
                {
                    DeleteFile(selectedRow);
                    int delay = 100;
                    
                    ProgressForm waiter = new ProgressForm(delay);
                    waiter.ShowDialog();
                    await Task.Delay(delay);
                    _mngr.DeleteFile(selectedRow);
                }
                else
                {
                    _mngr.DeleteFile(selectedRow);
                }
                UpdateSource(this, new EventArgs());

            }
        }
        string _filesFolder = ".\\Storage\\";
        private void btnPickFile_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count > 0)
            {
                int selectedRow = (int)dgvDocuments.SelectedRows[0].Cells[0].Value;

                string _path = "";
                string _name = "";
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        //txbFileName.Text = ofd.FileName;
                        _path = ofd.FileName;
                        _name = ofd.SafeFileName;

                        _mngr.AddFileToDoc( _name, _filesFolder + _name, selectedRow);
                        SendData(_name, _path);
                        UpdateSource(this, new EventArgs());
                    }
                }
            }
        }
        void SendData(string _name,string _path)
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
        private void btnApprove_Click(object sender, EventArgs e)
        {
            _mngr.SetApproved(_ID);
            if (OnApprove != null)
                OnApprove.Invoke(this, new EventArgs());
            Close();
        }

        void DeleteFile(int id)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);
                byte[] nameBuffer = new byte[256];
                byte operType = (byte)(2);
                for (int i = 0; i < nameBuffer.Length; i++)
                {
                    nameBuffer[i] = (byte)'\0';
                }
                nameBuffer[0] = operType;
                byte[] bytesName = BitConverter.GetBytes((int)id);
                for (int i = 0; i < bytesName.Length; i++)
                {
                    nameBuffer[i + 1] = bytesName[i];
                }

                NetworkStream stream = client.GetStream();

                stream.Write(nameBuffer, 0, nameBuffer.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

        }
        string _temp = Path.GetTempPath();
        void DownloadFile(int id)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(server, port);
                byte[] nameBuffer = new byte[256];
                byte operType = (byte)(3);
                for (int i = 0; i < nameBuffer.Length; i++)
                {
                    nameBuffer[i] = (byte)'\0';
                }
                nameBuffer[0] = operType;
                byte[] bytesName = BitConverter.GetBytes((int)id);
                for (int i = 0; i < bytesName.Length; i++)
                {
                    nameBuffer[i + 1] = bytesName[i];
                }

                //byte[] data = File.ReadAllBytes(_path);
                //byte[] allInfo = nameBuffer.Concat(data).ToArray();
                //StringBuilder response = new StringBuilder();
                NetworkStream stream = client.GetStream();

                stream.Write(nameBuffer, 0, nameBuffer.Length);
                List<byte> allBytes = new List<byte>();
                byte[] data = new byte[256];
                int bytes = 0;
                bool first = true;
                string name = "";
                do
                {
                    if (first)
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        name = Encoding.UTF8.GetString(data).Replace("\0", String.Empty);
                        first = false;
                    }
                    else
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        allBytes.AddRange(data);
                    }


                }
                while (stream.DataAvailable);

                File.WriteAllBytes(_temp + name, allBytes.ToArray());
                Process.Start(_temp + name);
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvDocuments.SelectedRows.Count > 0)
            {
                int selectedRow = (int)dgvDocuments.SelectedRows[0].Cells[0].Value;

                DownloadFile(selectedRow);

            }
            
        }
    }
}
