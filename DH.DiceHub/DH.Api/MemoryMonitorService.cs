using System.Diagnostics;

public class MemoryMonitorService : BackgroundService
{
    private readonly ILogger<MemoryMonitorService> _logger;

    public MemoryMonitorService(ILogger<MemoryMonitorService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var process = Process.GetCurrentProcess();
            var memoryUsedMb = process.WorkingSet64 / (1024.0 * 1024.0);

            var gcInfo = GC.GetGCMemoryInfo();
            var totalAvailableMb = gcInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0);

            // If GC doesn’t know, totalAvailableMb can be 0 on some platforms
            if (totalAvailableMb <= 0)
            {
                totalAvailableMb = GetSystemTotalMemoryMb();
            }

            var usagePercent = (memoryUsedMb / totalAvailableMb) * 100;

            _logger.LogInformation(
                "Memory usage: {Used:F2} MB / {Total:F2} MB ({Percent:F1}%)",
                memoryUsedMb, totalAvailableMb, usagePercent);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private static double GetSystemTotalMemoryMb()
    {
        // Works on Windows & Linux (cross-platform)
        return GC.GetGCMemoryInfo().TotalAvailableMemoryBytes > 0
            ? GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0)
            : 8192; // fallback if not available, e.g. assume 8GB
    }
}
