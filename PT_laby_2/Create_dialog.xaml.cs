using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PT_laby_2
{
    /// <summary>
    /// Logika interakcji dla klasy Create_dialog.xaml
    /// </summary>
    public partial class Create_dialog : Window
    {
        public bool file_created = false;
        private string dir;
        private bool isDir = true;
        private string full_dir;


        public Create_dialog(string directory)
        {
            dir = directory;
            InitializeComponent();
        }

        private void create_butt(object sender, RoutedEventArgs e)
        {
            full_dir = dir + "\\" + Name_str.Text;
            if (isDir) 
            {
                Directory.CreateDirectory(full_dir);
                SetRAHSAtributes(full_dir);
                file_created=true;
            }
            else if (Regex.IsMatch(Name_str.Text, "^[A-Za-z0-9_~-]{1,8}\\.(txt|php|html)$")) 
            {
                File.Create(full_dir);
                SetRAHSAtributes(full_dir);                
                file_created = true;
            }
            else
            {
                MessageBox.Show("something is wrong with your file name");
            }

            if (file_created)
            {
                this.Close();
            }
        }

        private void SetRAHSAtributes(string dir)
        {
            FileAttributes atr = FileAttributes.Normal;
            if((bool)Readable_bool.IsChecked)
            {
                atr |= FileAttributes.ReadOnly;
            }
            if((bool)Archive_bool.IsChecked) 
            { 
                atr |= FileAttributes.Archive;
            }
            if((bool)System_bool.IsChecked)
            {
                atr |= FileAttributes.System;
            }
            if((bool)Hidden_bool.IsChecked) 
            {
                atr |= FileAttributes.Hidden;
            }

            File.SetAttributes(dir, atr);
        }

        private void exit_butt(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public bool IsDir() { return isDir; }
        private void SetDir(object sender, RoutedEventArgs e) { isDir = true; }
        private void SetFile(object sender, RoutedEventArgs e) { isDir = false; }
        public string path() { return full_dir; }    
        public string name() { return Name_str.Text; }
    }
}
