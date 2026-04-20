using MudBlazor;

namespace PropertyManagementApp.Client.Services;

// simple service that wraps MudBlazor snackbar for toast notifications
public class NotificationService
{
    public event Action<string, Severity>? OnNotify;

    public void Success(string msg) => OnNotify?.Invoke(msg, Severity.Success);
    public void Error(string msg) => OnNotify?.Invoke(msg, Severity.Error);
    public void Info(string msg) => OnNotify?.Invoke(msg, Severity.Info);
    public void Warning(string msg) => OnNotify?.Invoke(msg, Severity.Warning);
}
