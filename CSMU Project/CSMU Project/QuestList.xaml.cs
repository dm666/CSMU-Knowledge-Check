using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace CSMU_Project
{
    /// <summary>
    /// Логика взаимодействия для QuestList.xaml
    /// </summary>
    public partial class QuestList : Elysium.Controls.Window
    {
        public QuestList()
        {
            InitializeComponent();
        }

        string[] files;

        private List<string> GetFilesList()
        {
            string path = @"Вопросы\";
            if (!Directory.Exists(path))
                throw new Exception(string.Format("Директория {0} не найдена", path));

            files = Directory.GetFileSystemEntries(path, "*.csmu");

            List<string> str = new List<string>();

            for (int i = 0; i < files.Length; i++)
                str.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]));

            return str;
        }
    }
}
