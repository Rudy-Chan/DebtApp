using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debt
{
    class Data_File
    {
        private string id;
        private string name;
        private string directory;
        private string size;

        public string Id { get { return id; } }
        public string Name { get { return name; } }
        public string Directory { get { return directory; } }
        public string Size { get { return size; } }
        public Data_File(string id, string path)
        {
            this.id = id;
            name = System.IO.Path.GetFileName(path);
            size = ConvertSize(new FileInfo(path).Length);
            directory = System.IO.Path.GetDirectoryName(path);
        }

        public string ConvertSize(long length)
        {
            string newSize = "";
            double size = (int)(length / 1024.0 / 1024.0 * 100) / 100.0;
            if (size < 1)
            {
                size = (int)(length / 1024.0 * 100) / 100.0;
                newSize = size + "KB";
            }
            else if (size > 1000)
            {
                size = (int)(length / 1024.0 / 1024.0 / 1024.0 * 10) / 10.0;
                newSize = size + "GB";
            }
            else
            {
                newSize = size + "MB";
            }
            return newSize;
        }
    }
}
