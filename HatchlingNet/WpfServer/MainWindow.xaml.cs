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

        DockPanel dock;
        TextBox leftBox;

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

            dock = new DockPanel();
            Content = dock;

            leftBox = new TextBox
            {
                Width = Width / 4,
                IsReadOnly = true
            };
            AddDock(leftBox, Dock.Left);

            TextBox logBox2 = new TextBox
            {
                Text = "test2...",
                Width = Width / 4
            };
            AddDock(logBox2, Dock.Right);
            //Background = Brushes.Black;

            TextBox logBox3 = new TextBox
            {
                Text = "input"
            };
            AddDock(logBox3, Dock.Bottom);

            TextBox logBox4 = new TextBox
            {
                Text = "log"
            };
            AddDock(logBox4);
        }

        void AddDock(UIElement element, Dock position)
        {
            DockPanel.SetDock(element, position);
            dock.Children.Add(element);
        }

        void AddDock(UIElement element)
        {
            dock.Children.Add(element);
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
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        void UpdateCpuUsage(object sender, EventArgs e)
        {
            leftBox.Text = $"CPU: {serverState.GetUsageCpu()}%\n" +
                $"RAM Usage: {serverState.GetMemoryUsage()}\n" +
                $"RAM Total: {serverState.GetMemoryTotal()}GB\n" +
                $"Users: 00\n" +
                $"Packet: 00";
        }
    }
}
