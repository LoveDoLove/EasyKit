namespace CommonUtilities.Helpers.Scheduler;

public interface IScheduleConfig<T>
{
    string CronExpression { get; set; }
    TimeZoneInfo TimeZoneInfo { get; set; }
}

public class ScheduleConfig<T> : IScheduleConfig<T>
{
    public string CronExpression { get; set; } = string.Empty;
    public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
}