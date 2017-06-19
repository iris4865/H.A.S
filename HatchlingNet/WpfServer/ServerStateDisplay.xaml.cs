using Management;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace WpfServer
{
    public partial class ServerStateDisplay : Grid
    {
        StateObserver stateObserver = StateObserver.Instance;

        List<StateMenuItem> stateMenuInstance = new List<StateMenuItem>();
        ServerMonitor monitor = ServerMonitor.Instance;

        public ServerStateDisplay()
        {
            InitializeComponent();
            StateMenuInitialize();
        }

        void StateMenuInitialize()
        {
            foreach (string name in monitor.Names)
            {
                //StateMenuItem UsageMenu = new StateMenuItem(name, name, stateObserver);
                stateMenuInstance.Add(new StateMenuItem(name, name, stateObserver));
            }

            LeftStateList.ItemsSource = stateMenuInstance;
        }

        void RealtimeValueGraphBindingToObserver(object sender, RoutedEventArgs e)
        {
            bool isGraphVisualizer = sender is GraphVisualizer;
            FrameworkElement realtimeValueGraph
                = isGraphVisualizer ? sender as GraphVisualizer : sender as FrameworkElement;

            StateMenuItem nowMenuInstance = realtimeValueGraph.DataContext as StateMenuItem;

            if (nowMenuInstance == null)
                throw new ArgumentException("nowMenuInstance is not type of StateMenuItem!");

            MultiBinding widthBindingToRealtimeValue = new MultiBinding();

            Binding propertyBinding = new Binding()
            {
                Path = new PropertyPath(nowMenuInstance.Property),
                Source = stateObserver,
                Mode = BindingMode.OneWay,
                StringFormat = "{0:F2}"
            };

            widthBindingToRealtimeValue.Bindings.Add(propertyBinding);

            Binding maxWidthBinding = new Binding()
            {
                Path = new PropertyPath("MaxWidth"),
                RelativeSource = new RelativeSource(RelativeSourceMode.Self)
            };
            widthBindingToRealtimeValue.Bindings.Add(maxWidthBinding);

            widthBindingToRealtimeValue.Converter
                = new PercentageValueToGraphWidthConverter();

            if (isGraphVisualizer)
                realtimeValueGraph.SetBinding(GraphVisualizer.DesiredWidthProperty, widthBindingToRealtimeValue);
            else
                realtimeValueGraph.SetBinding(WidthProperty, widthBindingToRealtimeValue);

        }

        private void RealtimeValueTextBindingToObserver(object sender, RoutedEventArgs e)
        {
            Run realtimeValueText = sender as Run;

            ServerMonitor monitor = ServerMonitor.Instance;


            foreach (string name in monitor.Names)
            {
                if (realtimeValueText.Text.CompareTo(name) == 0)
                {
                    Binding b = new Binding(name)
                    {
                        Path = new PropertyPath(name),
                        Source = stateObserver,
                        StringFormat = "{0:F2}%",
                        Mode = BindingMode.OneWay
                        
                    };
                    realtimeValueText.SetBinding(Run.TextProperty, b);
                }
            }


            //if (realtimeValueText.Text.CompareTo("CpuUsage") == 0)
            //{
            //    Binding b = new Binding("CpuUsage")
            //    {
            //        Path = new PropertyPath("CpuUsage"),
            //        Source = stateObserver,
            //        StringFormat = "{0:F2}%",
            //        Mode = BindingMode.OneWay
            //    };
            //    realtimeValueText.SetBinding(Run.TextProperty, b);
            //}
            //else if (realtimeValueText.Text.CompareTo("MemoryUsage") == 0)
            //{
            //    Binding b = new Binding("MemoryUsage")
            //    {
            //        Path = new PropertyPath("MemoryUsage"),
            //        Source = stateObserver,
            //        StringFormat = "{0:F2}%",
            //        Mode = BindingMode.OneWay
            //    };
            //    realtimeValueText.SetBinding(Run.TextProperty, b);
            //}
        }
    }
}
