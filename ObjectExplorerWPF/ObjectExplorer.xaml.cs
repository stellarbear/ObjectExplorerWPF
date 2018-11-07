using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ObjectExplorerWPF
{
    /// <summary>
    /// Логика взаимодействия для ObjectExplorer.xaml
    /// </summary>
    public partial class ObjectExplorer : Window
    {
        public string SelectedObject
        {
            get => _selectedObjects.Count() > 0 ?
                _selectedObjects.ElementAt(0) : default(string);
        }
        public IEnumerable<string> SelectedObjects
        {
            get
            {
                foreach (string selectedObject in _selectedObjects)
                    yield return selectedObject;
            }
        }

        private string filter;
        private ESelectionRestrictions type;
        private IEnumerable<string> _selectedObjects;


        public ObjectExplorer(IEnumerable<string> dirs, ESelectionRestrictions type, string filter = "*", string title = "Выбор объектов")
        {
            this.type = type;
            this.filter = filter;
            _selectedObjects = new List<string>();
            DataContext = new ViewModel(dirs, type, filter);

            InitializeComponent();
            LTitle.Content = title;
            this.MouseDown += MouseDown_Event;
        }


        public void MouseDown_Event(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BAccept_Click(object sender, RoutedEventArgs e)
        {
            _selectedObjects = (DataContext as ViewModel).GetCheckedNodes();
            this.Close();
        }

        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            string path = TBAdditionalPath.Text;

            try
            {
                path = Environment.ExpandEnvironmentVariables(path);

                if (Directory.Exists(path))
                {
                    (DataContext as ViewModel).AppendNode(path);
                }
            }
            catch (Exception ex)
            {
                LError.Content = ex.Message;
            }
        }

        private void GotToPath_Click(object sender, RoutedEventArgs e)
        {
            string path = TBAdditionalPath.Text;

            try
            {
                path = Environment.ExpandEnvironmentVariables(path);

                if (Directory.Exists(path))
                {
                    IEnumerable<string> dirs = Directory.GetDirectories(path);
                    IEnumerable<string> files = Directory.GetFiles(path);

                    DataContext = new ViewModel(dirs.Concat(files), type, filter);
                }
            }
            catch (Exception ex)
            {
                LError.Content = ex.Message;
            }
        }

        private async void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            await ((e.OriginalSource as TreeViewItem).DataContext as ExplorerNode).ExtendNodeAsync();
        }
    }
}
