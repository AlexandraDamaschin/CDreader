using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;


namespace DVDCD
{
    public partial class FormDVDCD : Form
    {
        private Manager fileManager;

        public FormDVDCD()
        {
            InitializeComponent();
            comboBox.SelectedItem = 2;

            buttonSearch.Enabled = false;       
            comboBox.Enabled = false;
            buttonSearch.Enabled = false;


         
            var imageList = new ImageList();
            var largeImageList = new ImageList();
            imageList.Images.Add("file", new Icon(AppDomain.CurrentDomain.BaseDirectory + "\\file.ico"));
            largeImageList.Images.Add("png", Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\file.png"));

            listView.SmallImageList = imageList;
            listView.LargeImageList = largeImageList;
            listView.StateImageList = imageList;

            fileManager = new Manager();
        }

        private void checkCDDVDMenu_Click(object sender, EventArgs e)
        {
            treeView.Nodes.Clear();
            listView.Clear();

            buttonSearch.Enabled = true;      
            comboBox.Enabled = true;
            buttonSearch.Enabled = true;

            try
            {
                var result = fileManager.CheckDrives();
                foreach(var dvdcd in result)
                {
                    TreeNode parent = new TreeNode();

                    if (dvdcd.GetType()==typeof(DVDCD))
                    {
                        parent.Name = dvdcd.VolumeLabel;
                        parent.Text = dvdcd.VolumeLabel;
                    }
                    else
                    {
                        parent.Name = dvdcd.Name;
                        parent.Text = dvdcd.Name;

                    }
                    parent.Tag = dvdcd.FileList;
                    parent.ToolTipText = dvdcd.RootDirectory.FullName;
                    parent.Nodes.AddRange(GetNodes(dvdcd.FolderList));
                    treeView.Nodes.Add(parent);  

                 }
                if(CheckIfFileExistOrIsEmpty())
                {
                    LoadDVDCD(treeView);
                }
                SaveDVDCD(treeView);
            }
            catch(Exception)
            {
                //MessageBox.Show(ex.Message + ex.InnerException);
              MessageBox.Show(Helpers.NoDVDCD, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var fileList = e.Node.Tag;
            var resultString = new List<ListViewItem>();

            if(fileList.GetType()==typeof(List<FileClass>))
            {
                foreach(var file in (List<FileClass>)fileList)
                {
                    resultString.Add(new ListViewItem(file.Name,0) { Tag = file.FullPath });

                //    var bla = new  ListViewItem(file.Name, 0);
                //    bla.Tag = file.FullPath;
                }
            }
            if(resultString.Count==0)
            {
                listView.Clear();
                listView.Items.Add(Helpers.NoFilesInFolder);
            }
            else
            {
                listView.Items.Clear();
                listView.Items.AddRange(resultString.ToArray());

            }

        }

        //Aici incepe TreeNode

        public TreeNode [] GetNodes(List<FolderClass>folders)
        {
            List<TreeNode> child = new List<TreeNode>();
            foreach(var folder in folders)
            {
                TreeNode parentNode = new TreeNode();
                parentNode.Name = folder.Name;
                parentNode.Text = folder.Name;
                parentNode.Tag = folder.FileList;
                parentNode.ToolTipText = folder.DirectoryInfo.FullName;
                parentNode.Nodes.AddRange(GetNodes(folder.FolderList));
                child.Add(parentNode);
            }
            return child.ToArray();
        }
        
        public void FindFile(string searchTerm)
        {
            ChangeListView(2);

            var treeNodes = treeView.Nodes;
            var result = SearchTreeView(searchTerm, treeNodes);
            var resultString = new List<ListViewItem>();

            if(result.Count == 0)
            {
                 listView.Clear();
                MessageBox.Show(Helpers.NoFileFound, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
             else {
               
                 foreach(var file in result)
                {
                    resultString.Add(new ListViewItem(GetPath(file.FullPath, file.DVD_CDname), 0) { Tag = file.FullPath });
                }
                listView.Items.Clear();
                listView.Items.AddRange(resultString.ToArray());
                
            }
        }
        private string GetPath(string fullPath, string DVDCDname)
        {
            string result = "";
            var parts = fullPath.Split(':');
            result += DVDCDname + parts[1];
            return result;
               
        }
        public List <FileClass> SearchTreeView (string searchTerm, TreeNodeCollection nodes)
        {
            var result = new List<FileClass>();
            
            foreach(TreeNode node in nodes)
            {
                if (node.Tag.GetType()==typeof(List<FileClass>))
                {
                    foreach(var file in (List<FileClass>)node.Tag)
                    {
                        if (file.Name.ToLower().Contains(searchTerm.ToLower()))
                        {
                            result.Add(file);
                        }
                    }

                }
                result.AddRange(SearchTreeView(searchTerm, node.Nodes));
            }
            return result;

        }
 //Aici am incheiat cu TreeNode-ul

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            FindFile(searchTextBox.Text);
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar== Convert.ToChar(Keys.Enter))
            {
                FindFile(searchTextBox.Text);
            }

        }

        public void SaveDVDCD(TreeView tree)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\savedData.txt";

            using (Stream file = File.Open(fileName, FileMode.Create))
            {

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, tree.Nodes.Cast<TreeNode>().ToList());
            }
        }

        public bool CheckIfFileExistOrIsEmpty()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "\\savedData.txt";

            if (File.Exists(filename))
            {
                if (new FileInfo(filename).Length == 0)
                {
                    return false;
                }
                return true;

            }
            return false;
        } 


            public void LoadDVDCD(TreeView tree)
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "\\savedData.txt";
            if (!File.Exists(filename))
            {
                MessageBox.Show(Helpers.FileNoExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                using (Stream file = File.Open(filename, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object obj = bf.Deserialize(file);

                    TreeNode[] nodeList = (obj as IEnumerable<TreeNode>).ToArray();
                    bool addNode = true;
                    var listToAdd = new List<TreeNode>();

                    foreach(TreeNode nodeToAdd in nodeList)
                    {
                        addNode = true;
                        foreach(TreeNode nodeExisting in tree.Nodes)
                        {
                            if(nodeExisting.Name== nodeToAdd.Name)
                            {
                                addNode &= false;
                            }
                        }
                        if(addNode)
                        {
                            listToAdd.Add(nodeToAdd);
                        }
                    }
                    tree.Nodes.AddRange(listToAdd.ToArray());
                }

            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeListView(comboBox.SelectedIndex);
        }
        public void ChangeListView(int index)
        {
            switch(index)
            {
                case 0:
                    listView.View = View.LargeIcon;
                    break;
                case 1:
                    listView.View = View.SmallIcon;
                    break;
                case 2:
                    listView.View = View.List;
                    break;
                case 3:
                    listView.View = View.Tile;
                    break;

            }
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Helpers.About, "Information about DVDCD", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}


