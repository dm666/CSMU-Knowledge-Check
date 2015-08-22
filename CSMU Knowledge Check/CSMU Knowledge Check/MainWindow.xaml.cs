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

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            CSMU cs = new CSMU();
            List<CSMU> _un = new List<CSMU>();

            selector.Items.Add(new CSMU()
            {
                QuestName = "Header",
                Answer = "Blablabla",
                ImageSource = @"D:\wowprogramm\wowicons\INV_Thrown_1H_FirelandsRaid_D_01.png"
            }
            );
        }
    }

    public class CSMU
    {
        public CSMU() { }

        public string QuestName { get; set; }
        public string ImageSource { get; set; }
        public string Answer { get; set; }
    }
}
