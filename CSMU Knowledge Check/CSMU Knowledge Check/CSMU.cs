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

namespace CSMU_Knowledge_Check
{
    public class CSMU
    {
        // begin vars
        private CFile CFileData;

        public Dictionary<int, CFile> CFileMgr;
        public double UltimateResult;
        public Dictionary<int, double> ResultCollection = new Dictionary<int, double>();
        // var questType not define. So, values: 1 - single, 2 - multiple, 3 - image, 4 - from image

        // end vars

        public class CFile
        {
            public CFile()
            {
                Answers = new List<string>();
                Correct = new List<string>();
            }

            public string Quest { get; set; }
            public int QuestType { get; set; }
            public int NumberOfCorrect { get; set; }
            public string HeaderImage { get; set; }
            public string ImageAnswer { get; set; }

            public List<string> Answers { get; set; }
            public List<string> Correct { get; set; }

            public int _time;
        }

        public CSMU() { }

        public bool Loading(string _file)
        {
            if (!File.Exists(_file))
            {
                // msgbox.... blabla bla
                return false;
            }

            CFileMgr = new Dictionary<int, CFile>();

            string[] fileData = File.ReadAllLines(_file);

            if (fileData.Length < 2)
                return false;

            for (int rowIndex = 0; rowIndex < fileData.Length; rowIndex++)
            {
                if (fileData[rowIndex] == string.Empty)
                    continue;

                List<string> rowData = GetRowData(fileData[rowIndex]);

                CFileData = new CFile();

                CFileData.Quest = rowData[0];
                int type = int.Parse(rowData[1]);
                if (type == 4) // image 
                    CFileData.HeaderImage = rowData[2];
                else // or correct count
                    CFileData.NumberOfCorrect = int.Parse(rowData[2]);

                switch (type)
                {
                    case 1: CFileData.QuestType = 1; break;
                    case 2: CFileData.QuestType = 2; break;
                    case 3: CFileData.QuestType = 3; break;
                    case 4: CFileData.QuestType = 4; break;
                    default: break;
                }

                int lastIndex = rowData.Count - 1;
                int firstIndex = rowData.Count - CFileData.NumberOfCorrect;

                if (type < 4 && type > 0)
                {
                 //   if (type == 1)
                   //     CFileData.Correct.Add(rowData[lastIndex]);
                 //   else if (type == 2) // multiple
                 //   {
                        for (int index = lastIndex; index >= firstIndex; index--)
                            CFileData.Correct.Add(rowData[index]);
                //    }

                    rowData.RemoveRange(firstIndex, CFileData.NumberOfCorrect);
                    rowData.RemoveRange(0, 3);

                    Random randomSorting = new Random();

                    CFileData.Answers = rowData.OrderBy(sort => randomSorting.Next()).ToList();
                }
                else if (type == 4) // image
                    CFileData.ImageAnswer = rowData[3];
                else if (type > 5) // unknown, return false!
                {
                }

                CFileMgr.Add(rowIndex, CFileData);
            }

            CFileMgr = Randomization(CFileMgr);

            return true;
        }

        public void ToNextQuest(int rowId, ListBox lb, Label field)
        {
            if (!CFileMgr.ContainsKey(rowId))
                throw new Exception("Key not found.");

            int type = CFileMgr[rowId].QuestType;

            field.Content = GetContent(CFileMgr[rowId].Quest, CFileMgr[rowId].HeaderImage);

            if (type < 4)
            {
                for (int i = 0; i < CFileMgr[rowId].Answers.Count; i++)
                    lb.Items.Add(addItem(type, CFileMgr[rowId].Answers[i]));
            }
            else if (type == 4) // empty text, see function below
                lb.Items.Add(addItem(type));
        }

        private object GetContent(string quest, string headerImage)
        {
            StackPanel p = new StackPanel() { Orientation = Orientation.Horizontal };

            TextBlock block = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                Text = quest
            };

            if (!string.IsNullOrEmpty(headerImage))
            {
                if (File.Exists(headerImage))
                {
                    Image img = new Image()
                    {
                        Source = new BitmapImage(new Uri(headerImage)),
                        Height = 120,
                        Width = 120
                    };

                    p.Children.Add(img);
                }
                else
                    throw new Exception("Type 4? Image not found.");
            }

            p.Children.Add(block);

            return p;
        }

        private ListBoxItem addItem(int type, string ItemText = "")
        {
            ListBoxItem item = new ListBoxItem();

            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };

            if (type != 4)
            {
                if (type == 3)
                {
                    if (!string.IsNullOrEmpty(ItemText))
                    {
                        if (File.Exists(ItemText))
                        {
                            {
                                Image img = new Image()
                                {
                                    Source = new BitmapImage(new Uri(ItemText)),
                                    Height = 48,
                                    Width = 48,
                                };

                                panel.Children.Add(img);
                            }
                        }
                    }
                }
                else
                {
                    TextBlock block = new TextBlock()
                    {
                        Text = ItemText,
                        TextAlignment = TextAlignment.Left,
                        FontSize = 24
                    };

                    panel.Children.Add(block);
                }
            }
            else
            {
                Color c = Color.FromRgb(23, 23, 23);
                SolidColorBrush backgroundBrush = new SolidColorBrush(c);
                item.Background = backgroundBrush;
                panel.Children.Add(new TextBox());
            }

            item.Content = panel;

            return item;
        }

        private string GetItemText(ListBox box)
        {
                ListBoxItem l = box.SelectedItem as ListBoxItem;
                TextBlock b = FindFirstElementInVisualTree<TextBlock>(l);

                return b.Text;
        }

        private string GetItemsText(ListBox box, int index)
        {
            ListBoxItem l = box.SelectedItems[index] as ListBoxItem;

            TextBlock b = FindFirstElementInVisualTree<TextBlock>(l);

            return b.Text;
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

        private Dictionary<int, CFile> Randomization(Dictionary<int, CFile> mgr)
        {
            var random = new Random();

            for (int i = mgr.Count; i > 0; i--)
            {
                int j = random.Next();
                var t = mgr[i];
                mgr[i] = mgr[j];
                mgr[j] = t;
            }

            return mgr;
        }

        private List<string> GetRowData(string row)
        {
            List<string> rowData = new List<string>();

            foreach (string str in row.Split(';'))
                rowData.Add(str);

            return rowData;
        }

        private string Security(byte[] array)
        {
            byte[] arrOfStr = new byte[array.Length];

            for (int i = 0; i < arrOfStr.Length; i++)
                arrOfStr[i] = (byte)(array[i] ^ 5);

            return Encoding.ASCII.GetString(arrOfStr);
        }

        private string RemoveSecurity(string encrypted)
        {
            return Security((Encoding.ASCII.GetBytes(encrypted)));
        }

        public void CalculateAmount(int entry, ListBox box, int diff)
        {
            if (!CFileMgr.ContainsKey(entry))
                throw new Exception("Key not foud!");

            int wrong = 0;

            if (box.SelectedItems.Count < 1)
            {
                ResultCollection.Add(entry, 0);
                return;
            }

            if (CFileMgr[entry].QuestType == 1) // single
            {
                if (!CFileMgr[entry].Correct.Contains(GetItemText(box)))
                    wrong++;
            }
            else if (CFileMgr[entry].QuestType == 2) // multiple
            {
            }
        }
    }
}