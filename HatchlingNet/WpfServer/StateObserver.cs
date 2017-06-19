using Management;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

namespace WpfServer
{
    public class StateObserver : INotifyPropertyChanged
    {
        static readonly Lazy<StateObserver> instance
            = new Lazy<StateObserver>(() => new StateObserver());

        public static StateObserver Instance => instance.Value;

        DispatcherTimer refresher = new DispatcherTimer();
        readonly ServerMonitor serverState = ServerMonitor.Instance;
        public event PropertyChangedEventHandler PropertyChanged;

        float serverCPURate;
        float serverRAMUsage;
        //readonly Dictionary<string, float> serverStateElement = new Dictionary<string, float>();

        volatile bool updaterIsOn;

        public static double PeriodValueRefreshing { get; private set; }
        StateObserver()
        {
            refresher.Tick += UpdateServerState;
            PeriodValueRefreshing = 500;
        }

        void OnPropertyChanged(String Property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }

        public float CpuUsage
        {
            get
            {
                if (!updaterIsOn)
                    UpdaterOn();

                //return serverStateElement["CpuRate"];
                return serverCPURate;
            }
            private set
            {
                serverCPURate = value;
                OnPropertyChanged("CpuUsage");
            }
        }

        public float MemoryUsage
        {
            private set
            {
                serverRAMUsage = value;
                OnPropertyChanged("MemoryUsage");
            }
            get
            {
                if (!updaterIsOn)
                    UpdaterOn();

                return serverRAMUsage;
            }
        }

        void UpdaterOn()
        {
            refresher.Interval = TimeSpan.FromMilliseconds(PeriodValueRefreshing);
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
            //foreach(string name in serverState.Names)
            //    serverStateElement[name] = serverState[name].Percentage;

            CpuUsage = serverState["CpuUsage"].Percentage;
            MemoryUsage = serverState["MemoryUsage"].Percentage;
        }
    }
}
