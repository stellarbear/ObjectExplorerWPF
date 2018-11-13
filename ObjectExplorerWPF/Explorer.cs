using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace ObjectExplorerWPF
{
    public enum ESelectionRestrictions
    {
        DirOnlySingle,
        DirOnlyMultiple,
        FileOnlySingle,
        FileOnlyMultiple,
        DirAndFileSingle,
        DirAndFileMultiple
    }

    public class Explorer
    {
        public Regex Filter { get; private set; }
        public ESelectionRestrictions SelectionRestrictions { get; private set; }
        public ObservableCollection<ExplorerNode> Nodes { get; private set; }
        public Explorer(ESelectionRestrictions selectionRestrictions, string filter)
        {
            Filter = WildCardToRegular(filter);
            SelectionRestrictions = selectionRestrictions;
            Nodes = new ObservableCollection<ExplorerNode>();
        }
        public Explorer(IEnumerable<string> dirs, ESelectionRestrictions selectionRestrictions, string filter) : this(selectionRestrictions, filter)
        {
            foreach (string dir in dirs)
            {
                AppendNode(dir);
            }
        }
        public void AppendNode(string path)
        {
            if (Directory.Exists(path) || File.Exists(path))
            {
                Nodes.Add(new ExplorerNode(path, SelectionRestrictions, Filter));
            }
        }
        private Regex WildCardToRegular(string filter)
        {
            return new Regex("^" + Regex.Escape(filter).Replace("\\?", ".").Replace("\\*", ".*") + "$", RegexOptions.Compiled);
        }
        public IEnumerable<string> GetCheckedNodes()
        {
            foreach (ExplorerNode node in Nodes)
            {
                foreach (string path in node.GetCheckedNodes())
                    yield return path;
            }
        }
    }

    public class ExplorerNode : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string PropertyName = default(string)) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        public Regex Filter { get; private set; }
        public ESelectionRestrictions SelectionRestrictions { get; private set; }
        public string Path { get; private set; }
        public string Name { get; private set; }
        public ImageSource Icon { get; private set; }

        public bool IsDirectory { get; private set; }
        public bool IsChecked { get; set; }

        public ObservableCollection<ExplorerNode> Nodes { get; private set; }

        public ExplorerNode(string path, ESelectionRestrictions selectionRestrictions, Regex filter)
        {
            Path = path;
            Filter = filter;
            IsChecked = false;
            Name = UpdateName(Path);
            SelectionRestrictions = selectionRestrictions;
            Icon = AuxiliarySharp.IO.ShellIcon.GetShellIcon(Path);
            IsDirectory = Alphaleonis.Win32.Filesystem.Directory.Exists(Path);

            Nodes = new ObservableCollection<ExplorerNode>();

            //  Adding fake path
            if (IsDirectory)
                Nodes.Add(new ExplorerNode(null, selectionRestrictions, filter));
        }
        private string UpdateName(string name)
        {
            if (name == null)
            {
                return null;
            }

            string shortenedName = Alphaleonis.Win32.Filesystem.Path.GetFileName(name);

            shortenedName = shortenedName == "" ? name : shortenedName;
            return shortenedName;
        }

        public async Task ExtendNodeAsync()
        {
            if (IsDirectory)
            {
                string[] childFiles = Alphaleonis.Win32.Filesystem.Directory.GetFiles(Path);
                string[] childFolders = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(Path);

                Nodes.Clear();

                foreach (string folder in childFolders)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Nodes.Add(new ExplorerNode(folder, SelectionRestrictions, Filter));

                    });
                    await TaskEx.Delay(8);
                }
                foreach (string file in childFiles)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (Filter.IsMatch(file))
                        {
                            Nodes.Add(new ExplorerNode(file, SelectionRestrictions, Filter));
                        }

                    });
                    await TaskEx.Delay(8);
                }
            }
        }
        public void ClearNode()
        {
            Nodes.Clear();
        }
        public IEnumerable<string> GetCheckedNodes()
        {
            if (IsChecked)
                yield return Path;

            foreach (ExplorerNode node in Nodes)
            {
                foreach (string path in node.GetCheckedNodes())
                    yield return path;
            }
        }
    }
}
