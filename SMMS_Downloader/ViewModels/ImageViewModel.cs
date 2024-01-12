using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMMS_Downloader.ViewModels
{
    public partial class ImageViewModel : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string size;

        [ObservableProperty]
        private int width;

        [ObservableProperty]
        private int height;

        [ObservableProperty]
        private string uploadDate;

        [ObservableProperty]
        private string url;

        [ObservableProperty]
        private string storeName;

        [ObservableProperty]
        private bool downloaded;

        [ObservableProperty]
        private string webPage;

        [ObservableProperty]
        private TaskState state;
    }

    public enum TaskState
    {
        Wait, Downloading, Error, Done
    }
}
