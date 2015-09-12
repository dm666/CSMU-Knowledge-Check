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
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class User : Elysium.Controls.Window
    {
        public User()
        {
            InitializeComponent();
        }

        QuestList QuestLst;

        private void StartUp(object sender, RoutedEventArgs e)
        {
            QuestLst = this.Owner as QuestList;
        }

        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            if (QuestLst != null)
            {
                if (QuestLst.window != null)
                {
                    if (student.Text.Length < 1 || group.Text.Length < 1)
                    {
                        MessageBox.Show("Заполните все поля.");
                        return;
                    }

                    QuestLst.window.student = student.Text;
                    QuestLst.window.group = group.Text;

                    QuestLst.window.LoadingQuery(QuestLst.selectedFile);
                    QuestLst.window._NextQuest(QuestLst.window.rowId);
                    QuestLst.window.Status.Content = string.Format("1 вопрос из {0}", QuestLst.window.CSMUFileMgr.Count);
                    QuestLst.window.Visibility = Visibility.Visible;
                    QuestLst.window.timer.Start();

                    QuestLst.Close();
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
