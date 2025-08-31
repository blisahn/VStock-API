namespace VBorsa_API.Core.Entities.Identity.Helpers;

public static class Permissions
{
    public const string ViewProfile = "Profile.View";
    public const string EditProfile = "Profile.Edit";

    public const string UsersView = "Users.View";
    public const string UsersManage = "Users.Manage";

    public const string SettingsView = "Settings.View";
    public const string SettingsManage = "Settings.Manage";

    public static IReadOnlyList<string> All => new[]
    {
        ViewProfile, EditProfile,
        UsersView, UsersManage,
        SettingsView, SettingsManage
    };
}