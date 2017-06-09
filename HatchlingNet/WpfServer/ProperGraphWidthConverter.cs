using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfServer
{
    public class ProperGraphWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            const int index_WidthOfParent = 0;
            const int index_PaddingOfParent = 1;
            const int index_MarginOfParent = 2;
            const int index_ThicknessOfParent = 3;
            const int index_MarginOfSelf = 4;

            const int correctionDecreasingValue = 16;

            double widthOfVisualParent = (double)value[index_WidthOfParent];
            Thickness paddingOfVisualParent = (Thickness)value[index_PaddingOfParent];
            Thickness marginOfVisualParent = (Thickness)value[index_MarginOfParent];
            Thickness thicknessOfVisualParent = (Thickness)value[index_ThicknessOfParent];
            Thickness marginOfSelf = (Thickness)value[index_MarginOfSelf];

            double properGraphWidth = widthOfVisualParent
                - (paddingOfVisualParent.Left + paddingOfVisualParent.Right)
                - (marginOfVisualParent.Left + marginOfVisualParent.Right)
                - (thicknessOfVisualParent.Left + thicknessOfVisualParent.Right)
                - (marginOfSelf.Left + marginOfSelf.Right)
                - correctionDecreasingValue;

            return properGraphWidth;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
