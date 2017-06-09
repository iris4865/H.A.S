using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Management
{
    /// <summary>
    /// 추적 및 디버그 출력을 모니터링하는 수신기의 abstract 기본 클래스를 제공합니다.
    /// </summary>
    public class LogTracedListener : TraceListener, INotifyPropertyChanged
    {
        string logString;

        public string Data { get => logString; }

        public override void Write(string message)
        {
            logString += message;
            OnPropertyChanged(new PropertyChangedEventArgs("Data"));
        }

        public override void WriteLine(string message)
        {
            logString += message + Environment.NewLine;
            OnPropertyChanged(new PropertyChangedEventArgs("Data"));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}