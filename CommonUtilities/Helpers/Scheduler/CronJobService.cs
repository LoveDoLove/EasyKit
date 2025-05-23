using Cronos;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace CommonUtilities.Helpers.Scheduler;

public abstract class CronJobService : IHostedService, IDisposable
{
    private readonly CronExpression _expression;
    private readonly TimeZoneInfo _timeZoneInfo;
    private Timer _timer;

    protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo)
    {
        _expression = CronExpression.Parse(cronExpression);
        _timeZoneInfo = timeZoneInfo;
    }

    public virtual void Dispose()
    {
        _timer?.Dispose();
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await ScheduleJob(cancellationToken);
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        await Task.CompletedTask;
    }

    protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
        if (next.HasValue)
        {
            var delay = next.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds <= 0) // prevent non-positive values from being passed into Timer
                await ScheduleJob(cancellationToken);
            _timer = new Timer(delay.TotalMilliseconds);
            _timer.Elapsed += async (sender, args) =>
            {
                _timer.Dispose(); // reset and dispose timer
                _timer = null;
                if (!cancellationToken.IsCancellationRequested) await DoWork(cancellationToken);
                if (!cancellationToken.IsCancellationRequested) await ScheduleJob(cancellationToken); // reschedule next
            };
            _timer.Start();
        }

        await Task.CompletedTask;
    }

    public virtual async Task DoWork(CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken); // do the work
    }
}