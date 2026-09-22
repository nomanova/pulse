using System;
using System.Threading.Tasks;

namespace Pulse.Web.Common.Components.Base;

public class Debouncer
{
    private System.Timers.Timer? _timer;

    public void Debounce(int interval, Func<Task> action)
    {
        _timer?.Stop();
        _timer = null;

        _timer = new System.Timers.Timer
        {
            Interval = interval,
            Enabled = false,
            AutoReset = false
        };

        _timer.Elapsed += (s, e) =>
        {
            if (_timer == null)
            {
                return;
            }

            _timer?.Stop();
            _timer = null;

            try
            {
                Task.Run(action);
            }
            catch (TaskCanceledException)
            {
                // NOP
            }
        };

        _timer.Start();
    }
}