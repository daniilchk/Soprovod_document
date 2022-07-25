using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportingDocExplorer.Server
{
    public class ServerManager
    {
        public List<ListItem> CreateTypeList()
        {
            List<ListItem> result = new List<ListItem>();
            DocBaseDB.DocTypesDataTable table = new DocBaseDB.DocTypesDataTable();
            table.ReadAll();
            foreach(DocBaseDB.DocTypesRow row in table)
            {
                result.Add(new ListItem(row.TypeID, row.TypeName));
            }
            return result;
        }

        public void AddFileToDoc(string fileName,string storagePath,int id)
        {
            DocBaseDB.FilesDataTable table = new DocBaseDB.FilesDataTable();
            table.AddFileToDoc(fileName,storagePath,id);

        }
        public void InsertEmptyDoc(int type,int projID)
        {
            DocBaseDB.FilesDataTable table = new DocBaseDB.FilesDataTable();
            int id=table.InsertEmptyFile(type);
            SetLink(projID, id);
        }
        public void InsertFullDoc(int type,string name,string path,int projID)
        {
            DocBaseDB.FilesDataTable table = new DocBaseDB.FilesDataTable();
            int id=table.InsertFile(type, name, path);
            SetLink(projID, id);
        }

        public void SetLink(int projID,int fileID)
        {
            DocBaseDB.FileLinksDataTable table = new DocBaseDB.FileLinksDataTable();
            table.InsertLinks(projID, fileID);
        }
        public void FillProjects(ref DocBaseDB.ProjectsDataTable table)
        {
            table = new DocBaseDB.ProjectsDataTable();
            table.ReadAll();
        }

        public void SetApproved(int id)
        {
            DocBaseDB.ProjectsDataTable table = new DocBaseDB.ProjectsDataTable();
            table.SetApproved(id);
        }

        public void DeleteFile(int id)
        {
            DocBaseDB.FilesDataTable _table = new DocBaseDB.FilesDataTable();
            _table.DeleteByID(id);
        }
        public void AddProject(string name)
        {
            DocBaseDB.ProjectsDataTable _table = new DocBaseDB.ProjectsDataTable();
            _table.AddProject(name);
        }

        public void DeleteProject(int id)
        {
            DocBaseDB.ProjectsDataTable _table = new DocBaseDB.ProjectsDataTable();
            _table.DeleteProject(id);
        }

        public void FillDocuments(int projID,ref DocBaseDB.FilesDataTable table)
        {
            table = new DocBaseDB.FilesDataTable();
            table.FillByProject(projID);
        }
    }

    public class ListItem
    {
        int _key;
        string _value;
        public ListItem(int key,string value)
        {
            _key = key;
            _value = value;
        }
        public int Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
