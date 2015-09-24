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
using System.Windows.Threading;
using System.ComponentModel;
using System.IO;

namespace CSMU_Project
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

        public string student, group;
        public int rowId = 0, diff;
        public Dictionary<int, double> ResultCollection = new Dictionary<int,double>();
        public double UltimateResult;
        public DispatcherTimer timer = new DispatcherTimer();
        private int time = 60;
        public string currentTitle = "";
        private int endResponce = 1;

        private void StartUp(object sender, RoutedEventArgs e)
        {
            this.timer.Interval = new TimeSpan(0, 0, 1);
            this.timer.Tick += Tick;

            this.answerBox.BorderBrush = Elysium.AccentBrushes.Blue;
            this.answerBox.Foreground = Elysium.AccentBrushes.Blue;
            this.QuestField.Foreground = Elysium.AccentBrushes.Blue;
            this.QuestField.BorderBrush = Elysium.AccentBrushes.Blue;
            this.currentQuest.Foreground = Elysium.AccentBrushes.Blue;
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            CalculateAmount(rowId, diff);
            if (rowId < CSMUFileMgr.Count - 1)
            {
                resetTime();
                rowId++;
                _NextQuest(rowId);
                currentQuest.Content = string.Format("Вопрос {0} из {1}.", rowId + 1, CSMUFileMgr.Count);
            }
            else
                Screen();
        }

        private void Tick(object sender, EventArgs e)
        {
            diff = time / 1;

            if (time > 0)
            {
                time--;

                if (time < 10)
                {
                    block.Text = "";
                    block.Inlines.Add(new Run(timeLeft(time % 60)) { Foreground = Brushes.Red });
                }
                else
                {
                    block.Foreground = Elysium.AccentBrushes.Blue;
                    block.Text = timeLeft(time % 60);
                }
            }
            else
            {
                block.Text = "Время вышло.";
                CalculateAmount(rowId, diff);
                if (rowId < CSMUFileMgr.Count - 1)
                {
                    resetTime();
                    rowId++;
                    _NextQuest(rowId);
                }
                else
                    Screen();
            }
        }

        private void Screen()
        {
            timer.Stop();

            ((TabItem)mainTab.Items[0]).Visibility = System.Windows.Visibility.Collapsed;
            ((TabItem)mainTab.Items[1]).Visibility = System.Windows.Visibility.Visible;

            // fill results
            table.ItemsSource = GetScreenResult();

            // then select tab "Result"
            mainTab.SelectedIndex = 1;

            this.Width = table.Width;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private string shorted(string name, int chars)
        {
            return name.ToCharArray().Count() > chars ? name.Substring(0, chars) + "..." : name;
        }

        private List<ScreenResult> GetScreenResult()
        {
            List<ScreenResult> table = new List<ScreenResult>();

            for (int x = 0; x < CSMUFileMgr.Count; x++)
                table.Add(
                    new ScreenResult(shorted(CSMUFileMgr[x].Quest, 30),
                        GetPercentOfQuestByEntry(x) + "%",
                    CSMUFileMgr[x].Time)
                    );

            object time = GetTotalTime() + " сек.";

            table.Add(new ScreenResult(""));
            table.Add(new ScreenResult("Общий результат", TotalResult()));
            table.Add(new ScreenResult("Время тестирования", time));

            return table;
        }

        private string TotalResult()
        {
            return string.Format("{0:0.0%}", Result());
        }

        private class ScreenResult
        {
            public ScreenResult(string q, object p = null, object t = null)
            {
                this.SQuest = q;

                if (p != null)
                    this.SPercent = p;

                if (t != null)
                    this.STime = t;
            }

            public string SQuest { get; set; }
            public object STime { get; set; }
            public object SPercent { get; set; }
        }

        private string timeLeft(int second)
        {
            string value = "";

            switch (second % 10)
            {
                case 1:
                    value = "секунда";
                    break;
                case 2:
                case 3:
                case 4:
                    value = "секунды";
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 0:
                    value = "секунд";
                    break;
            }

            return string.Format("Осталось {0} {1}.", second, value);
        }

        private void resetTime()
        {
            time = 60;
            block.Foreground = Elysium.AccentBrushes.Blue;
            timer.Start();
        }

        public void _NextQuest(int rowId)
        {
            for (int i = 0; i < CSMUFileMgr.Count; i++)
            {
                if (!CSMUFileMgr.ContainsKey(rowId))
                    return;
            }

            // clear answers and quest
            QuestField.Content = string.Empty;
            box.Items.Clear();

            if (CSMUFileMgr[rowId].questType == QuestType.TextingImage)
            {
                answerBox.Visibility = System.Windows.Visibility.Visible;

                if (box.Visibility == System.Windows.Visibility.Visible)
                    box.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (CSMUFileMgr[rowId].questType == QuestType.Single)
            {
                answerBox.Visibility = System.Windows.Visibility.Hidden;
                box.Visibility = System.Windows.Visibility.Visible;
                box.SelectionMode = SelectionMode.Single;
            }
            else if (CSMUFileMgr[rowId].questType == QuestType.Multiple ||
                CSMUFileMgr[rowId].questType == QuestType.ChoiceImage)
            {
                answerBox.Visibility = System.Windows.Visibility.Hidden;
                box.Visibility = System.Windows.Visibility.Visible;
                box.SelectionMode = SelectionMode.Multiple;
            }

            if (CSMUFileMgr[rowId].questType == QuestType.TextingImage)
                AddItemWithTextImage(CSMUFileMgr[rowId].Quest, CSMUFileMgr[rowId].HeaderImage);
            else
            {
                SetQuestField(QuestField, CSMUFileMgr[rowId].Quest);

                for (int i = 0; i < CSMUFileMgr[rowId].Answers.Count; i++)
                    box.Items.Add(AddItemWithoutImage(CSMUFileMgr[rowId].questType, CSMUFileMgr[rowId].Answers[i]));
            }
        }

        private void CalculateAmount(int id, int diff)
        {
            if (!CSMUFileMgr.ContainsKey(id))
                throw new Exception("Key " + id + " not found.");

            if ((endResponce + 1) == CSMUFileMgr.Count)
                NextQst.Content = "Завершить.";

            endResponce++;

            int Wrong = 0;

            if (CSMUFileMgr[id].questType != QuestType.TextingImage)
                if (box.SelectedItems.Count < 1)
                {
                    ResultCollection.Add(id, 0.0);
                    return;
                }

            if (CSMUFileMgr[id].questType == QuestType.Single)
            {
                if (!CSMUFileMgr[id].corrected.Contains(GetSingleItemText()))
                    Wrong++;
            }
            else if (CSMUFileMgr[id].questType == QuestType.Multiple || CSMUFileMgr[id].questType == QuestType.ChoiceImage)
            {
                if (box.SelectedItems.Count > 1)
                {
                    for (int i = 0; i < box.SelectedItems.Count; i++)
                    {
                        if (!CSMUFileMgr[id].corrected.Contains(GetMultipleItemText(i, CSMUFileMgr[id].questType)))
                            Wrong++;
                    }
                }

                if (box.SelectedItems.Count == 1)
                {
                    for (int i = 0; i < box.SelectedItems.Count; i++)
                    {
                        if (CSMUFileMgr[id].corrected.Contains(GetMultipleItemText(i, CSMUFileMgr[id].questType)))
                            Wrong = CSMUFileMgr[id].corrected.Count - 1;
                        else
                            Wrong = CSMUFileMgr[id].corrected.Count;
                    }
                }

                if (box.SelectedItems.Count == CSMUFileMgr[id].Answers.Count)
                {
                    if (CSMUFileMgr[id].corrected.Count == CSMUFileMgr[id].Answers.Count)
                    {
                        ResultCollection.Add(id, 1.0);
                        return;
                    }
                    else
                    {
                        ResultCollection.Add(id, 0);
                        return;
                    }
                }

                if (box.SelectedItems.Count == 0)
                    Wrong = CSMUFileMgr[id].corrected.Count;
            }
            else if (CSMUFileMgr[id].questType == QuestType.TextingImage)
            {
                ResultCollection.Add(id, answerBox.Text.ToLower() == CSMUFileMgr[id].ImageAnswer.ToLower() ? 1.0 : 0.0);
                CSMUFileMgr[id].Time = ((60 - diff) == 0 ? 1 : (60 - diff));
                return;
            }

            UltimateResult = (CSMUFileMgr[id].corrected.Count - Wrong);
            UltimateResult /= CSMUFileMgr[id].corrected.Count;

            ResultCollection.Add(id, UltimateResult);
            CSMUFileMgr[id].Time = ((60 - diff) == 0 ? 1 : (60 - diff));
        }

        public double GetPercentOfQuestByEntry(int questId)
        {
            if (!CSMUFileMgr.ContainsKey(questId))
                throw new Exception("Key not found.");

            return ResultCollection[questId] * 100;
        }

        public double Result()
        {
            double length = 0;

            for (int i = 0; i < ResultCollection.Count; i++)
            {
                if (!ResultCollection.ContainsKey(i))
                    continue;

                length += ResultCollection[i];
            }

            double res = length / CSMUFileMgr.Count;
            return res;
        }

        private int GetTotalTime()
        {
            int obj = 0;

            for (int i = 0; i < CSMUFileMgr.Count; i++)
                obj += CSMUFileMgr[i].Time;

            return obj;
        }

        private void _Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddItemWithTextImage(string quest, string header)
        {
            if (!File.Exists(header))
                throw new Exception("File not found.");

            StackPanel p2 = new StackPanel() { Orientation = Orientation.Horizontal };

            TextBlock field = new TextBlock()
            {
                Text = quest,
                TextAlignment = TextAlignment.Left,
                Height = 45,
            };

            Image img = new Image()
            {
                Source = new BitmapImage(new Uri(header)),
                Height = 100,
                Width = 80
            };

            p2.Children.Add(field);
            p2.Children.Add(img);

            QuestField.Content = p2;
        }

        private ListBoxItem AddItemWithoutImage(QuestType iType, string ItemText)
        {
            ListBoxItem Item = new ListBoxItem();

            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };

            if (iType == QuestType.Multiple || iType == QuestType.Single)
                Item.Content = ItemText;
            else if (iType == QuestType.ChoiceImage)
            {
                if (!File.Exists(ItemText))
                    throw new Exception("Файл не найден.");

                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(ItemText)),
                    Height = 48,
                    Width = 48,
                };

                TextBlock block = new TextBlock()
                {
                    Text = ItemText,
                    TextAlignment = TextAlignment.Left,
                    Height = 32,
                    Width = box.Width,
                    Visibility = System.Windows.Visibility.Hidden
                };

                panel.Children.Add(img);
                panel.Children.Add(block);

                Item.Content = panel;
            }

            return Item;
        }

        private void SetQuestField(Label questField, string quest)
        {
            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };

            TextBlock main = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                Text = quest,
                Height = questField.Height,
                Width = questField.Width - 100
            };

            panel.Children.Add(main);

            questField.Content = panel;
        }

        private string GetSingleItemText()
        {
            ListBoxItem item = (ListBoxItem)box.ItemContainerGenerator.ContainerFromIndex(box.SelectedIndex);

            return item.Content.ToString();
        }

        private string GetMultipleItemText(int i, QuestType iType)
        {
            ListBoxItem item = (ListBoxItem)box.ItemContainerGenerator.ContainerFromItem(box.SelectedItems[i]);

            if (iType == QuestType.Multiple)
                return item.Content.ToString();
            else
                return FindFirstElementInVisualTree<TextBlock>(item).Text;
        }

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }

        public enum QuestType
        {
            Single = 1,
            Multiple,
            ChoiceImage,
            TextingImage
        }

        public Dictionary<int, CSMUFile> CSMUFileMgr;
        private CSMUFile CFile;

        public class CSMUFile
        {
            public CSMUFile()
            {
                Answers = new List<string>();
                corrected = new List<string>();
            }

            public QuestType questType { get; set; }
            public string Quest { get; set; }
            public int NumberOfCorrect { get; set; }
            public string HeaderImage { get; set; }
            public string ImageAnswer { get; set; }

            public List<string> Answers { get; set; }
            public List<string> corrected { get; set; }

            public int Time { get; set; }
        }

        private string Security(byte[] array)
        {
            byte[] arrOfStr = new byte[array.Length];

            for (int i = 0; i < arrOfStr.Length; i++)
                arrOfStr[i] = (byte)(array[i] ^ 5);

            return Encoding.ASCII.GetString(arrOfStr);
        }

        private string Desecurity(string encrypted)
        {
            return Security((Encoding.ASCII.GetBytes(encrypted)));
        }

        public void LoadingQuery(string file)
        {
            CSMUFileMgr = new Dictionary<int, CSMUFile>();

            string[] data = File.ReadAllLines(file)
                .Where(empty => !string.IsNullOrEmpty(empty)).ToArray();

            if (data.Length < 1)
                throw new Exception("File is empty.");

            for (int rowId = 0; rowId < data.Length; rowId++)
            {
                List<string> rowData = GetRowData(data[rowId]);

                CFile = new CSMUFile();

                int firstIndex = 0;

                // get quest type
                int iType = int.Parse(rowData[0]);

                // set type
                switch (iType)
                {
                    case 1: CFile.questType = QuestType.Single; break;
                    case 2: CFile.questType = QuestType.Multiple; break;
                    case 3: CFile.questType = QuestType.ChoiceImage; break;
                    case 4: CFile.questType = QuestType.TextingImage; break;
                    default:
                        throw new Exception("Unknown type.");
                }

                // get quest
                CFile.Quest = rowData[1];

                if (iType == 4) // set image..
                    CFile.HeaderImage = rowData[2];
                else          // or number of correct
                {
                    CFile.NumberOfCorrect = int.Parse(rowData[2]);

                    // get first index 
                    firstIndex = rowData.Count - CFile.NumberOfCorrect;
                }

                // get last index
                int lastIndex = rowData.Count - 1;

                if (iType == 1)
                {
                    // add corrected to list
                    CFile.corrected.Add(rowData[lastIndex]);

                    // prepare answer's list
                    // remove correct
                    rowData.RemoveRange(lastIndex, 1);

                    // remove first data such as type, quest, headerimage/numberofcorrect
                    rowData.RemoveRange(0, 3);
                }
                else if (iType == 2 || iType == 3)
                {
                    // fill 
                    for (int index = lastIndex; index >= firstIndex; index--)
                        CFile.corrected.Add(rowData[index]);

                    // prepare answer's list
                    // remove correct
                    rowData.RemoveRange(firstIndex, CFile.NumberOfCorrect);

                    // remove first data such as type, quest, headerimage/numberofcorrect
                    rowData.RemoveRange(0, 3);
                }
                else if (iType == 4) // set answer
                    CFile.ImageAnswer = rowData[3];

                Random rnd = new Random();

                // random Answers
                CFile.Answers = rowData.OrderBy(s => rnd.Next()).ToList();

                // add to dictionary
                CSMUFileMgr.Add(rowId, CFile);
            }

            // mix quest's
            CSMUFileMgr = RandomDictionary(CSMUFileMgr);
        }

        private Dictionary<int, CSMUFile> RandomDictionary(Dictionary<int, CSMUFile> mgr)
        {
            var rnd = new Random();

            for (int i = mgr.Count - 1; i >= 0; i--)
            {
                int j = rnd.Next(0, i);
                var t = mgr[i];
                mgr[i] = mgr[j];
                mgr[j] = t;
            }

            return mgr;
        }

        private List<string> GetRowData(string str)
        {
            List<string> row = new List<string>();

            foreach (string r in str.Split(';'))
                row.Add(r);//Desecurity(r));

            return row;
        }

        private string Parsing(string[] data, string attr)
        {
            string AttrValue = "Not found.";

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].ToLower().Contains(attr.ToLower() + "\""))
                {
                    AttrValue = data[i].Replace("\"", "").Replace(attr, "").TrimStart();
                    break;
                }
            }

            return AttrValue;
        }
    }
}
