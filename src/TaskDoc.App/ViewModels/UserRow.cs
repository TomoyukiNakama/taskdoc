using TaskDoc.Contracts.Users;

namespace TaskDoc.App.ViewModels;

/// <summary>One line of the user list, with display-ready text.</summary>
public sealed class UserRow(UserDto user)
{
    public UserDto User { get; } = user;

    public long Id => User.Id;

    public string UserName => User.UserName;

    public string DisplayName => User.DisplayName;

    public string AuthTypeLabel => User.AuthType == UserAuthType.Ad ? "AD" : "システム";

    public bool IsAdmin => User.IsAdmin;

    public bool IsActive => User.IsActive;

    public string StatusLabel => User.IsActive ? "有効" : "無効";

    /// <summary>Inactive rows are drawn faded.</summary>
    public double RowOpacity => User.IsActive ? 1.0 : 0.55;

    public string LastLoginLabel =>
        User.LastLoginAt is { } at ? at.ToLocalTime().ToString("yyyy/MM/dd HH:mm") : "—";
}
