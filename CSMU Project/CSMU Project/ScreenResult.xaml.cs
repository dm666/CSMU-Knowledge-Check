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

namespace CSMU_Project
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

        MainWindow window;

        private void Loaded(object sender, RoutedEventArgs e)
        {
            window = this.Owner as MainWindow;

            if (window != null)
            {
                ConvertToScreen(window);
                this.Title = string.Format("Результаты тестирования - [студент {0}]", window.student);
            }
        }

        private void ConvertToScreen(MainWindow Mgr)
        {
            List<Screen> source = new List<Screen>();

            for (int i = 0; i < Mgr.CSMUFileMgr.Count; i++)
            {
                if (!Mgr.CSMUFileMgr.ContainsKey(i))
                    continue;

                source.Add(new Screen(Mgr.CSMUFileMgr[i].Quest,
                    Mgr.GetPercentOfQuestByEntry(i),
                    Mgr.CSMUFileMgr[i].Time));
            }

            source.Add(new Screen("Итог теста", Mgr.Result()));

            table.ItemsSource = source;

            foreach (Screen s in table.ItemsSource)
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

        private class Screen
        {
            public Screen(string q, double p, int t = 0)
            {
                this.quest = q;
                this.percent = p;
                this.time = t;
            }

            public string quest { get; set; }
            public double percent { get; set; }
            public int time { get; set; }
        }
    }
}
