using ObjectExplorerWPF;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ObjectExplorerWPF.Converters
{
    public class SelectionRestrictionToCheckBoxConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ESelectionRestrictions && values[1] is bool)
            {
                ESelectionRestrictions restriction = (ESelectionRestrictions)values[0];
                bool isDirectory = (bool)values[1];

                switch (restriction)
                {
                    case ESelectionRestrictions.DirOnlySingle:
                        return Visibility.Collapsed;
                    case ESelectionRestrictions.DirOnlyMultiple:
                        return isDirectory ? Visibility.Visible : Visibility.Collapsed;
                    case ESelectionRestrictions.FileOnlySingle:
                        return Visibility.Collapsed;
                    case ESelectionRestrictions.FileOnlyMultiple:
                        return !isDirectory ? Visibility.Visible : Visibility.Collapsed;
                    case ESelectionRestrictions.DirAndFileSingle:
                        return Visibility.Collapsed;
                    case ESelectionRestrictions.DirAndFileMultiple:
                        return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SelectionRestrictionToRadioButtonConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ESelectionRestrictions && values[1] is bool)
            {
                ESelectionRestrictions restriction = (ESelectionRestrictions)values[0];
                bool isDirectory = (bool)values[1];

                switch (restriction)
                {
                    case ESelectionRestrictions.DirOnlySingle:
                        return isDirectory ? Visibility.Visible : Visibility.Collapsed;
                    case ESelectionRestrictions.DirOnlyMultiple:
                        return Visibility.Collapsed;
                    case ESelectionRestrictions.FileOnlySingle:
                        return !isDirectory ? Visibility.Visible : Visibility.Collapsed;
                    case ESelectionRestrictions.FileOnlyMultiple:
                        return Visibility.Collapsed;
                    case ESelectionRestrictions.DirAndFileSingle:
                        return Visibility.Visible;
                    case ESelectionRestrictions.DirAndFileMultiple:
                        return Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
