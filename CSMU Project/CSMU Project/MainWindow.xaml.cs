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

        public void NextQuest(int rowId)
        {
            for (int i = 0; i < CSMUFileMgr.Count; i++)
            {
                if (!CSMUFileMgr.ContainsKey(rowId))
                    continue;
            }

            // clear answers and quest
            QuestField.Content = string.Empty;
            box.Items.Clear();

            if (CSMUFileMgr[rowId].questType == QuestType.TextingImage)
                box.Items.Add(AddItemWithTextImage(CSMUFileMgr[rowId].Quest, CSMUFileMgr[rowId].HeaderImage));
            else
            {
                SetQuestField(QuestField, CSMUFileMgr[rowId].Quest);

                for (int i = 0; i < CSMUFileMgr[rowId].Answers.Count; i++)
                    box.Items.Add(AddItemWithoutImage(CSMUFileMgr[rowId].questType, CSMUFileMgr[rowId].Answers[i]));
            }
        }

        private ListBoxItem AddItemWithTextImage(string quest, string header)
        {
            if (File.Exists(header))
                throw new Exception("File not found.");

            ListBoxItem item = new ListBoxItem();

            StackPanel p = new StackPanel() { Orientation = Orientation.Horizontal };
            StackPanel p2 = new StackPanel() { Orientation = Orientation.Horizontal };

            TextBlock field = new TextBlock()
            {
                Text = quest,
                TextAlignment = TextAlignment.Left,
                Height = 45,
                Width = box.Width - 100
            };

            Image img = new Image()
            {
                Source = new BitmapImage(new Uri(header)),
                Height = 100,
                Width = 100
            };

            p2.Children.Add(field);
            p2.Children.Add(img);

            QuestField.Content = p2;

            TextBox iBox = new TextBox()
            {
                TextAlignment = TextAlignment.Left,
                Height = 45,
                Width = box.Width
            };

            p.Children.Add(iBox);

            item.Content = p;

            return item;
        }

        private ListBoxItem AddItemWithoutImage(QuestType iType, string ItemText)
        {
            ListBoxItem Item = new ListBoxItem();

            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };

            if (iType == QuestType.Multiple || iType == QuestType.Single)
            {
                TextBlock block = new TextBlock()
                {
                    Text = ItemText,
                    TextAlignment = TextAlignment.Left,
                    Height = 45,
                    Width = box.Width
                };

                panel.Children.Add(block);
            }
            else if (iType == QuestType.ChoiceImage)
            {
                if (!File.Exists(ItemText))
                    throw new Exception("Файл не найден.");

                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(ItemText)),
                    Height = 72,
                    Width = 72,
                };

                TextBlock block = new TextBlock()
                {
                    Text = ItemText,
                    TextAlignment = TextAlignment.Left,
                    Height = 45,
                    Width = box.Width
                };

                panel.Children.Add(img);
                panel.Children.Add(block);
            }

            Item.Content = panel;

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

        private enum QuestType
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
        }

        public void LoadingQuery(string file)
        {
            CSMUFileMgr = new Dictionary<int, CSMUFile>();

            string[] data = File.ReadAllLines(file);

            if (data.Length < 2)
                throw new Exception("File is empty.");

            for (int rowId = 0; rowId < data.Length; rowId++)
            {
                if (data[rowId] == string.Empty)
                    continue;

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
                row.Add(r);

            return row;
        }
    }
}
