using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace PT_laby_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ProcessDirectory(ItemsControl root, String targetDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(targetDirectory);
            var item = new TreeViewItem();
            item.Tag = targetDirectory;
            item.Header = d.Name;
            item.ContextMenu = new ContextMenu();
            MenuItem create = new MenuItem() { Header = "Create" };
            create.Click += Create_func;
            MenuItem delete = new MenuItem() { Header = "Delete" };
            delete.Click += Delete_dir_func;
            item.ContextMenu.Items.Add(create);
            item.ContextMenu.Items.Add(delete);
            root.Items.Add(item);

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(item, fileName);


            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {                
                ProcessDirectory(item, subdirectory);
            }

        }

        void Create_func(object sender, RoutedEventArgs e) {
            TreeViewItem clickedDir = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            string dir = (string)clickedDir.Tag;
            var dlg = new Create_dialog(dir); 
            dlg.ShowDialog();
            if (!dlg.file_created)
            {
                return;
            }
            if (dlg.IsDir())
            {
                Add_dir(dlg.path(), dlg.name(), clickedDir);
            }
            else
            {
                Add_file(dlg.path(), dlg.name(), clickedDir);
            }
        }

        void Add_dir(string path, string name, ItemsControl root)
        {
            var item = new TreeViewItem();
            item.Tag = path;
            item.Header = name;
            item.ContextMenu = new ContextMenu();
            MenuItem create = new MenuItem() { Header = "Create" };
            create.Click += Create_func;
            MenuItem delete = new MenuItem() { Header = "Delete" };
            delete.Click += Delete_dir_func;
            item.ContextMenu.Items.Add(create);
            item.ContextMenu.Items.Add(delete);
            root.Items.Add(item);
        }

        void Add_file(string path, string name, ItemsControl root)
        {
            var item = new TreeViewItem();
            item.Tag = path;
            item.Header = name;
            item.ContextMenu = new ContextMenu();
            MenuItem open = new MenuItem() { Header = "open" };
            open.Click += Show_file_func;
            MenuItem delete = new MenuItem() { Header = "Delete" };
            delete.Click += Delete_file_func;
            item.ContextMenu.Items.Add(open);
            item.ContextMenu.Items.Add(delete);
            root.Items.Add(item);
        }

        void Show_file_func(object sender, RoutedEventArgs e)
        {
            TreeViewItem clickedFile = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            string text = File.ReadAllText(clickedFile.Tag as string);
            TextBlock1.Text = text;
        }

        void Delete_file_func(object sender, RoutedEventArgs e)
        {
            RemoveFromTree(sender, e);
            TreeViewItem chosen_object = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            FileInfo f = new FileInfo(chosen_object.Tag as string);
            f.Attributes = FileAttributes.Normal;
            f.Delete();
        }

        void RemoveFromTree(object sender, RoutedEventArgs e)
        {
            TreeViewItem chosen_object = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            TreeViewItem parent = (TreeViewItem)chosen_object.Parent;
            parent.Items.Remove(chosen_object);
        }
        void Delete_dir_func(object sender, RoutedEventArgs e)
        {
            RemoveFromTree(sender, e);
            TreeViewItem chosen_object = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            DirectoryInfo d = new DirectoryInfo(chosen_object.Tag as string);
            d.DeleteAll();
        }

        public void ProcessFile(ItemsControl root, String path)
        {
            FileInfo f = new FileInfo(path);
            var item = new TreeViewItem() { Tag = path };
            item.Header = f.Name;
            item.ContextMenu = new ContextMenu();
            MenuItem open = new MenuItem() { Header = "open" };
            open.Click += Show_file_func;
            MenuItem delete = new MenuItem() { Header = "delete" };
            delete.Click += Delete_file_func;
            item.ContextMenu.Items.Add(open);
            item.ContextMenu.Items.Add(delete);
            root.Items.Add(item);
            
        }

        private void Open_file(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            dlg.ShowDialog();
            ProcessDirectory(triview, dlg.SelectedPath);
        }

        private void Exit_program(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void About(object sender, RoutedEventArgs e)
        {
            var dlg = new About();
            dlg.ShowDialog();
        }

        private void RAHS_attr(object sender, RoutedEventArgs e)
        {
            FileInfo file = new FileInfo((string)((TreeViewItem)triview.SelectedItem).Tag);
            RAHS_info.Text = file.RAHS_atribiute();
        }

    }



    public static class MISC
    {
        public static void DeleteAll(this System.IO.DirectoryInfo d)
        {
            DirectoryInfo[] sub_d = d.GetDirectories();
            FileInfo[] sub_f = d.GetFiles();   
            foreach(FileInfo file in sub_f)
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }
            foreach(DirectoryInfo dir in sub_d)
            {
                dir.DeleteAll();
            }
            d.Attributes = FileAttributes.Normal;
            d.Delete();
        }




        public static string RAHS_atribiute(this FileSystemInfo info)
        {
            String rahs = "";
            if (info.Attributes.HasFlag(FileAttributes.ReadOnly))
                rahs += "r";
            else
                rahs += "-";
            if (info.Attributes.HasFlag(FileAttributes.Archive))
                rahs += "a";
            else
                rahs += "-";
            if (info.Attributes.HasFlag(FileAttributes.Hidden))
                rahs += "h";
            else
                rahs += "-";
            if (info.Attributes.HasFlag(FileAttributes.System))
                rahs += "s";
            else
                rahs += "-";

            return rahs;
        }
    }
}
