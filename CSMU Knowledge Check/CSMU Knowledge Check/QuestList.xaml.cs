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

namespace CSMU_Knowledge_Check
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

        public MainWindow _MainWindow;
        private User _User;
        public string msg = "";
        string[] files;

        private void Loading(object sender, RoutedEventArgs e)
        {
            string path = @"Вопросы\";

            if (!Directory.Exists(path))
                throw new Exception("Директория " + path + " не найдена.");

            files = Directory.GetFileSystemEntries(path, "*.csmu");

            if (files.Length < 1)
                throw new Exception("Не найдено ни одного тестового задания.");

            for (int i = 0; i < files.Length; i++)
                questList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]));

            _MainWindow = this.Owner as MainWindow;

            _User = new User();
            _User.Owner = this;
        }

        private void Search(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < questList.Items.Count; i++)
            {
                if (questList.Items[i].ToString().ToLower() == e.Key.ToString().ToLower())
                {
                    questList.SelectedItem = i;
                    break;
                }
            }
        }

        private void Selected(object sender, MouseButtonEventArgs e)
        {
            if (files.Length < 1)
                throw new Exception();

            if (_MainWindow != null && _User != null)
            {
                msg = files[questList.SelectedIndex];
                _MainWindow.Title = ((ListBox)sender).SelectedItem.ToString();
                _User.ShowDialog();
            }
        }
    }
}
