using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Windows;

namespace ObjectExplorerWPF
{
    /// <summary>
    /// Логика взаимодействия для WPFObjectExplorerWPF.xaml
    /// </summary>
    public partial class WPFObjectExplorerWPF : Window
    {
        public string SelectedObject
        {
            get => _selectedObjects.Count > 0 ?
                _selectedObjects[0] : default(string);
        }
        public IEnumerable<string> SelectedObjects
        {
            get
            {
                foreach (string selectedObject in _selectedObjects)
                    yield return selectedObject;
            }
        }

        private List<string> _selectedObjects;
        private ESelectionRestrictions _type;
        public WPFObjectExplorerWPF(IEnumerable<string> dirs, ESelectionRestrictions type, string title = "Выбор объектов")
        {
            _selectedObjects = new List<string>();
            this.MouseDown += MouseDown_Event;
            InitializeComponent();
            _type = type;

            TitleLabel.Content = title;
            foreach (string dir in dirs)
                TVExplorer.AddCustomSelectableNodeExt(dir, type);
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
            _selectedObjects = TVExplorer.GetSelectedNodesExt(ESelectType.Checked);
            TVExplorer.Items.Clear();
            this.Close();
        }

        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(TBAdditionalPath.Text))
                TVExplorer.AddCustomSelectableNodeExt(TBAdditionalPath.Text, _type);
        }

        private void GotToPath_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(TBAdditionalPath.Text))
            {
                TVExplorer.Items.Clear();
                IEnumerable<string> dirs = Directory.GetDirectories(TBAdditionalPath.Text);
                IEnumerable<string> files = Directory.GetFiles(TBAdditionalPath.Text);

                foreach (string dir in dirs)
                    TVExplorer.AddCustomSelectableNodeExt(dir, _type);

                foreach (string file in files)
                    TVExplorer.AddCustomSelectableNodeExt(file, _type);
            }
        }
    }
}
