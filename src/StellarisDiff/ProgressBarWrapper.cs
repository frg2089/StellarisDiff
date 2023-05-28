using System.Diagnostics;

using ShellProgressBar;

namespace StellarisDiff;

public sealed class ProgressBarWrapper : IDisposable
{
    readonly ChildProgressBar _progressBar;
    readonly Stopwatch _stopwatch = new();
    readonly DateTime _dateTime = DateTime.Now;

    public ProgressBarWrapper(ChildProgressBar progressBar)
    {
        _progressBar = progressBar;
        _stopwatch.Start();
    }

    public void Dispose()
    {
        _progressBar.Dispose();
    }

    public void Report(int value, string? message = null)
    {
        if (_stopwatch.ElapsedTicks is > TimeSpan.TicksPerSecond)
        {
            _progressBar.Tick(
                value,
                TimeSpan.FromTicks(DateTime.Now.Subtract(_dateTime).Ticks * (_progressBar.MaxTicks - value) / value),
                message
            );
        }
        else
        {
            _progressBar.Tick(
                value,
                message
            );
        }
    }

    public void Finished()
    {
        _progressBar.Tick(_progressBar.MaxTicks);
        _stopwatch.Stop();
    }
}