using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Management
{
    class WindowMain : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        ServerMonitor serverState = ServerMonitor.Instance;

        public WindowMain(string windowTitle, double height, double width, Brush backgroundColor)
        {
            Title = windowTitle;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Height = height;
            Width = width;
            Background = backgroundColor;
            Show();
            Update();
        }

        public void Update()
        {
            timer.Tick += UpdateCpuUsage;
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Start();
        }

        private void UpdateCpuUsage(object sender, EventArgs e)
        {
            Content = $"CPU: {serverState.GetUsageCpu()}%\n" +
                $"RAM Usage: {serverState.GetMemoryUsage()}\n" +
                $"RAM Total: {serverState.GetMemoryTotal()}GB";
        }
    }
}
