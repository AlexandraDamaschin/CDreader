using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace DVDCD
{
    [Serializable]
    public  class DVDCD : ItemClass
    {
        public DirectoryInfo RootDirectory { get; set; }
        public long TotalSpace { get; set; }
        public long TotalFreeSpace { get; set; }
        public string VolumeLabel { get; set; }
        public List<FileClass> FileList { get; set; }
        public List<FolderClass> FolderList { get; set; }

        public DVDCD (string name, DirectoryInfo rootDirectory, long totalSpace, long totalFreeSpace, string volumeLabel)
        {
            Name = name;
            DVD_CDname =volumeLabel;
            FullPath = rootDirectory.FullName;
            RootDirectory = rootDirectory;
            TotalFreeSpace = totalFreeSpace;
            TotalSpace = totalSpace;
            VolumeLabel = volumeLabel;

            PopulateFolderList();
            PopulateFileList();
        }

        private void PopulateFolderList ()
        {
            FolderList = new List<FolderClass>();
            var folders = RootDirectory.GetDirectories();
            if (folders.Count()!=0)
            {
                foreach (var folder in folders)
                {
                    FolderList.Add(new FolderClass(folder.Name, FolderType.NormalFolder, folder, DVD_CDname));
                }
            }
            else
            {
                 FolderList.Add(new FolderClass(Helpers.DefaultFolderFullName, FolderType.DefaultFolder, RootDirectory, DVD_CDname));
            }
        }

        private void PopulateFileList ()
        {
            FileList = new List<FileClass>();
            var files = RootDirectory.GetFiles();
            
            foreach (var file in files)
            {
                FileList.Add(new FileClass(file.Name, file)
                {
                    DVD_CDname = DVD_CDname
                });
            }
        }
        ~DVDCD() { }

    }
}
