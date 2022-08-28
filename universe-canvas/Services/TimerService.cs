#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;

namespace universe_canvas.Services;

public class TimerService
{
    private Dictionary<Guid, Timer> _timers = new();
        
    public Guid AddTimer(int period, Action action)
    {
        var id = Guid.NewGuid();
        _timers.Add(id,
            new Timer(_ => action.Invoke(), new AutoResetEvent(false), period, period));

        return id;
    }

    public void StopTimer(Guid id)
    {
        _timers[id].Dispose();
        _timers.Remove(id);
    }

    public void StopAllTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Dispose();
        }
        _timers.Clear();
    }
}