using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.Data;

namespace MosoteachCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 用户登录
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
            if (loginWindow.DialogResult != true)
            {
                this.Close();
            }
        }

        // 更新 消息日志
        public delegate void UpdateMessageDelegate(string message);
        private void UpdateMessage(string message)
        {
            log.Content = message;
        }

        // 更新 DataGrid
        public delegate void UpdateResourceDelegate();
        private void UpdateResource()
        {
            dataGrid.ItemsSource = null;
            if (Administrator.Resource != null)
            {
                dataGrid.ItemsSource = Administrator.Resource.DefaultView;
            }
        }
        // 更新 选择数量
        public delegate void UpdateSelectedCountDelegate(string count);
        private void UpdateSelectedCount(string count)
        {
            if (count == "0")
            {
                checkAll.IsChecked = false;
            }
            labSeclectInfo.Content = $"已选择 {count} 个，共 {Administrator.Resource.Rows.Count} 个";
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Administrator.UserCourse != null && Administrator.UserCourse.data != null)
            {
                for (int i = 0; i < Administrator.UserCourse.data.Count; i++)
                {
                    string name = Administrator.UserCourse.data[i].course.name;
                    cboCourseName.Items.Add(name);
                }
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (cboCourseName.Text.Length == 0)
            {
                UpdateMessage("请 选 择 课 程 !");
                return;
            }

            dataGrid.ItemsSource = null;
            labSeclectInfo.Content = "已选择 0 个，共 0 个";
            checkAll.IsChecked = false;
            

            Administrator.SelectCourseName = cboCourseName.Text;
            Administrator.Resource = null;

            Thread thread = new Thread(new ThreadStart(SelectItem));
            thread.IsBackground = true;
            thread.Start();

        }

        private void SelectItem()
        {
            Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "正 在 查 询，请 稍 后 . . . . . .");

            DataTable resource = new DataTable();
            resource.Columns.Add("select", typeof(bool));
            resource.Columns.Add("name", typeof(string));
            resource.Columns.Add("url", typeof(string));
            var result = Crawler.GetCourseRes(resource);
            switch (result.Result)
            {
                case false:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "未 知 错 误 !");
                    return;
                case true:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "成 功 获 取 资 源 !");
                    Administrator.Resource = resource;
                    break;
            }

            Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "正 在 加 载 资 源，请 稍 后 . . . . . .");
            Dispatcher.Invoke(new UpdateResourceDelegate(UpdateResource), null);
            Dispatcher.Invoke(new UpdateSelectedCountDelegate(UpdateSelectedCount), "0");
            Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "已 加 载 资 源 !");
        }

        private void checkAll_Click(object sender, RoutedEventArgs e)
        {
            if (Administrator.Resource == null || Administrator.Resource.Rows.Count == 0)
            {
                UpdateMessage("请 选 择 课 程 并 查 询 !");
                checkAll.IsChecked = false;
                return;
            }

            if (checkAll.IsChecked == true)
            {
                for (int i = 0; i < Administrator.Resource.Rows.Count; i++)
                {
                    Administrator.Resource.Rows[i][0] = true;
                }
                labSeclectInfo.Content = $"已选择 {Administrator.Resource.Rows.Count} 个，共 {Administrator.Resource.Rows.Count} 个";
                UpdateResource();
                return;
            }

            if (checkAll.IsChecked == false)
            {
                for (int i = 0; i < Administrator.Resource.Rows.Count; i++)
                {
                    Administrator.Resource.Rows[i][0] = false;
                }
                labSeclectInfo.Content = "已选择 0 个，共 0 个";
                UpdateResource();
                return;
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (Administrator.Resource == null || Administrator.Resource.Rows.Count == 0)
            {
                UpdateMessage("请 选 择 课 程 并 查 询 !");
                return;
            }

            Thread thread = new Thread(new ThreadStart(Download));
            thread.IsBackground = true;
            thread.Start();
        }

        private void Download()
        {
            Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "正 在 后 台 下 载，请 勿 进 行 其 他 操 作 或 关 闭 程 序 !");

            var result = Crawler.DownloadRes();

            switch (result.Result)
            {
                case false:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "未 知 错 误 !");
                    return;
                case true:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "所 选 资 源 已 全 部 下 载 到 程 序 根 目 录 !");
                    break;
            }
            Dispatcher.Invoke(new UpdateResourceDelegate(UpdateResource), null);
            Dispatcher.Invoke(new UpdateSelectedCountDelegate(UpdateSelectedCount), "0");
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int selectNum = 0;
            for (int i = 0; i < Administrator.Resource.Rows.Count; i++)
            {
                if (Administrator.Resource.Rows[i][0].Equals(true))
                {
                    selectNum++;
                }
            }
            labSeclectInfo.Content = $"已选择 {selectNum} 个，共 {Administrator.Resource.Rows.Count} 个";
        }
    }
}
