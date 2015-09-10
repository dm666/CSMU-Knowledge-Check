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
using System.ComponentModel;
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

        private string[] files;
        private User user;
        public MainWindow window;
        public string selectedFile;

        private List<string> GetFilesList()
        {
            string path = @"Вопросы\";
            if (!Directory.Exists(path))
                throw new Exception(string.Format("Директория {0} не найдена.", path));

            files = Directory.GetFileSystemEntries(path, "*.csmu");

            if (files.Length < 1)
                throw new Exception("Вопросов не найдено.");

            List<string> str = new List<string>();

            for (int i = 0; i < files.Length; i++)
                str.Add(System.IO.Path.GetFileNameWithoutExtension(files[i]));

            return str;
        }

        private void StartUp(object sender, RoutedEventArgs e)
        {
            box.ItemsSource = GetFilesList();

            border.BorderBrush = Elysium.AccentBrushes.Blue;
            border.BorderThickness = new Thickness(1.0);

            window = this.Owner as MainWindow;

            user = new User();
            user.Owner = this;
        }

        private void Searching(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                if (box.Items[i].ToString().ToLower() == e.Key.ToString().ToLower())
                {
                    box.SelectedItem = i;
                    break;
                }
            }
        }

        private void SelectedByDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (window != null)
            {
                selectedFile = files[box.SelectedIndex];
                window.Title = box.SelectedItem.ToString();
                user.ShowDialog();
            }
        }

        private void _Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
