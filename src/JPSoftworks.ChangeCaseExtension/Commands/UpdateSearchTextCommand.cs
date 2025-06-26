// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using JPSoftworks.ChangeCaseExtension.Pages;

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class UpdateSearchTextCommand : InvokableCommand
{
    private readonly string _searchText;
    private readonly ChangeCaseExtensionPage _target;

    public UpdateSearchTextCommand(string searchText, ChangeCaseExtensionPage target)
    {
        ArgumentNullException.ThrowIfNull(target);

        this._searchText = searchText;
        this._target = target;
    }

    public override ICommandResult Invoke(object? sender)
    {
        // if searchText is multiline, then copy to clipboard and set page search text to empty
        if (this._searchText.IndexOfAny(['\n', '\r']) > -1)
        {
            ClipboardHelper.SetText(this._searchText);
            this._target.SearchText = string.Empty;
            this._target.ForceUpdateSearch();

            return CommandResult.ShowToast(new ToastArgs()
            {
                Message = "Text copied to clipboard", Result = CommandResult.KeepOpen()
            });
        }
        else
        {
            this._target.SearchText = this._searchText;
            return CommandResult.KeepOpen();
        }
    }
}