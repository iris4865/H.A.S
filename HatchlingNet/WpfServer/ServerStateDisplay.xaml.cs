using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfServer
{
    public partial class ServerStateDisplay : Grid
    {
        StateObserver stateObserver = StateObserver.Instance;

        List<StateMenuItem> stateMenuInstance = new List<StateMenuItem>();

        public ServerStateDisplay()
        {
            InitializeComponent();
            StateMenuInitialize();
        }

        void StateMenuInitialize()
        {
            StateMenuItem cpuUsageMenu = new StateMenuItem("CPU Usage", "cpuRate", stateObserver);
            stateMenuInstance.Add(cpuUsageMenu);

            StateMenuItem ramUsageMenu = new StateMenuItem("RAM Usage", "ramUsage", stateObserver);
            stateMenuInstance.Add(ramUsageMenu);

            StateMenuItem ramTotalMenu = new StateMenuItem("Total Ram", "ramTotal", stateObserver);
            stateMenuInstance.Add(ramTotalMenu);
            

            LeftStateList.ItemsSource = stateMenuInstance;
        }

        private void RealtimeValueGraphBindingToObserver(object sender, RoutedEventArgs e)
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

            if (realtimeValueText.Text.CompareTo("cpuRate") == 0)
            {
                Binding b = new Binding("cpuRate")
                {
                    Path = new PropertyPath("cpuRate"),
                    Source = stateObserver,
                    StringFormat = "{0:F2}%",
                    Mode = BindingMode.OneWay
                };
                realtimeValueText.SetBinding(Run.TextProperty, b);
            }
            else if (realtimeValueText.Text.CompareTo("ramUsage") == 0)
            {
                Binding b = new Binding("ramUsage")
                {
                    Path = new PropertyPath("ramUsage"),
                    Source = stateObserver,
                    StringFormat = "{0:F2}%",
                    Mode = BindingMode.OneWay
                };
                realtimeValueText.SetBinding(Run.TextProperty, b);
            }
            else if (realtimeValueText.Text.CompareTo("ramTotal") == 0)
            {
                Binding b = new Binding("ramTotal")
                {
                    Path = new PropertyPath("ramTotal"),
                    Source = stateObserver,
                    StringFormat = "{0:F2}MB",
                    Mode = BindingMode.OneWay
                };
                realtimeValueText.SetBinding(Run.TextProperty, b);
            }
        }
    }
}
