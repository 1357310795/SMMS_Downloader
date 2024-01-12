using SMMS_Downloader.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SMMS_Downloader.Converters
{
    public class StateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskState state = (TaskState)value;
            switch(state)
            {
                case TaskState.Wait:
                    return "⏱️";
                case TaskState.Downloading:
                    return "▶️";
                case TaskState.Error:
                    return "❌";
                case TaskState.Done:
                    return "✅";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
