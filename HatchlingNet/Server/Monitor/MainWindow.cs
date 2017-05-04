using System.Windows;
using System.Windows.Media;

namespace Management
{
    class MainWindow : Window
    {
        public MainWindow(string windowTitle, double height, double width, Brush backgroundColor)
        {
            Title = windowTitle;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Height = height;
            Width = width;
            Background = backgroundColor;
            Show();
            Server.Monitor.MainWpf a;
        }
    }
}
