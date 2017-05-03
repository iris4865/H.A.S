using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualBasic.Devices;

namespace Management
{
    class Monitor
    {
        private static readonly Lazy<Monitor> instance = new Lazy<Monitor>(() => new Monitor());
        public static Monitor Instance => instance.Value;

        PerformanceCounter cpuUsage;
        PerformanceCounter cpuTotalUsage;
        PerformanceCounter memUsage;
        PerformanceCounter memAvailable;
        PerformanceCounter memTotal;

        private Monitor()
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
            while(true)
            {
                ComputerInfo info = new ComputerInfo();
                

                Console.WriteLine($"thisPcpu: {cpuUsage.NextValue()}, TOTAL: {cpuTotalUsage.NextValue()}.");
                Console.WriteLine($"mem: {memUsage.NextValue()/1024/1024}MB, available: {memAvailable.NextValue()/1024}, total: {((double)info.TotalPhysicalMemory)/1024/1024/1024}");
                Thread.Sleep(1500);
            }
        }
    }
}
