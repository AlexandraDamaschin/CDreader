using System;

namespace DVDCD
{
    [Serializable]
   public class ItemClass
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string DVD_CDname { get; set; }


        public ItemClass ()
            {
                Name = "" ;
                FullPath = "" ;
            }

        ~ItemClass() { }
    }
}
