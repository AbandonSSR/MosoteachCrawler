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
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;

namespace MosoteachCrawler
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
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

        // 更新 消息日志
        public delegate void UpdateMessageDelegate(string message);
        private void UpdateMessage(string message)
        {
            log.Content = message;
        }
        // 更新 DialogResult
        public delegate void UpdateDialogResultDelegate(bool value);
        private void UpdateDialogResult(bool value)
        {
            this.DialogResult = value;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtAccount.Text.Length == 0)
            {
                UpdateMessage("请 输 入 账 号 !");
                return;
            }
            if (txtPassword.Password.Length == 0)
            {
                UpdateMessage("请 输 入 密 码 !");
                return;
            }

            Administrator.Account = this.txtAccount.Text;
            Administrator.Password = this.txtPassword.Password;

            Thread thread = new Thread(Login);
            thread.IsBackground = true;
            thread.Start();
        }

        private void Login()
        {
            Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "正 在 登 录，请 稍 后 . . . . . .");

            var result = Crawler.GetUserInfo();
            switch (result.Result)
            {
                case false:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "账 号 或 密 码 有 误 !");
                    return;
                case true:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "登 录 成 功 !");
                    break;
            }

            Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "正 在 获 取 课 程，请 稍 后 . . . . . .");

            result = Crawler.GetUserCourse();
            switch (result.Result)
            {
                case false:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "未 知 错 误 !");
                    return;
                case true:
                    Dispatcher.Invoke(new UpdateMessageDelegate(UpdateMessage), "成 功 获 取 课 程 !");
                    break;
            }

            Dispatcher.Invoke(new UpdateDialogResultDelegate(UpdateDialogResult), true);
        }
    }
}
