namespace BankingSystemMVC.Areas.Admin.Models.ViewModels.Dashboard
{
    public class SystemStatusViewModel
    {
        public ApiStatusViewModel Api { get; set; } = new();
        public List<NewRelicAppStatus> Applications { get; set; } = new();
        public bool NewRelicConfigured { get; set; }
        public string? NewRelicError { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ApiStatusViewModel
    {
        public TimeSpan Uptime { get; set; }
        public double MemoryUsageMB { get; set; }
    }

    public class NewRelicAppStatus
    {
        public string AppName { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = "gray";
        public bool Reporting { get; set; }
    }
}
