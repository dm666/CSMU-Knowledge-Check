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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace CSMU_Test_Editor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<CAnswer> gridFields;
        List<string> quests, currentQuest;

        private void WndLoaded(object sender, RoutedEventArgs e)
        {
            Label[] fields = new Label[] 
            {
                QType,
                QLabel,
                HeadNOC,
                AcI,
                FName,
                NQList,
                ELAcI,
                ELHeadNoc,
                ELQType,
                ELQuest
            };

            for (int i = 0; i < fields.Length; i++)
                fields[i].Foreground = Elysium.AccentBrushes.Blue;

            // only quest
            quests = new List<string>();

            // all qst data: type, quest, answers etc..
            currentQuest = new List<string>();
        }

        private void SelectQuestType(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            switch (comboBox.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                    HeadNOC.Content = "Кол-во правильных ответов";
                    AcI.Content = "Количество ответов";
                    if (grid.Visibility != System.Windows.Visibility.Visible)
                        grid.Visibility = System.Windows.Visibility.Visible;
                    if (img.Visibility == System.Windows.Visibility.Visible)
                        img.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 3:
                    HeadNOC.Content = "Путь к изображению";
                    AcI.Content = "Ответ";
                    if (grid.Visibility != System.Windows.Visibility.Collapsed)
                        grid.Visibility = System.Windows.Visibility.Collapsed;
                    if (img.Visibility != System.Windows.Visibility.Visible)
                        img.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void MakeAnswersFields(int count)
        {
            if (count <= 1)
            {
                MessageBox.Show("Нужно добавить минимум 2 ответа.");
                return;
            }

            gridFields = new List<CAnswer>();

            for (int i = 1; i <= count; i++)
            {
                CAnswer _t = new CAnswer();
                _t.emptyField = i.ToString();
                gridFields.Add(_t);
            }

            grid.ItemsSource = gridFields;
        }

        private void AddFields(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int reserved;

                if (int.TryParse(TAcImg.Text, out reserved))
                {
                    if (gridFields != null)
                        gridFields.Clear();

                    MakeAnswersFields(reserved);
                }
            }
        }

        private void RemoveLast(object sender, RoutedEventArgs e)
        {

        }

        private void ApplyCurrentQuest(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GetAnswers());
            MessageBox.Show(GetCorrectAnswers());
           // string currentQuest = string.Format("");
        }

        private void SaveCurrentTest(object sender, RoutedEventArgs e)
        {

        }

        // functions

        // Security test from students :D
        private string Security(byte[] array)
        {
            byte[] arrOfStr = new byte[array.Length];

            for (int i = 0; i < arrOfStr.Length; i++)
                arrOfStr[i] = (byte)(array[i] ^ 5);

            return Encoding.ASCII.GetString(arrOfStr);
        }

        private class CAnswer
        {
            public string emptyField { get; set; }
        }

        private string GetAnswers()
        {
            string answers = "";


            for (int i = 0; i < grid.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(i);

                for (int z = 0; z < 1; z++)
                {
                    TextBlock bl = grid.Columns[z].GetCellContent(row) as TextBlock;
                    answers += bl.Text + ";";
                }
            }

            return answers;
        }

        private string GetCorrectAnswers()
        {
            string corrects = "";

            for (int i = 0; i < grid.SelectedItems.Count; i++)
            {
                CAnswer a = (CAnswer)grid.SelectedItems[i];

                corrects += a.emptyField + ";";
            }

            return corrects.TrimEnd(';');
        }
    }
}
