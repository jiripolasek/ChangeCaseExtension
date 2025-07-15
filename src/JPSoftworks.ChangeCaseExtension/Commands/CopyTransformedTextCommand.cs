// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class CopyTransformedTextCommand : InvokableCommand
{
    private readonly HistoryManager _historyManager;
    private readonly IDynamicListPage _parentList;
    private readonly TransformationType _transformationType;

    public string Text { get; set; }

    public CommandResult Result { get; set; }

    public CopyTransformedTextCommand(IDynamicListPage parentList, string text, TransformationType transformationType, HistoryManager historyManager, bool keepOpen = false)
    {
        ArgumentNullException.ThrowIfNull(historyManager);

        this._parentList = parentList;
        this._transformationType = transformationType;
        this._historyManager = historyManager;

        this.Text = text ?? "";
        this.Name = "Copy";
        this.Icon = Icons.Copy;
        this.Result = CommandResult.ShowToast(new ToastArgs
        {
            Message = Strings.CommandResult_CopiedToClipboard!,
            Result = keepOpen ? CommandResult.KeepOpen() : CommandResult.Hide()
        });
    }

    public override ICommandResult Invoke()
    {
        this._historyManager.RememberTransformation(this._transformationType);
        ClipboardHelper.SetText(this.Text);
        this._parentList.SearchText = "";
        return this.Result;
    }
}