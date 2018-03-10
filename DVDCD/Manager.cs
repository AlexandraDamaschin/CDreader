using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DVDCD
{
    public class Manager
    {
        public List<DVDCD> drivesList;

        
        public List<DVDCD> CheckDrives()
        {
           drivesList = new List<DVDCD>();

            DriveInfo[] drives = DriveInfo.GetDrives();

            if (drives == null || drives.Count() == 0)
            {
                throw new Exception();
            }


            
            foreach (var drive in drives)
            {
                if (drive.DriveType == DriveType.CDRom && drive.IsReady) 
                {
                    drivesList.Add(new DVDCD(drive.Name, drive.RootDirectory, drive.TotalSize, drive.TotalFreeSpace, drive.VolumeLabel)); 
                }
            }
            return drivesList;

        }
    }
}
