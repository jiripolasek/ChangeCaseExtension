// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class UpdateSearchTextCommand : InvokableCommand
{
    private static readonly char[] NewLineChars = ['\n', '\r'];
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
        if (this._searchText.IndexOfAny(NewLineChars) > -1)
        {
            ClipboardHelper.SetText(this._searchText);
            this._target.SearchText = string.Empty;
            this._target.ForceUpdateSearch();

            return CommandResult.ShowToast(new ToastArgs
            {
                Message = Strings.CommandResult_CopiedToClipboard!, Result = CommandResult.KeepOpen()
            });
        }
        else
        {
            this._target.SearchText = this._searchText;
            return CommandResult.KeepOpen();
        }
    }
}