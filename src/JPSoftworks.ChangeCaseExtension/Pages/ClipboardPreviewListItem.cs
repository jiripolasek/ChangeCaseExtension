// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class ClipboardPreviewListItem : ListItem
{
    private readonly CopyTextCommand _command;

    public ClipboardPreviewListItem()
    {
        this._command = new CopyTextCommand("");
        this.Command = this._command;
        this.Subtitle = Strings.Command_ClipboardPreview_Title!;
        this.Icon = Icons.View;
        this.Update("");
    }

    public void Update(string clipboardText)
    {
        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            this.Title = Strings.Command_ClipboardPreview_Title!;
            this.Details = new Details
            {
                Title = Strings.Command_ClipboardPreview_Title!,
                Body = Strings.Command_ClipboardPreview_EmptyClipboard!
            };
            this._command.Text = "";
        }
        else
        {
            this.Title = TransformationListItemBase.ToPreview(clipboardText.ToLines());
            this.Details = new Details
            {
                Title = Strings.Command_ClipboardPreview_Title!,
                Body = TransformationListItemBase.BuildDetailPreview(clipboardText)
            };
            this._command.Text = clipboardText;
        }
    }
}