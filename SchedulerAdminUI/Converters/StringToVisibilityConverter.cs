using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SchedulerAdminUI.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value?.ToString();
            return string.IsNullOrWhiteSpace(text)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}