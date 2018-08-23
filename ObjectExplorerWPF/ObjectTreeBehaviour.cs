using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ObjectExplorerWPF
{
    public enum ESelectType { Checked, Unchecked, All };
    public enum ESelectionRestrictions { DirOnlySingle, DirOnlyMultiple, FileOnlySingle, FileOnlyMultiple, DirAndFileSingle, DirAndFileMultiple }
    internal static class ObjectTreeBehaviour
    {
        public static void AddCustomSelectableNodeExt(this TreeView tv, string nodeName, ESelectionRestrictions type)
        {
            tv.Items.Add(CreateNode(nodeName, type));
        }
        public static List<string> GetSelectedNodesExt(this TreeView tv, ESelectType type)
        {
            List<string> result = new List<string>();

            foreach (TreeViewItem node in tv.Items)
            {
                result.AddRange(GetSelectedNodes(node, type));
            }

            return result;
        }

        private static List<string> GetSelectedNodes(TreeViewItem node, ESelectType type)
        {
            List<string> result = new List<string>();

            if (node.Items.Count > 0)
            {
                foreach (TreeViewItem child in node.Items)
                {
                    result.AddRange(GetSelectedNodes(child, type));
                }
            }

            bool checkState = false;
            if (node.Header != null)
            {
                object control = (node.Header as Grid)?.Children[0];

                if (control is CheckBox)
                    checkState = (control as CheckBox).IsChecked == true;
                else if (control is RadioButton)
                    checkState = (control as RadioButton).IsChecked == true;
            }

            switch (type)
            {
                case ESelectType.Checked:
                    if (checkState)
                        result.Add(node.Tag.ToString());
                    break;
                case ESelectType.Unchecked:
                    if (!checkState)
                        result.Add(node.Tag.ToString());
                    break;
                case ESelectType.All:
                    result.Add(node.Tag.ToString());
                    break;
            }

            return result;
        }

        private static TreeViewItem CreateNode(string nodeName, ESelectionRestrictions type)
        {
            Grid nodeStructure = new Grid();
            ColumnDefinition checkColumn = new ColumnDefinition() { Width = GridLength.Auto },
                iconColumn = new ColumnDefinition() { Width = GridLength.Auto },
                nodeColumn = new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Star) };

            nodeStructure.ColumnDefinitions.Add(checkColumn);
            nodeStructure.ColumnDefinitions.Add(iconColumn);
            nodeStructure.ColumnDefinitions.Add(nodeColumn);

            UIElement checkControl = new Control();

            switch (type)
            {
                case ESelectionRestrictions.FileOnlySingle:
                    if (File.Exists(nodeName))
                        checkControl = (CreateEditButton(ButtonEditType.Single));
                    else
                        checkControl = (CreateEditButton(ButtonEditType.None));
                    break;
                case ESelectionRestrictions.FileOnlyMultiple:
                    if (File.Exists(nodeName))
                        checkControl = (CreateEditButton(ButtonEditType.Multiple));
                    else
                        checkControl = (CreateEditButton(ButtonEditType.None));
                    break;
                case ESelectionRestrictions.DirOnlySingle:
                    if (Directory.Exists(nodeName))
                        checkControl = (CreateEditButton(ButtonEditType.Single));
                    else
                        checkControl = (CreateEditButton(ButtonEditType.None));
                    break;
                case ESelectionRestrictions.DirOnlyMultiple:
                    if (Directory.Exists(nodeName))
                        checkControl = (CreateEditButton(ButtonEditType.Multiple));
                    else
                        checkControl = (CreateEditButton(ButtonEditType.None));
                    break;
                case ESelectionRestrictions.DirAndFileSingle:
                    checkControl = (CreateEditButton(ButtonEditType.Single));
                    break;
                case ESelectionRestrictions.DirAndFileMultiple:
                    checkControl = (CreateEditButton(ButtonEditType.Multiple));
                    break;
            }

            //  Иконка
            UIElement iconControl = new Image()
            {
                Width = 20,
                Height = 20,
                Margin = new Thickness(4, 0, 0, 0),
                Source = AuxiliarySharp.IO.ShellIcon.GetShellIcon(nodeName)
            };

            UIElement nodeControl = new Label()
            {
                Content = ((Path.GetFileName(nodeName) == "") ? nodeName :
                                Path.GetFileName(nodeName))
            };


            nodeStructure.Children.Add(checkControl);
            Grid.SetColumn(checkControl, 0);
            nodeStructure.Children.Add(iconControl);
            Grid.SetColumn(iconControl, 1);
            nodeStructure.Children.Add(nodeControl);
            Grid.SetColumn(nodeControl, 2);

            TreeViewItem rootNode = new TreeViewItem()
            {
                Padding = new Thickness(0),
                Tag = nodeName,
                Header = nodeStructure
            };


            //  Если имеем дело с директорией, создадим пустой (фейковый) элемент
            if (Directory.Exists(nodeName)) rootNode.Items.Add(new TreeViewItem() { Tag = null, MaxHeight = 0 });

            //  Формируем наполнение вершин следующего
            rootNode.Expanded += (sender, e) => NodeExpand_Event(sender, type);

            //rootNode.Style = Application.Current.Resources["MTreeViewItem"] as Style;
            return rootNode;
        }

        private static void NodeExpand_Event(object sender, ESelectionRestrictions type)
        {
            PopulateNode(sender as TreeViewItem, type);
        }
        private static async void PopulateNode(TreeViewItem node, ESelectionRestrictions type)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            string nodeName = node.Tag.ToString();
            if (Directory.Exists(nodeName))
            {
                if ((node.Items.Count == 1))
                {
                    try
                    {
                        string[] baseFiles = Directory.GetFiles(nodeName);
                        string[] baseFolders = Directory.GetDirectories(nodeName);

                        foreach (string baseFolder in new ObservableCollection<string>(baseFolders))
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                TreeViewItem tvi = CreateNode(baseFolder, type);
                                if (tvi != null)
                                {
                                    tvi.Style = node.Style;
                                    node.Items.Add(tvi);
                                }

                            });
                            await TaskEx.Delay(8);    //  GUI
                        }
                        foreach (string baseFile in new ObservableCollection<string>(baseFiles))
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                TreeViewItem tvi = CreateNode(baseFile, type);
                                if (tvi != null)
                                {
                                    tvi.Style = node.Style;
                                    node.Items.Add(tvi);
                                }
                            });
                            await TaskEx.Delay(8);    //  GUI
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        enum ButtonEditType { Single, Multiple, None }
        private static UIElement CreateEditButton(ButtonEditType buttonType)
        {
            switch (buttonType)
            {
                case ButtonEditType.Multiple:
                    return (new CheckBox()
                    {
                        IsChecked = false,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(0, 0, 4, 0)
                    });
                case ButtonEditType.Single:
                    return (new RadioButton()
                    {
                        IsChecked = false,
                        GroupName = "RB",
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(0, 0, 4, 0)
                    });

            }

            return (new RadioButton()
            {
                IsChecked = false,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 20,
                Height = 20,
                Margin = new Thickness(0, 0, 4, 0),
                Visibility = Visibility.Collapsed
            });
        }
    }
}
