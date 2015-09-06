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
            if (QuestLst.window != null)
            {
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
