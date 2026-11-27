namespace TaskDoc.WinFormsApp;

/// <summary>
/// Colors and small styling behaviors used from code. The static look of each view (colors, fonts, sizes) lives in
/// its .Designer.cs so it can be edited in the Visual Studio designer; keep the colors there in step with these.
/// Light theme only: .NET 8 Windows Forms has no dark mode.
/// </summary>
public static class Ui
{
    public static readonly Color Surface = Color.White;

    /// <summary>Background of the selected sidebar item.</summary>
    public static readonly Color Selected = Color.FromArgb(225, 236, 248);

    private static readonly Color DisabledFill = Color.FromArgb(230, 230, 230);

    /// <summary>
    /// Flat buttons keep their colors when disabled; fade them so the state is visible. Call after
    /// InitializeComponent: the colors set in the designer are the enabled look.
    /// </summary>
    public static void FadeWhenDisabled(params Button[] buttons)
    {
        foreach (var button in buttons)
        {
            var back = button.BackColor;
            var border = button.FlatAppearance.BorderColor;
            void Apply()
            {
                button.BackColor = button.Enabled ? back : DisabledFill;
                button.FlatAppearance.BorderColor = button.Enabled ? border : DisabledFill;
            }

            button.EnabledChanged += (_, _) => Apply();
            Apply();
        }
    }
}
