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

namespace CSMU_Knowledge_Check
{
    /// <summary>
    /// Логика взаимодействия для ScreenResult.xaml
    /// </summary>
    public partial class ScreenResult : Elysium.Controls.Window
    {
        public ScreenResult()
        {
            InitializeComponent();
        }

        private void LoadingData(CSMU Mgr, int index)
        {
            List<ResultTable> source = new List<ResultTable>();

            for (int i = 0; i < Mgr.CFileMgr.Count; i++)
            {
                if (!Mgr.CFileMgr.ContainsKey(i))
                    continue;

                source.Add(new ResultTable(Mgr.CFileMgr[i].Quest,
                    Mgr.GetPercentOfQuestByEntry(i),
                    Mgr.CFileMgr[i]._time));
            }

            source.Add(new ResultTable("Итог теста", Mgr.Result()));

            table.ItemsSource = source;

            foreach (ResultTable s in table.ItemsSource)
            {
                var row = table.ItemContainerGenerator.ContainerFromItem(s) as DataGridRow;

                if (s.percent >= 0.7)
                    row.Background = Brushes.Green;
                else if (s.quest == "Итог теста")
                {
                }
                else
                    row.Background = Brushes.Red;
            }
        }
    }

    public class ResultTable
    {
        public ResultTable(string _quest, double _percent, int _time = 0)
        {
            this.quest = _quest;
            this.percent = _percent;
            this.time = _time;
        }

        public string quest { get; set; }
        public double percent { get; set; }
        public int time { get; set; }
    }
}
