// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal abstract partial class TransformationListItemBase : ListItem, IEquatable<TransformationListItemBase>
{
    private readonly CopyTransformedTextCommand _command;
    private string[] _lines;

    public TransformationDefinition Definition { get; }

    protected TransformationListItemBase(
        IDynamicListPage parentList,
        TransformationDefinition definition,
        string[] lines,
        HistoryManager historyManager,
        string? tag = null,
        string? extraSubject = null)
    {
        this.Definition = definition;
        this.Command = this._command = new CopyTransformedTextCommand(parentList, "", definition.Type, historyManager);
        this.Update(lines);
        this.Subtitle = this.Definition.Title + extraSubject;
        this.Tags = string.IsNullOrWhiteSpace(tag) ? [] : [new Tag(tag)];
        this._lines = lines;

        if (definition.Category == TransformationCategory.Technical)
        {
            this.Icon = Icons.Keyboard;
        }
        else if (definition.Category == TransformationCategory.Text)
        {
            this.Icon = Icons.FontSansSerif;
        }
        else if (definition.Category == TransformationCategory.Separators)
        {
            this.Icon = Icons.Space;
        }
        else if (definition.Category == TransformationCategory.Cleanup)
        {
            this.Icon = Icons.Broom;
        }
    }

    public void Update(string[] batchTransformResult)
    {
        this._lines = batchTransformResult;
        this.Title = ToPreview(batchTransformResult);
        var transformedText = string.Join(Environment.NewLine, this._lines);
        this.Details = new Details { Title = this.Definition.Title, Body = BuildDetailPreview(transformedText) };
        this._command.Text = transformedText;
    }

    internal static string BuildDetailPreview(string? textToTransform) => MarkdownHelpers.Escape(textToTransform ?? "");

    internal static string ToPreview(params string[]? lines)
    {
        if (lines == null || lines.Length == 0)
            return "";

        const int maxLines = 2;
        const int maxCharsPerLine = 256;
        const string ellipsis = "…";

        var resultLines = new List<string>();

        for (int i = 0; i < Math.Min(lines.Length, maxLines); i++)
        {
            var line = lines[i];

            if (line.Length > maxCharsPerLine)
            {
                line = line[..maxCharsPerLine] + ellipsis;
            }

            resultLines.Add(line);
        }

        var result = string.Join(Environment.NewLine, resultLines);

        if (lines.Length > maxLines)
        {
            result += ellipsis;
        }

        return result;
    }

    public bool Equals(TransformationListItemBase? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Definition.Equals(other.Definition);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((TransformationListItemBase)obj);
    }

    public override int GetHashCode()
    {
        return this.Definition.GetHashCode();
    }

    public static bool operator ==(TransformationListItemBase? left, TransformationListItemBase? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TransformationListItemBase? left, TransformationListItemBase? right)
    {
        return !Equals(left, right);
    }
}