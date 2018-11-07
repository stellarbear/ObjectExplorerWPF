using ObjectExplorerWPF;
using System.Collections.Generic;
using System.Windows;

namespace ObjectExplorerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ObjectExplorer OE = new ObjectExplorer(new List<string> { @"D:\Test", @"D:\Test\log" }, ESelectionRestrictions.DirAndFileMultiple);
            OE.ShowDialog();
        }
    }
}
