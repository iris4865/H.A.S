using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Management
{
    public class ServerMonitor
    {
        private static readonly Lazy<ServerMonitor> instance = new Lazy<ServerMonitor>(() => new ServerMonitor());
        public static ServerMonitor Instance => instance.Value;

        readonly Dictionary<string, Func<float>> monitorList = new Dictionary<string, Func<float>>();

        readonly PerformanceCounter cpuTotalUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        readonly PerformanceCounter memUsage = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName);
        readonly ComputerInfo info = new ComputerInfo();

        public float CpuUsage() => cpuTotalUsage.NextValue();
        public float MemoryUsage() => memUsage.NextValue() / 1024 / 1024 / 8;
        public float MemoryTotal() => (float)info.TotalPhysicalMemory / 1024 / 1024 / 1024;
        
        public string[] Names => monitorList.Keys.ToArray();
        public float this[string name] => monitorList[name].Invoke();

        private ServerMonitor()
        {
            AddList(CpuUsage);
            AddList(MemoryUsage);
            AddList(MemoryTotal);
        }
        void AddList(Func<float> funcHandler) => monitorList[funcHandler.Method.Name] = funcHandler;

    }
}