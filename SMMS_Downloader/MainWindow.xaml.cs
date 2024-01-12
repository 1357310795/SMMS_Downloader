using CommunityToolkit.Mvvm.ComponentModel;
using SMMS_Downloader.Extensions;
using SMMS_Downloader.Helpers;
using SMMS_Downloader.Modules;
using SMMS_Downloader.Services;
using SMMS_Downloader.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace SMMS_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        [ObservableProperty]
        private string account;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private ObservableCollection<ImageViewModel> images;

        [ObservableProperty]
        private int total;

        [ObservableProperty]
        private int loadedCount;

        [ObservableProperty]
        private int loadedPage;

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            var res = ApiService.Login(Account, Password);
            if (!res.success)
            {
                MessageBox.Show($"登录失败：{res.result}");
                return;
            }
            MessageBox.Show($"登录成功！");
            TokenStorage.Default.Save();
        }

        private void ButtonRead_Click(object sender, RoutedEventArgs e)
        {
            if (!ApiService.IsLogined)
            {
                MessageBox.Show($"请先登录");
                return;
            }
            Task.Run(GetAllImages);
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            if (Images == null)
            {
                MessageBox.Show("请先读取列表");
                return;
            }
            MessageBox.Show("下载任务开始");
            Task.Run(StartDownload);
        }

        private void LinkCopy_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Hyperlink)sender).DataContext as ImageViewModel;
            Clipboard.SetText(item.Url);
            MessageBox.Show("已复制");
        }

        private void LinkView_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Hyperlink)sender).DataContext as ImageViewModel;
            LaunchHelper.OpenURL(item.WebPage);
        }

        private void GetAllImages()
        {
            Images = new ObservableCollection<ImageViewModel>();
            Total = LoadedCount = LoadedPage = 0;
            var res1 = ApiService.GetUploadHistory(1);
            if (!res1.Success)
            {
                MessageBox.Show($"发生异常：{res1.Message}");
                return;
            }

            var db = DbService.db;
            db.CreateTable<SmmsUploadHistoryItem>();
            var table = db.Table<SmmsUploadHistoryItem>();

            foreach(var item in res1.Result.Data)
            {
                if (!table.Any(x => x.Url == item.Url))
                    db.Insert(item);
            }
            this.Dispatcher.Invoke(() => {
                foreach (var item in res1.Result.Data)
                    Images.Add(new ImageViewModel()
                    {
                        Id = item.Id,
                        Url = item.Url,
                        Name = item.Filename,
                        Downloaded = false,
                        UploadDate = item.CreatedAt,
                        Width = item.Width,
                        Height = item.Height,
                        Size = item.Size.PrettyPrint(),
                        StoreName = item.Storename,
                        WebPage = item.Page,
                        State = TaskState.Wait
                    });
            });
            Total = res1.Result.TotalPages;
            LoadedPage += 1;
            LoadedCount += res1.Result.Data.Count;
            for (int i = 2; i <= Total; i++)
            {
                res1 = ApiService.GetUploadHistory(i);
                if (!res1.Success)
                {
                    MessageBox.Show($"发生异常：{res1.Message}");
                    return;
                }

                foreach (var item in res1.Result.Data)
                {
                    if (!table.Any(x => x.Url == item.Url))
                        db.Insert(item);
                }
                this.Dispatcher.Invoke(() => {
                    foreach (var item in res1.Result.Data)
                        Images.Add(new ImageViewModel()
                        {
                            Id = item.Id,
                            Url = item.Url,
                            Name = item.Filename,
                            Downloaded = false,
                            UploadDate = item.CreatedAt,
                            Width = item.Width,
                            Height = item.Height,
                            Size = item.Size.PrettyPrint(),
                            StoreName = item.Storename,
                            WebPage = item.Page,
                            State = TaskState.Wait
                        });
                });
                LoadedPage += 1;
                LoadedCount += res1.Result.Data.Count;
            }
        }

        private void StartDownload()
        {
            var path = Path.Combine(PathHelper.AppPath, "images");
            Directory.CreateDirectory(path);
            HttpClient client = new HttpClient(new HttpClientHandler());
            var db = DbService.db;
            db.CreateTable<SmmsUploadHistoryItem>();
            var table = db.Table<SmmsUploadHistoryItem>();

            foreach (var item in Images)
            {
                if (item.State == TaskState.Done)
                    continue;
                item.State = TaskState.Downloading;
                try
                {
                    var res = client.GetAsync(item.Url).GetAwaiter().GetResult();
                    if (!res.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"下载 {item.Url} 时发生错误：服务器返回 {res.StatusCode}");
                        item.State = TaskState.Error;
                        return;
                    }
                    var imagestream = res.Content.ReadAsStream();
                    var filestream = new FileStream(Path.Combine(path, item.StoreName), FileMode.Create, FileAccess.Write);
                    imagestream.CopyTo(filestream);
                    filestream.Flush();
                    filestream.Close();
                    item.State = TaskState.Done;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"下载 {item.Url} 时发生错误：{ex.Message}");
                    item.State = TaskState.Error;
                    return;
                }
                finally
                {
                    var dbModel = table.Where(x=>x.Id ==  item.Id).FirstOrDefault();
                    dbModel.State = (int)item.State;
                    db.Update(dbModel);
                }
            }
            MessageBox.Show("下载任务完成");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DbService.Init();
            var res = TokenStorage.Default.Read();
            if (res.success)
            {
                MessageBox.Show("读取持久化Token成功！");
            }
            Images = new ObservableCollection<ImageViewModel>();
            var db = DbService.db;
            db.CreateTable<SmmsUploadHistoryItem>();
            var table = db.Table<SmmsUploadHistoryItem>();
            foreach(var item in table)
            {
                Images.Add(new ImageViewModel()
                {
                    Id = item.Id,
                    Url = item.Url,
                    Name = item.Filename,
                    Downloaded = false,
                    UploadDate = item.CreatedAt,
                    Width = item.Width,
                    Height = item.Height,
                    Size = item.Size.PrettyPrint(),
                    StoreName = item.Storename,
                    State = (TaskState)item.State,
                    WebPage = item.Page,
                });
            }
            LoadedCount += Images.Count;
        }
    }
}