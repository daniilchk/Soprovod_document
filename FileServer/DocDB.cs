namespace FileServer
{


    partial class DocDB
    {
        partial class FilesDataTable
        {
            public void SetPath(int fileID, string path)
            {

            }

            public void DeletePath(int fileID)
            {

            }

            public string GetPath(int fileId)
            {
                DocDBTableAdapters.FilesTableAdapter _adapter = new DocDBTableAdapters.FilesTableAdapter();
                _adapter.FillById(this, fileId);
                var row = this[0];
                return row.FileName;

            }
        }
    }
}
