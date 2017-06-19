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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfServer
{
    public partial class GraphVisualizer : Label
    {
        public static readonly DependencyProperty DesiredWidthProperty
            = DependencyProperty.Register("DesiredWidth", typeof(double), typeof(GraphVisualizer)
                , new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDesiredWidthPropertyChanged)));
    
        private static void OnDesiredWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GraphVisualizer g = d as GraphVisualizer;

            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = g.Width,
                To = (double?)e.NewValue,
                Duration = TimeSpan.FromMilliseconds(StateObserver.PeriodValueRefreshing),
                AccelerationRatio = 0.5,
                DecelerationRatio = 0.5
            };
            Storyboard.SetTarget(widthAnimation, g);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(GraphVisualizer.WidthProperty));

            Storyboard widthStoryboard = new Storyboard();
            widthStoryboard.Children.Add(widthAnimation);

            widthStoryboard.Begin();
        }

        public GraphVisualizer() : base()
        {
            InitializeComponent();
        }

        public double DesiredWidth
        {
            set
            {
                SetValue(DesiredWidthProperty, value);
            }
            get { return (double)GetValue(DesiredWidthProperty); }
        }
    }
}
