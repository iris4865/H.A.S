using Management;
using Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;


namespace WpfServer
{
    public partial class MainWindow : Window
    {
        ServerApp app = new ServerApp();
        Thread appThread;

        public MainWindow()
        {
            InitializeComponent();
            CustomInitialize();
        }

        void CustomInitialize()
        {
            Height = SystemParameters.MaximizedPrimaryScreenHeight / 2;
            Width = SystemParameters.MaximizedPrimaryScreenWidth / 2;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            app.Initialize();
            app.Start();
            appThread = app.GetThread;
            Closed += (object sender, EventArgs e) => appThread.Abort();

            UpdateSize();
        }

        void UpdateSize()
        {
            dockPanel.Width = Width;
            dockPanel.Height = Height - SystemParameters.WindowCaptionHeight;

            LeftStateList.Width = Width / 4;
            rightBox.Width = Width / 4;
        }

        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                UpdateSize();
            }
            catch (Exception a)
            {
                Trace.WriteLine(a.Data);
            }
            //imcomplete
            logDisplay.Height = inputBox.PointToScreen(new Point(0, 0)).Y - 300;
        }

        void Exited(object sender, EventArgs e)
        {
            appThread.Abort();
        }
    }
}