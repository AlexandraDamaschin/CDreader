using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace DVDCD
   
{
    [Serializable]
    public class FolderClass : ItemClass
    {
        public FolderType  FolderType { get; set; }
        public DirectoryInfo DirectoryInfo { get; set; }
        public List <FolderClass> FolderList { get; set; }
        public List <FileClass> FileList { get; set; }

        public FolderClass (string FullName, FolderType type, DirectoryInfo info, string DVDCDname)
        {
            Name = FullName;
            FolderType = type;
            DirectoryInfo = info;
            FullPath = info.FullName;
            DVD_CDname = DVDCDname;

            PopulateFolderList();
            PopulateFileList();

        }

        public FolderClass(string fullName, string fullPath)
        {
            Name = fullName;
            FolderType = FolderType.NormalFolder;
            DirectoryInfo = null;
            FullPath = fullPath;
        }


        private void PopulateFolderList ()
        {
            FolderList = new List<FolderClass>();
            var folders = DirectoryInfo.GetDirectories();
            if (folders.Count()!=0)
            {
                foreach (var folder in folders)
                {
                    FolderList.Add(new FolderClass(folder.Name, FolderType.NormalFolder, folder, DVD_CDname));
                }
            }
        }

        private void PopulateFileList ()
        {
            FileList = new List<FileClass>();
            var files = DirectoryInfo.GetFiles();
            foreach(var file in files)
            {
                FileList.Add(new FileClass(file.Name, file) { DVD_CDname = DVD_CDname });
            }
        }
        ~FolderClass() { }
        
    }
}
