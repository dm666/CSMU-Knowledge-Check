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

namespace CSMU_Test_Editor
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

        private enum EditorMode
        {
            Create = 1,
            Edit = 2
        }

        private void CreateNewFile(object sender, RoutedEventArgs e)
        {
            // make UI for creating mode
            LoadingUI(EditorMode.Create);
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            // make UI for editing mode
            LoadingUI(EditorMode.Edit);
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Save!");
        }

        private void LoadingUI(EditorMode mode)
        {
            string userInterface = mode == EditorMode.Create ? "Development.xaml" : "Edit.xaml";

            if (!File.Exists(userInterface))
                throw new Exception("Can't load UI. Aborted.");

            var obj = mainGrid.FindName(mode == EditorMode.Create ? "editWork" : "developWork") as UIElement;

            if (obj != null)
                mainGrid.Children.Remove(obj);

            FileStream stream = new FileStream(userInterface, FileMode.Open);

            Grid workspace = new Grid();
            workspace.Name = mode == EditorMode.Create ? "developWork" : "editWork";
            workspace = System.Windows.Markup.XamlReader.Load(stream) as Grid;

            mainGrid.Children.Add(workspace);

            stream.Close();
        }

        private bool Save()
        {
            return true;
        }
    }
}
