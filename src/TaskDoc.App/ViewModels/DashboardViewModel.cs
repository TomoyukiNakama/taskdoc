using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskDoc.App.Services;

namespace TaskDoc.App.ViewModels;

/// <summary>Home section. A placeholder until the task and document screens exist.</summary>
public partial class DashboardViewModel(ITaskDocApi api) : ViewModelBase
{
    [ObservableProperty]
    public partial string ConnectionStatus { get; set; } = "未確認";

    [RelayCommand]
    private async Task CheckConnectionAsync(CancellationToken ct)
    {
        ConnectionStatus = "確認中…";
        try
        {
            var health = await api.GetHealthAsync(ct);
            var me = await api.GetMeAsync(ct);
            ConnectionStatus = $"接続OK（SQLite {health.DatabaseVersion} / 認証済み: {me.UserName}）";
        }
        catch (Exception ex) when (ApiErrors.ToMessage(ex) is { } message)
        {
            ConnectionStatus = message;
        }
    }
}
