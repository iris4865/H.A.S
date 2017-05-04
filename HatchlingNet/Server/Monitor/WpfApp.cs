using System.Windows;
using System.Windows.Media;
using Server;

namespace Management
{
    class WpfApp : System.Windows.Application
    {
        static ServerApp server;
        public void AppStartUp(object sender, StartupEventArgs e)
        {

            double height = SystemParameters.MaximizedPrimaryScreenHeight / 2;
            double width = SystemParameters.MaximizedPrimaryScreenWidth / 2;

            Window mainWindow = new MainWindow("Hatchling Server Monitor", height, width, Brushes.Black);
            mainWindow.Show();
        }

        public void ServerStart()
        {
            server = new ServerApp();
            server.Initialize();
            server.Start();
        }

        public void AppExit(object sender, ExitEventArgs e)
        {
            MessageBox.Show("App has exited");
        }
    }
}
