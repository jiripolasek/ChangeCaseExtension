// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension;

public sealed partial class ChangeCaseExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public ChangeCaseExtensionCommandsProvider()
    {
        this.DisplayName = Strings.Page_ChangeCase_Title!;
        this.Icon = Icons.ChangeCaseIcon;
        this._commands =
        [
            new CommandItem(new ChangeCaseExtensionPage())
            {
                Title = this.DisplayName, 
                Subtitle = Strings.Page_ChangeCase_Subtitle!,
            },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return this._commands;
    }
}