using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Management
{
    public class ServerMonitor
    {
        static readonly Lazy<ServerMonitor> instance = new Lazy<ServerMonitor>(() => new ServerMonitor());
        public static ServerMonitor Instance => instance.Value;

        readonly Dictionary<string, MonitorElement> monitorList = new Dictionary<string, MonitorElement>();

        readonly PerformanceCounter cpuTotalUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        readonly PerformanceCounter memUsage = new PerformanceCounter("Memory", "Available MBytes");
        readonly ComputerInfo info = new ComputerInfo();

        float CpuUsage() => cpuTotalUsage.NextValue();
        float MemoryUsage() => MemoryTotal() - memUsage.NextValue();
        float MemoryTotal() => (float)info.TotalPhysicalMemory / 1024 / 1024;

        public string[] Names => monitorList.Keys.ToArray();
        public MonitorElement this[string name] => monitorList[name];

        ServerMonitor()
        {
            AddList(CpuUsage, 100, "%");
            AddList(MemoryUsage, MemoryTotal(), "MB");
        }

        void AddList(Func<float> getMethod, float totalValue, string unit) => monitorList[getMethod.Method.Name] = new MonitorElement(getMethod, totalValue, unit);
    }
}