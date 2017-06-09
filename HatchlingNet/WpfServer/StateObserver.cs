using Management;
using System;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Data;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Diagnostics;

namespace WpfServer
{
    public class StateObserver : INotifyPropertyChanged
    {
        private static readonly Lazy<StateObserver> instance 
            = new Lazy<StateObserver>(() => new StateObserver());

        public static StateObserver Instance => instance.Value;

        DispatcherTimer refresher = new DispatcherTimer();
        ServerMonitor serverState = ServerMonitor.Instance;
        public event PropertyChangedEventHandler PropertyChanged;

        private static double periodValueRefreshing = 500;

        private float serverCPURate = (float)0.0;
        private float serverRAMUsage = (float)0.0;
        private float serverRAMTotal = (float)0.0;

        volatile private bool updaterIsOn = false;

        private StateObserver()
        {
            refresher.Tick += UpdateServerState;
        }

        private void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

        public float cpuRate{
            private set
            {
                serverCPURate = value;
                OnPropertyChanged("cpuRate");
            }
            get
            {
                if (!updaterIsOn)
                    UpdaterOn();

                return serverCPURate;
            }
        }

        public float ramUsage
        {
            private set
            {
                serverRAMUsage = value;
                OnPropertyChanged("ramUsage");
            }
            get
            {
                if (!updaterIsOn)
                    UpdaterOn();

                return serverRAMUsage;
            }
        }

        public float ramTotal
        {
            private set
            {
                serverRAMTotal = value;
                OnPropertyChanged("ramTotal");
            }
            get
            {
                if (!updaterIsOn)
                    UpdaterOn();

                return serverRAMTotal;
            }
        }

        public static double Period
        {
            private set => periodValueRefreshing = value;
            get
            {
                return periodValueRefreshing;
            }
        }
        
        void UpdaterOn()
        {
            refresher.Interval = TimeSpan.FromMilliseconds(Period);
            refresher.Start();
            updaterIsOn = true;
        }

        public void UpdaterOff()
        {
            refresher.Stop();
            updaterIsOn = false;
        }

        void UpdateServerState(object sender, EventArgs e)
        {
            cpuRate = serverState.CpuUsage();
            ramUsage = serverState.MemoryUsage();
            ramTotal = serverState.MemoryTotal();
        }
    }
}
