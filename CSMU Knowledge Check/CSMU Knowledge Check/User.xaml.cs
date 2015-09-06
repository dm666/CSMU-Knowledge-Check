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
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class User : Elysium.Controls.Window
    {
        public User()
        {
            InitializeComponent();

            //QList = this.Owner as QuestList;
        }

        private void Loading(object sender, RoutedEventArgs e)
        {
            QList = this.Owner as QuestList;
        }

        QuestList QList;

        private void Start(object sender, RoutedEventArgs e)
        {
            if (QList != null)
            {
                if (QList._MainWindow != null)
                {
                    if (Student.Text.Length < 1 || Group.Text.Length < 1)
                    {
                        MessageBox.Show("Заполните все поля.");
                        return;
                    }

                    QList._MainWindow.Student = Student.Text;
                    QList._MainWindow.Group = Group.Text;

                    if (QList._MainWindow.data != null)
                    {
                        QList._MainWindow.rowId = 0;
                        QList._MainWindow.data.Loading(QList.msg);
                        QList._MainWindow.data.ToNextQuest(QList._MainWindow.rowId,
                            QList._MainWindow.box,
                            QList._MainWindow.field);
                        QList._MainWindow.counter.Content = string.Format("Вопрос 1 из {0}.",
                            QList._MainWindow.data.CFileMgr.Count);
                    //    QList._MainWindow.timer.IsEnabled = true;
                    }

                    this.Close();
                    QList.Close();
                }
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
