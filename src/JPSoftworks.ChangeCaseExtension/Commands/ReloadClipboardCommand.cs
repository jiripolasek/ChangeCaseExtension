// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class ReloadClipboardCommand : InvokableCommand
{
    private readonly ChangeCaseExtensionPage _changeCaseExtensionPage;

    public ReloadClipboardCommand(ChangeCaseExtensionPage changeCaseExtensionPage)
    {
        this._changeCaseExtensionPage = changeCaseExtensionPage;
        this.Name = "Refresh Clipboard";
    }

    public override ICommandResult Invoke()
    {
        this._changeCaseExtensionPage.ForceUpdateSearch();
        return CommandResult.ShowToast(new ToastArgs()
        {
            Message = "Clipboard content updated", Result = CommandResult.KeepOpen()
        });
    }
}