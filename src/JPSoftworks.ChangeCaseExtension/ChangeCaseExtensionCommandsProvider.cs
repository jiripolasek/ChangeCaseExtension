// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension;

public sealed partial class ChangeCaseExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    private readonly SettingsManager _settingsManager = new();

    public ChangeCaseExtensionCommandsProvider()
    {
        this.DisplayName = Strings.Page_ChangeCase_Title!;
        this.Icon = Icons.ChangeCaseIcon;
        this.Settings = this._settingsManager.Settings;

        this._commands =
        [
            new CommandItem(new ChangeCaseExtensionPage(this._settingsManager))
            {
                Title = this.DisplayName,
                Subtitle = Strings.Page_ChangeCase_Subtitle!,
                MoreCommands = [
                    new CommandContextItem(_settingsManager.Settings.SettingsPage)
                    ]
            }
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return this._commands;
    }
}