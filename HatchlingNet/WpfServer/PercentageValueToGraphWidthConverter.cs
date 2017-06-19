using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfServer
{
    public class PercentageValueToGraphWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            const int index_realtimeValue = 0;
            const int index_MaxWidthOfGraph = 1;
            
            float percentageValue = (float)values[index_realtimeValue] / 100;

            if(percentageValue < 0 || percentageValue > 1)
                throw new ArgumentException("Getted percentageValue was " + percentageValue + ".");

            double maxWidthOfGraph = (double)values[index_MaxWidthOfGraph];

            double resultGraphWidth = maxWidthOfGraph * percentageValue;
            return resultGraphWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
