// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers;


public sealed class SettingsManager : JsonSettingsManager
{
    private const string DefaultNamespace = "jpsoftworks.changecaseextension";

    private readonly ChoiceSetSetting _specialItemsOrder = new(
        Namespaced(nameof(SpecialItemsOrder)),
        Strings.Settings_SpecialItemsOrder_Label!,
        Strings.Settings_SpecialItemsOrder_Label!,
        [
            // first option is the default one (hardcoded in the extension SDK)
            new ChoiceSetSetting.Choice(Strings.Settings_SpecialItemsOrder_Choice_RecentFirst, SpecialItemsOrder.RecentFirst.ToString("G")),
            new ChoiceSetSetting.Choice(Strings.Settings_SpecialItemsOrder_Choice_PinnedFirst, SpecialItemsOrder.PinnedFirst.ToString("G"))
        ]);

    private readonly ChoiceSetSetting _recentItemsCount = new(
        Namespaced(nameof(RecentItemsCount)),
        Strings.Settings_RecentItemsCount_Label!,
        Strings.Settings_RecentItemsCount_Label!,
        [
            // first option is the default one (hardcoded in the extension SDK)
            new ChoiceSetSetting.Choice(Strings.Settings_RecentItemsCount_Choice_None, "0"),
            new ChoiceSetSetting.Choice(Strings.Settings_RecentItemsCount_Choice_1, "1"),
            new ChoiceSetSetting.Choice(Strings.Settings_RecentItemsCount_Choice_2, "2"),
            new ChoiceSetSetting.Choice(Strings.Settings_RecentItemsCount_Choice_3, "3"),
            new ChoiceSetSetting.Choice("5", "5")
        ])
    { Value = "3" };


    public SpecialItemsOrder SpecialItemsOrder => Enum.TryParse<SpecialItemsOrder>(this._specialItemsOrder.Value, out var savedValue) ? savedValue : SpecialItemsOrder.RecentFirst;

    public int RecentItemsCount => int.TryParse(this._recentItemsCount.Value, out var count) ? count : 3;

    public SettingsManager()
    {
        this.FilePath = SettingsJsonPath();
        this.Settings.Add(this._specialItemsOrder);
        this.Settings.Add(this._recentItemsCount);
        this.LoadSettings();
        this.Settings.SettingsChanged += (_, _) => this.SaveSettings();
    }

    private static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "settings.json");
    }

    private static string Namespaced(string propertyName)
    {
        return $"{DefaultNamespace}.{propertyName}";
    }
}

public enum SpecialItemsOrder
{
    RecentFirst = 0,
    PinnedFirst = 1
}