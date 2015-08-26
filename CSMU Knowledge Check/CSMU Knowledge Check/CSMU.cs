using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        // var questType not define. So, values: 1 - single, 2 - multiple, 3 - image

        public class CFile
        {
            public CFile()
            {
            }

            public string Quest;
            public string QuestType;
            public int NumberOfCorrect;

            public List<string> Answers;
            public List<string> Correct;

            public int _time;
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
                    default:
                        break;
                }

                int lastIndex = rowData.Count - 1;
                int firstIndex = rowData.Count - CFileData.NumberOfCorrect;

                if (type == 1 || type == 2)
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
                else if (type > 3) // unknown, return false!
                {
                }

                CFileMgr.Add(rowIndex, CFileData);
            }

            CFileMgr = Randomization(CFileMgr);

            return true;
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

        private string Classification(byte[] array)
        {
            byte[] arrOfStr = new byte[array.Length];

            for (int i = 0; i < arrOfStr.Length; i++)
                arrOfStr[i] = (byte)(array[i] ^ 5);

            return Encoding.ASCII.GetString(arrOfStr);
        }

        private string Declassification(string encrypted)
        {
            return Classification((Encoding.ASCII.GetBytes(encrypted)));
        }


        public void Calculate()
        {
        }
    }
}
