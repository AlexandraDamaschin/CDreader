using System;
using System.IO;

namespace DVDCD
{
   [Serializable]
    public class FileClass : ItemClass
    {
        public FileInfo FileInfo  { get; set; }

        public FileClass(string FullName, FileInfo fileInfo)
        {
            Name = FullName;
            FullPath = fileInfo.FullName;
            FileInfo = fileInfo;
        }

        public FileClass (string FullName, string fullPath)
        {
            Name = FullName;
            FileInfo = null;
            FullPath = fullPath;
        }
        ~FileClass() { }
    }
}
