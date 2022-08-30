#nullable enable
using System;

namespace universe_canvas.Services;

public interface ITimerService
{
    Guid AddTimer(int period, Action action);

    void StopTimer(Guid id);

    void StopAllTimers();
}