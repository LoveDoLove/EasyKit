using CommonUtilities.Helpers.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommonUtilities.Services;

/*
 * @author: LoveDoLove
 * @description:
 * The SyncService class is used to handle all the sync related services.
 */
public class SyncService : CronJobService
{
    private readonly ILogger<SyncService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SyncService(IScheduleConfig<SyncService> config, ILogger<SyncService> logger,
        IServiceScopeFactory scopeFactory)
        : base(config.CronExpression, config.TimeZoneInfo)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(SyncService)} starts.");
        return base.StartAsync(cancellationToken);
    }

    public override async Task<Task> DoWork(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{DateTime.Now:hh:mm:ss} {nameof(SyncService)} is working.");

        using IServiceScope scope = _scopeFactory.CreateScope();
        //IResetPasswordService resetPasswordService = scope.ServiceProvider.GetRequiredService<IResetPasswordService>();

        _logger.LogInformation("Syncing status");
        //await resetPasswordService.SyncExpiredResetPasswordAsync();

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(SyncService)} is stopping.");
        return base.StopAsync(cancellationToken);
    }
}