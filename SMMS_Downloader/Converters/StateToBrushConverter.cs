using SMMS_Downloader.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SMMS_Downloader.Converters
{
    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskState state = (TaskState)value;
            switch(state)
            {
                case TaskState.Wait:
                    return new SolidColorBrush(Color.FromArgb(50, 3, 169, 244));
                case TaskState.Downloading:
                    return new SolidColorBrush(Color.FromArgb(50, 63, 81, 181));
                case TaskState.Error:
                    return new SolidColorBrush(Color.FromArgb(50, 244, 67, 54));
                case TaskState.Done:
                    return new SolidColorBrush(Color.FromArgb(50, 76, 175, 80));
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
