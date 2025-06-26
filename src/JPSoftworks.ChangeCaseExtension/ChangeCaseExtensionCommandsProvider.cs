// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.ChangeCaseExtension.Helpers;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.ChangeCaseExtension;

public sealed partial class ChangeCaseExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public ChangeCaseExtensionCommandsProvider()
    {
        this.DisplayName = "Change Case";
        this.Icon = Icons.ChangeCaseIcon;
        this._commands =
        [
            new CommandItem(new Pages.ChangeCaseExtensionPage())
            {
                Title = this.DisplayName, Subtitle = "Change the case of text",
            },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return this._commands;
    }
}