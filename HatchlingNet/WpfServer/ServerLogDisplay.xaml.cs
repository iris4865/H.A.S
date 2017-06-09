using Management;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfServer
{
    /// <summary>
    /// ServerLogDisplay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ServerLogDisplay : TextBox
    {
        LogTracedListener trace = new LogTracedListener();

        DispatcherTimer testClockTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(800)
        };

        public ServerLogDisplay()
        {
            InitializeComponent();

            PropertyChangedEventManager.AddHandler(trace, TraceOnPropertyChanged, "Data");

            Trace.Listeners.Add(trace);
        }

        private void TraceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Data")
                return;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Text = trace.Data; }));
        }

        private void TestWriteLogTracing(object sender, EventArgs e)
        {
            trace.WriteLine("Logtracing is activating now normally...");
        }
    }
}
