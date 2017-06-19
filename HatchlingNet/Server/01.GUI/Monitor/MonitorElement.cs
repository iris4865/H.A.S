using System;

namespace Management
{
    public class MonitorElement
    {
        public string Unit { get; private set; }

        readonly Func<float> currentValue;
        public float Percentage => (currentValue() / TotalValue) * 100;

        public float TotalValue { get; private set; }

        public MonitorElement(Func<float> valueMethod, float totalValue, string unit)
        {
            currentValue = valueMethod;
            TotalValue = totalValue;
            Unit = unit;
        }

        public override string ToString()
        {
            string value;
            switch(Unit)
            {
                case "%":
                    value = Percentage.ToString();
                    break;
                default:
                    value = currentValue() + " / " + TotalValue;
                    break;
            }
            value += Unit;

            return value;
        }
    }
}
