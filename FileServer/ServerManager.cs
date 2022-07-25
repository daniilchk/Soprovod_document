using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class ServerManager
    {
        public void SetPathToDoc(int id,string path)
        {

        }

        public void ClearPath(int id)
        {

        }

        public string GetFileName(int id)
        {
            DocDB.FilesDataTable table = new DocDB.FilesDataTable();

            return table.GetPath(id);
        }
    }
}
