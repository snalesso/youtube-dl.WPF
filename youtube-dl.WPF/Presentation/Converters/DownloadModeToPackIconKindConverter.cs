using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using youtube_dl.WPF.Core.Models;

namespace youtube_dl.WPF.Presentation.Converters
{
    public class DownloadModeToPackIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var downloadMode = (DownloadMode)value;

            switch (downloadMode)
            {
                case DownloadMode.AudioOnly:
                    return PackIconKind.Music;
                case DownloadMode.AudioVideo:
                    return PackIconKind.Video;
                case DownloadMode.VideoOnly:
                    return PackIconKind.MusicOff;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
