using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfServer
{
    public class TestTimer : DispatcherTimer
    {
        public TestTimer() : base() { }
        public TestTimer(DispatcherPriority priority) : base(priority) { }
        public TestTimer(DispatcherPriority priority, Dispatcher dispatcher) : base(priority, dispatcher) { }
        public TestTimer(TimeSpan interval, DispatcherPriority priority, EventHandler callback, Dispatcher dispatcher)
            : base(interval, priority, callback, dispatcher) { }
        
        public event EventHandler BeforeStartExcutions;
        public event EventHandler AfterStopExcutions;
        
        int testCount = 0;
        int? limitCount = null;             //null : infinite

        public int? LimitTrialCount
        {
            set
            {
                if (value >= 0)
                    limitCount = value;
            }
            get { return limitCount; }
        }

        new public void Start()
        {
            BeforeStartExcutions(this, new EventArgs());
            Tick += CountEachTrial;

            base.Start();
        }

        new public void Stop()
        {
            base.Stop();

            Tick -= CountEachTrial;
            AfterStopExcutions(this, new EventArgs());
        }

        private void CountEachTrial(object sender, EventArgs e)
        {
            bool isInfiniteTrial = limitCount == null;

            if (!isInfiniteTrial && ++testCount >= limitCount)
                this.Stop();
        }
    }
}
