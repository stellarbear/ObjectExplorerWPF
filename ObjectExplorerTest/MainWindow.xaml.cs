using ObjectExplorerWPF;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            List<string> ObservableDirs = new List<string>();

            ObservableDirs.Add(Path.Combine(AuxiliarySharp.IO.General.GetCurrentDirectory(), "results"));
            ObservableDirs.AddRange(DriveInfo.GetDrives().
                Where(x => x.DriveType == System.IO.DriveType.Fixed).
                Select(x => x.Name));

            ObjectExplorer OE = new ObjectExplorer(ObservableDirs, ESelectionRestrictions.DirAndFileMultiple);
            OE.ShowDialog();
        }
    }
}
