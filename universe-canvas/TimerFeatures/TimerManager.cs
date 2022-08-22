#nullable enable
using System;
using System.Threading;

namespace universe_canvas.TimerFeatures
{
    public class TimerManager
    {
        private Timer? _timer;
        private AutoResetEvent? _autoResetEvent;
        public bool IsTimerStarted { get; set; }
        
        public void PrepareTimer(int period, Action action)
        {
            if (IsTimerStarted)
            {
                StopTimer();
            }
            
            _autoResetEvent = new AutoResetEvent(false);
            _timer = new Timer((_) => action.Invoke(), _autoResetEvent, period, period);
            IsTimerStarted = true;
        }

        public void StopTimer()
        {
            IsTimerStarted = false;
            _timer?.Dispose();
        }
    }
}