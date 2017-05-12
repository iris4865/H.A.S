using Server;
using System.Windows;
using System.Windows.Media;

namespace Management
{
    public class WpfApp
    {
        ServerApp server;
        Application wpf;
        WindowMain windowMain;

        public void Start()
        {
            wpf = new Application();
            wpf.Startup += WpfStartUp;
            wpf.Exit += WpfExit;
            wpf.Run();
        }

        public void WpfStartUp(object sender, StartupEventArgs e)
        {
            double height = SystemParameters.MaximizedPrimaryScreenHeight / 2;
            double width = SystemParameters.MaximizedPrimaryScreenWidth / 2;

            windowMain = new WindowMain("Hatchling Server Monitor", height, width, Brushes.White);
            windowMain.Show();
        }

        public void ServerStart()
        {
            server = new ServerApp();
            server.Initialize();
            server.Start();
        }

        public void WpfExit(object sender, ExitEventArgs e)
        {
        }
        /*
        public void  Deactivated()
        {

        }
        public void  SessionEnding()
        {

        }
        public void  DispatcherUnhandledException()
        {

        }
        public void  Navigating()
        {

        }
        public void  Navigated()
        {

        }
        public void  NavigationProgress()
        {

        }
        public void  NavigationFailed()
        {

        }
        public void  LoadCompleted()
        {

        }
        public void  Activated()
        {

        }
        public void  NavigationStopped()
        {

        }
        public void  FragmentNavigation()
        {

        }
        */
    }
}
