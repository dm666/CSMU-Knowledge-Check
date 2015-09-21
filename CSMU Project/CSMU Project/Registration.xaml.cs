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

            _student.Foreground = Elysium.AccentBrushes.Blue;
            _group.Foreground = Elysium.AccentBrushes.Blue;
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
            selectedFile = files[box.SelectedIndex];

            ((TabItem)tbControl.Items[0]).Visibility = System.Windows.Visibility.Collapsed;
            ((TabItem)tbControl.Items[1]).Visibility = System.Windows.Visibility.Visible;

            tbControl.SelectedIndex = 1;
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            selectedFile = string.Empty;
            studentName.Text = "";
            studentGroup.Text = "";

            ((TabItem)tbControl.Items[0]).Visibility = System.Windows.Visibility.Visible;
            ((TabItem)tbControl.Items[1]).Visibility = System.Windows.Visibility.Collapsed;

            tbControl.SelectedIndex = 0;
        }

        private void Starting(object sender, RoutedEventArgs e)
        {
            if (studentName.Text.Length < 1 || string.IsNullOrWhiteSpace(studentName.Text)
            || studentGroup.Text.Length < 1 || string.IsNullOrWhiteSpace(studentGroup.Text))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            window = new MainWindow();
            window.Owner = this;

            window.student = studentName.Text;
            window.group = studentGroup.Text;
            window.currentTitle = System.IO.Path.GetFileNameWithoutExtension(selectedFile);

            window.rowId = 0;
            window.LoadingQuery(selectedFile);
            window._NextQuest(window.rowId);
            window.currentQuest.Content = string.Format("Вопрос 1 из {0}.", window.CSMUFileMgr.Count);
            window.Show();
            window.timer.Start();

            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void _Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
