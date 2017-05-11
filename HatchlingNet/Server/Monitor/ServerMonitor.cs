using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualBasic.Devices;

namespace Management
{
    public class ServerMonitor
    {
        private static readonly Lazy<ServerMonitor> instance = new Lazy<ServerMonitor>(() => new ServerMonitor());
        public static ServerMonitor Instance => instance.Value;

        PerformanceCounter cpuUsage;
        PerformanceCounter cpuTotalUsage;
        PerformanceCounter memUsage;
        PerformanceCounter memAvailable;
        PerformanceCounter memTotal;
        readonly ComputerInfo info = new ComputerInfo();


        private ServerMonitor()
        {
            cpuUsage = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            cpuTotalUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memUsage = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName);
            memAvailable = new PerformanceCounter("Memory", "Available MBytes");
            //memTotal = new PerformanceCounter("Memory", "_total");
        }

        public void Start()
        {
            Thread moniterThread = new Thread(Monitoring);
            moniterThread.Start();
        }

        public void Monitoring()
        {
            while (true)
            {


                Console.WriteLine($"thisPcpu: {cpuUsage.NextValue()}, TOTAL: {cpuTotalUsage.NextValue()}.");
                Console.WriteLine($"mem: {memUsage.NextValue() / 1024 / 1024}MB, available: {memAvailable.NextValue() / 1024}, total: {((double)info.TotalPhysicalMemory) / 1024 / 1024 / 1024}");
                Thread.Sleep(1500);
            }
        }

        public float GetUsageCpu()
        {
            return cpuTotalUsage.NextValue();
        }

        public float GetMemoryUsage()
        {
            return memUsage.NextValue() / 1024 / 1024 / 8;
        }

        public float GetMemoryTotal()
        {
            return (float)info.TotalPhysicalMemory / 1024 / 1024 / 1024;
        }
    }
}
