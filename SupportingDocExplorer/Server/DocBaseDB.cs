namespace SupportingDocExplorer.Server
{


    partial class DocBaseDB
    {
        public partial class DocTypesDataTable
        {
            public void ReadAll()
            {
                DocBaseDBTableAdapters.DocTypesTableAdapter _adapter = new DocBaseDBTableAdapters.DocTypesTableAdapter();
                _adapter.ClearBeforeFill = true;
                _adapter.Fill(this);
            }
        }

        public partial class FileLinksDataTable
        {
            public void InsertLinks(int projId, int fileId)
            {
                DocBaseDBTableAdapters.FileLinksTableAdapter _adapter = new DocBaseDBTableAdapters.FileLinksTableAdapter();
                _adapter.ClearBeforeFill = true;
                int id = _adapter.InsertLink(projId, fileId);
            }
        }

        public partial class FilesDataTable
        {

            public void AddFileToDoc(string fileName, string storagePath, int id)
            {
                DocBaseDBTableAdapters.FilesTableAdapter _adapter = new DocBaseDBTableAdapters.FilesTableAdapter();
                _adapter.UpdatePickedFile(fileName, storagePath, id);
            }
            public void DeleteByID(int id)
            {
                DocBaseDBTableAdapters.FilesTableAdapter _adapter = new DocBaseDBTableAdapters.FilesTableAdapter();
                _adapter.ClearBeforeFill = true;
                _adapter.DeleteByID(id);

            }
            public int InsertFile(int docType, string fileName, string storagePath)
            {
                DocBaseDBTableAdapters.FilesTableAdapter _adapter = new DocBaseDBTableAdapters.FilesTableAdapter();
                _adapter.ClearBeforeFill = true;
                int id = (int)_adapter.InsertDocFile(docType, fileName, storagePath);
                return id;
            }
            public int InsertEmptyFile(int docType)
            {
                DocBaseDBTableAdapters.FilesTableAdapter _adapter = new DocBaseDBTableAdapters.FilesTableAdapter();
                _adapter.ClearBeforeFill = true;
                int id = (int)_adapter.InsertEmptyDoc(docType);
                return id;
            }
            public void FillByProject(int id)
            {
                DocBaseDBTableAdapters.FilesTableAdapter _adapter = new DocBaseDBTableAdapters.FilesTableAdapter();
                _adapter.ClearBeforeFill = true;
                _adapter.FillByProject(this, id);
            }
        }

        public partial class ProjectsDataTable
        {
            public void ReadAll()
            {
                DocBaseDBTableAdapters.ProjectsTableAdapter _adapter = new DocBaseDBTableAdapters.ProjectsTableAdapter();
                _adapter.ClearBeforeFill = true;
                _adapter.Fill(this);
            }

            public void AddProject(string projName)
            {
                DocBaseDBTableAdapters.ProjectsTableAdapter _adapter = new DocBaseDBTableAdapters.ProjectsTableAdapter();
                _adapter.InsertProject(projName);
            }

            public void DeleteProject(int id)
            {
                DocBaseDBTableAdapters.ProjectsTableAdapter _adapter = new DocBaseDBTableAdapters.ProjectsTableAdapter();
                _adapter.DeleteProject(id);
            }

            public void SetApproved(int id)
            {
                DocBaseDBTableAdapters.ProjectsTableAdapter _adapter = new DocBaseDBTableAdapters.ProjectsTableAdapter();
                _adapter.UpdateApprove(System.DateTime.Now, id);
            }
        }
    }
}
