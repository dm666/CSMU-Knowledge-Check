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

        public DispatcherTimer timer = new DispatcherTimer();
        public CSMU data = new CSMU();
        public int rowId, diff, SECOND = 1000;
        public string Student, Group;

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.Foreground = Elysium.AccentBrushes.Violet;
            this.box.SelectionMode = SelectionMode.Multiple;

            QuestList l = new QuestList();
            l.Owner = this;
            l.Show();

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.IsEnabled = false;
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

            return string.Format("Осталось {0} {1}", second, value);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (bar.Value == bar.Maximum)
                timer.IsEnabled = false;

            diff = (int)(bar.Maximum - bar.Value) / SECOND;

            if (diff > 0)
            {
                percent.Text = timeLeft(diff);
              //  bar.Value += Convert.ToDouble(timer.Interval);
            }
            else
            {
                percent.Text = "Time is gone!";
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
        }

        private void resetTime()
        {
            timer.Stop();
            bar.Value = 0;

            timer.Start();
        }

        private void Table() 
        {
            timer.Stop();

            ScreenResult result = new ScreenResult();
            result.Owner = this;
            result.ShowDialog();

            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
