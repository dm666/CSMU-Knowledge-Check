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
        List<string> questsHead, questList;

        private int currentIndex = -1;

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
                ELFindQuest,
                ELQuestList
            };

            for (int i = 0; i < fields.Length; i++)
                fields[i].Foreground = Elysium.AccentBrushes.Blue;

            // only quest
            questsHead = new List<string>();

            // all qst data: type, quest, answers etc..
            questList = new List<string>();
        }

        private void SelectQuestType(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            currentIndex = comboBox.SelectedIndex;

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

            if (!TQuest.IsEnabled)
                TQuest.IsEnabled = true;

            if (!THead.IsEnabled)
                THead.IsEnabled = true;

            if (!TAcImg.IsEnabled)
                TAcImg.IsEnabled = true;
        }

        private void MakeAnswersFields(int count)
        {
            gridFields = new List<CAnswer>();

            for (int i = 1; i <= count; i++)
            {
                CAnswer _t = new CAnswer();
                _t.emptyField = i.ToString();
                gridFields.Add(_t);
            }

            grid.ItemsSource = gridFields;
        }

        private void RemoveLast(object sender, RoutedEventArgs e)
        {
            int deleted = previewBox.SelectedIndex;

            if (deleted != -1)
            {
                questList.RemoveAt(deleted);
                previewBox.Items.RemoveAt(deleted);
            }
        }

        private void ApplyCurrentQuest(object sender, RoutedEventArgs e)
        {
            string quest = TQuest.Text;
            string header = THead.Text;
            int outdate;

            string questToApply = ApplyCurrentTest(quest, header, 
                int.TryParse(TAcImg.Text, out outdate) == true ? null : TAcImg.Text);

            previewBox.Items.Add(questToApply.Split(';')[1]);

            questList.Add(Security(Encoding.ASCII.GetBytes(questToApply)));
        }

        private void SaveCurrentTest(object sender, RoutedEventArgs e)
        {
            TextBox[] boxes = new TextBox[]
            {
                TQuest,
                THead,
                TAcImg,
                TFileName
            };

            for (int x = 0; x < boxes.Length; x++)
            {
                if (boxes[x].Text.Length < 1)
                {
                    MessageBox.Show("Заполните все поля.");
                    return;
                }
            }

            if (questList.Count < 1)
            {
                MessageBox.Show("Невозможно создать пустое тестирование.");
                return;
            }

            using (StreamWriter wr = File.AppendText(TFileName.Text + ".csmu"))
            {
                for (int i = 0; i < questList.Count; i++)
                    wr.WriteLine(questList[i]);
            }
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

        private string ApplyCurrentTest(string quest, string headerOrCorrected, object answer = null)
        {
            string complete = "";

            if (currentIndex != 3)
                complete = string.Format("{0};{1};{2};{3};{4}", (currentIndex + 1), quest,
                    headerOrCorrected, GetAnswers(), GetCorrectAnswers());
            else
                complete = string.Format("{0};{1};{2};{3}", 4, quest, headerOrCorrected, answer);

            return complete.Remove(complete.Length - 2, 1);
        }

        private void PreviewImage(object sender, TextChangedEventArgs e)
        {
            if (currentIndex == 3)
            {
                if (string.IsNullOrEmpty(THead.Text))
                    img.Source = null;
                else if (!File.Exists(THead.Text))
                    img.Source = new BitmapImage(
                        new Uri(@"C:\users\dm666\desktop\nf.png", UriKind.RelativeOrAbsolute));
                else
                    img.Source = new BitmapImage(new Uri(THead.Text));
            }
            else
            {
                if (THead.Text == "0")
                    THead.Text = currentIndex == 0 ? "1" : "2";

                int check;

                if (!int.TryParse(THead.Text, out check))
                    THead.Text = string.Empty;
                else
                {
                    if (check > int.Parse(TAcImg.Text))
                        THead.Text = "1";
                }
            }
        }

        private void AddFields(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TAcImg.Text))
                if (gridFields != null)
                {
                    gridFields.Clear();
                    // update ItemsSource
                    CollectionViewSource.GetDefaultView(gridFields).Refresh();
                    return;
                }

            if (currentIndex != 3)
            {
                int reserved;

                if (int.TryParse(TAcImg.Text, out reserved))
                {
                    if (gridFields != null)
                        gridFields.Clear();

                    if (reserved > 10)
                        reserved = 10;

                    MakeAnswersFields(reserved);
                }
                else
                    TAcImg.Text = string.Empty;
            }
        }

        private void EditorSelection(object sender, SelectionChangedEventArgs e)
        {
            this.Width = SelectLoadedQuest.IsSelected ? 400 : 725;
        }

        private void CreaterSelection(object sender, SelectionChangedEventArgs e)
        {
            if (tabItemCreate.IsSelected)
                this.Width = 725;
        }
    }
}
