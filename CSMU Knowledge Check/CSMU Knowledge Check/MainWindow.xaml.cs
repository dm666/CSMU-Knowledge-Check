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
using Elysium;
using Elysium.Parameters;

namespace CSMU_Knowledge_Check
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

        DispatcherTimer timer;
        public CSMU data = new CSMU();
        public int rowId, diff, SECOND = 1000;
        public string Student, Group;

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.Foreground = Elysium.AccentBrushes.Violet;
            timer = new DispatcherTimer();
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            data.CalculateAmount(rowId, box, diff);
            if (rowId < data.CFileMgr.Count)
            {
                resetTime();
                rowId++;
                data.ToNextQuest(rowId, box, field);
                counter.Content = string.Format("Вопрос {0} из {1}.", rowId, data.CFileMgr.Count);
            }
            else
                Table();
        }

        private void resetTime()
        {
            timer.Stop();
            bar.Value = 0;

            timer.Start();
        }

        private void Table() { }
    }
}
