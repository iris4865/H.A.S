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

using Server;
using System.Windows.Threading;
using Management;

namespace WpfServer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerApp server;
        DispatcherTimer timer = new DispatcherTimer();
        ServerMonitor serverState = ServerMonitor.Instance;

        public MainWindow()
        {
            InitializeComponent();
            CustomInitialize();
            //ServerStart();
            Update();
        }

        void CustomInitialize()
        {
            Height = SystemParameters.MaximizedPrimaryScreenHeight / 2;
            Width = SystemParameters.MaximizedPrimaryScreenWidth / 2;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Background = Brushes.Black;
        }

        void ServerStart()
        {
            server = new ServerApp();
            server.Initialize();
            server.Start();
        }

        void Update()
        {
            timer.Tick += UpdateCpuUsage;
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Start();
        }

        void UpdateCpuUsage(object sender, EventArgs e)
        {
            Content = $"CPU: {serverState.GetUsageCpu()}%\n" +
                $"RAM Usage: {serverState.GetMemoryUsage()}\n" +
                $"RAM Total: {serverState.GetMemoryTotal()}GB";
        }
    }
}
