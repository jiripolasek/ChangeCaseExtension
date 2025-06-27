// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System;
using JPSoftworks.ChangeCaseExtension.Helpers;
using JPSoftworks.ChangeCaseExtension.Helpers.Transformers;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed class CopyTransformedTextCommand : InvokableCommand
{
    private readonly HistoryManager _historyManager;
    private readonly TransformationType _transformationType;
    public string Text { get; set; }

    public CommandResult Result { get; set; } = CommandResult.ShowToast("Copied to clipboard");

    public CopyTransformedTextCommand(string text, TransformationType transformationType, HistoryManager historyManager)
    {
        ArgumentNullException.ThrowIfNull(historyManager);

        this._transformationType = transformationType;
        this._historyManager = historyManager;

        this.Text = text ?? "";
        this.Name = "Copy";
        this.Icon = new IconInfo("\uE8C8");
    }

    public override ICommandResult Invoke()
    {
        this._historyManager.RememberTransformation(this._transformationType);
        ClipboardHelper.SetText(this.Text);
        return (ICommandResult)this.Result;
    }
}