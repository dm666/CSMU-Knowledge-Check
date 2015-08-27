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
        // begin variables
        private CFile CFileData;

        public Dictionary<int, CFile> CFileMgr;
        public double UltimateResult;
        public Dictionary<int, double> ResultCollection = new Dictionary<int, double>();
        // var questType not define. So, values: 1 - single, 2 - multiple, 3 - image, 4 - from image

        public class CFile
        {
            public CFile()
            {
                Answers = new List<string>();
                Correct = new List<string>();
                ImageSource = new List<string>();
            }

            public string Quest { get; set; }
            public int QuestType { get; set; }
            public int NumberOfCorrect { get; set; }

            public List<string> Answers { get; set; }
            public List<string> Correct { get; set; }

            public int _time;

            public List<string> ImageSource { get; set; }
        }

        // end variables

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
                CFileData.NumberOfCorrect = int.Parse(rowData[2]);

                switch (type)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }

                int lastIndex = rowData.Count - 1;
                int firstIndex = rowData.Count - CFileData.NumberOfCorrect;

                if (type < 3 && type > 0)
                {
                    if (type == 1)
                        CFileData.Correct.Add(rowData[lastIndex]);
                    else if (type == 2) // multiple
                    {
                        for (int index = lastIndex; index >= firstIndex; index--)
                            CFileData.Correct.Add(rowData[index]);
                    }

                    rowData.RemoveRange(firstIndex, CFileData.NumberOfCorrect);
                    rowData.RemoveRange(0, 3);

                    Random randomSorting = new Random();

                    CFileData.Answers = rowData.OrderBy(sort => randomSorting.Next()).ToList();
                }
                else if (type == 3) // image
                {
                }
                else if (type > 4) // unknown, return false!
                {
                }

                CFileMgr.Add(rowIndex, CFileData);
            }

            CFileMgr = Randomization(CFileMgr);

            return true;
        }

        public void ToNextQuest(int rowId, ListBox selector, Label label)
        {
            if (!CFileMgr.ContainsKey(rowId))
                throw new Exception("Key not found.");

            int type = CFileMgr[rowId].QuestType;

            label.Content = labelContent(CFileMgr[rowId].Quest, "");

        }

        private object labelContent(string quest, string imgsource)
        {
            StackPanel p = new StackPanel() { Orientation = Orientation.Horizontal };

            TextBlock block = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                Text = quest
            };

            if (!string.IsNullOrEmpty(imgsource))
            {
                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(imgsource)),
                    Height = 120,
                    Width = 120
                };

                p.Children.Add(img);
            }

            p.Children.Add(block);

            return p;
        }

        private ListBoxItem addItem(int type, string ItemText, string imageSource)
        {
            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };

            if (type != 4)
            {
                TextBlock block = new TextBlock()
                {
                    Text = ItemText,
                    TextAlignment = TextAlignment.Center
                };


                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri(imageSource)),
                    Height = 48,
                    Width = 48,
                };

                panel.Children.Add(img);
                panel.Children.Add(block);
            }
            else if (type == 4) 
            {
                TextBox box = new TextBox()
                {
                    IsReadOnly = false,
                    Text = ItemText
                };

                panel.Children.Add(box);
            }

            return new ListBoxItem() { Content = panel };
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

        public void Calculate()
        {
        }

        private bool IsCorrect(string file)
        {
            // check file exist
            if (!File.Exists(file))
                return false;

            // file must contain minimum 2 rows
            if (File.ReadAllLines(file).Length < 2)
            {
                for (int i = 0; i < File.ReadAllLines(file).Length; i++)
                {
                    if (File.ReadAllLines(file)[i] == string.Empty)
                    {
                        // msgbox: quest in line [i] is empty, file contains max 1 row, return.
                        return false;
                    }
                }
                return false;
            }

            List<string> rows = File.ReadAllLines(file).ToList();

            foreach (var v in rows)
            {
                if (v.EndsWith(";"))
                    v.Remove(v.Length - 1, 1);
            }

            return true;
        }
    }
}
