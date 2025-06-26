// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;

using JPSoftworks.ChangeCaseExtension.Commands;
using JPSoftworks.ChangeCaseExtension.Helpers;

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class ChangeCaseExtensionPage : AsyncDynamicListPage
{
    private static readonly TransformationDefinition[] TransformationDefinitions =
    [
        new(TransformationType.AsIs, "As is", static s => s),
        new(TransformationType.LowerCase, "lower case", CaseTransformers.ToLowerCase),
        new(TransformationType.UpperCase, "UPPER CASE", CaseTransformers.ToUpperCase),
        new(TransformationType.CamelCase, "camelCase", CaseTransformers.ToCamelCase),
        new(TransformationType.PascalCase, "PascalCase", CaseTransformers.ToPascalCase),
        new(TransformationType.CapitalCase, "Capital Case", CaseTransformers.ToCapitalCase),
        new(TransformationType.SnakeCase, "snake_case", CaseTransformers.ToSnakeCase),
        new(TransformationType.UpperSnakeCase, "UPPER_SNAKE_CASE", CaseTransformers.ToUpperSnakeCase),
        new(TransformationType.PascalSnakeCase, "Pascal_Snake_Case", CaseTransformers.ToPascalSnakeCase),
        new(TransformationType.ConstantCase, "CONSTANT_CASE", CaseTransformers.ToConstantCase),
        new(TransformationType.KebabCase, "kebab-case", CaseTransformers.ToKebabCase),
        new(TransformationType.KebabUpperCase, "KEBAB-UPPER-CASE", CaseTransformers.ToKebabUpperCase),
        new(TransformationType.HeaderCase, "Header-Case", CaseTransformers.ToHeaderCase),
        new(TransformationType.DotCase, "dot.case", CaseTransformers.ToDotCase),
        new(TransformationType.PathCase, "path/case", CaseTransformers.ToPathCase),
        new(TransformationType.PathBackslashCase, @"path\case\backslash", CaseTransformers.ToPathBackslashCase),
        new(TransformationType.SentenceCase, "Sentence case", CaseTransformers.ToSentenceCase),
        new(TransformationType.LowerFirst, "lower First", CaseTransformers.ToLowerFirst),
        new(TransformationType.UpperFirst, "Upper first", CaseTransformers.ToUpperFirst),
        new(TransformationType.NoCase, "no case", CaseTransformers.ToNoCase),
        new(TransformationType.SwapCase, "sWAP cASE", CaseTransformers.ToSwapCase),
        new(TransformationType.RandomCase, "rAndOm cAsE", CaseTransformers.ToRandomCase),
        new(TransformationType.TitleCase, $"Title Case ({CultureInfo.CurrentCulture.DisplayName})", CaseTransformers.ToTitleCase),
        new(TransformationType.TitleCaseInvariant, "Title Case (invariant)", CaseTransformers.ToTitleCaseInvariant),
        new(TransformationType.UpperFirstSnakeCase, "Upper first Snake Case", CaseTransformers.ToUpperFirstSnakeCase),
        new(TransformationType.UpperFirstKebabCase, "Upper first Kebab Case", CaseTransformers.ToUpperFirstKebabCase)
    ];

    private readonly ClipboardMonitor _clipboardMonitor;

    private readonly HistoryManager _historyManager = new();
    private string? _lastClipboardText;

    protected override bool IsLoadingEnabled => false;

    public ChangeCaseExtensionPage()
    {
        this.Icon = Icons.ChangeCaseIcon;
        this.Title = "Change Case";
        this.Name = "Open";
        this.ShowDetails = true;
        this.PlaceholderText = "Type text to transform, or leave empty to transform text in the Clipboard...";

        this._clipboardMonitor = new ClipboardMonitor();
        this._clipboardMonitor.ClipboardChanged += this.ClipboardMonitorOnClipboardChanged;
        this._clipboardMonitor.StartMonitoring();
    }

    private void ClipboardMonitorOnClipboardChanged(object? sender, EventArgs e)
    {
        if (ClipboardHelper.GetText() != this._lastClipboardText)
        {
            this.ForceUpdateSearch();
        }
    }

    protected override Task<IListItem[]> LoadInitialItemsAsync(CancellationToken cancellationToken)
    {
        return this.SearchItemsAsync("", "", cancellationToken);
    }

    protected override Task<IListItem[]> SearchItemsAsync(
        string previousText,
        string searchText,
        CancellationToken cancellationToken)
    {
        string textToTransform = string.IsNullOrWhiteSpace(searchText) ? ClipboardHelper.GetText() : searchText;

        var result = new List<IListItem>();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            this._lastClipboardText = ClipboardHelper.GetText();

            // TODO: verify clipboard monitor
            //result.Add(new ListItem(new ReloadClipboardCommand(this))
            //{
            //    Title = "Reload Clipboard",
            //    TextToSuggest = "Reload Clipboard",
            //    Subtitle = "Reloads the clipboard content",
            //    Icon = Icons.Refresh,
            //    Details = new Details
            //    {
            //        Title = "Clipboard Text",
            //        Body = BuildDetailPreview(textToTransform)
            //    }
            //});
        }
        else
        {
            this._lastClipboardText = null;
        }

        var detectAndExtractWords = StringCaseDetector.DetectAndExtractWordsMultiLine(textToTransform);
        if (detectAndExtractWords.HasSpecialSeparators)
        {
            var stringedLines = detectAndExtractWords.Lines.Select(static line => string.Join(" ", line.Words))
                .ToArray();
            var newString = string.Join(Environment.NewLine, stringedLines).Trim();
            result.Add(new ListItem(new UpdateSearchTextCommand(newString, this))
            {
                Title = "Split input using special characters",
                Subtitle = "Non-space word separator detected; decode the string into separate words",
                Icon = Icons.Replace,
                Details = new Details
                {
                    Title = "Replace with detected words",
                    Body = BuildDetailPreview(newString)
                }
            });
        }

        if (string.IsNullOrWhiteSpace(textToTransform))
        {
            return Task.FromResult<IListItem[]>([.. result]);
        }

        var linesToTransform = textToTransform.Trim().ToLines();

        // recent transformations
        foreach (var recentTransformationType in this._historyManager.History.Take(3))
        {
            var definition = TransformationDefinitions.FirstOrDefault(d => d.Type == recentTransformationType);
            if (definition != null)
            {
                var item = this.CreateTransformationItem(definition, linesToTransform, " • recently used");
                item.Icon = Icons.History;
                result.Add(item);
            }
        }

        // case transformations
        foreach (var transformationDefinition in TransformationDefinitions)
        {
            result.Add(this.CreateTransformationItem(transformationDefinition, linesToTransform));
        }

        return Task.FromResult<IListItem[]>([.. result]);
    }

    // treat the text as raw text; format it as code in markdown and escape backticks
    private static string BuildDetailPreview(string? textToTransform) =>
        $"""
         ```
         {(textToTransform ?? "").Replace("`", "``")}
         ```
         """;

    private ListItem CreateTransformationItem(TransformationDefinition definition, string[] lines, string? extraSubject = null)
    {
        var title = definition.GetDisplayTitle();
        var transformedLines = lines.Select(line => definition.Transform(line)).ToArray();
        var transformed = string.Join(Environment.NewLine, transformedLines);
        var preview = ToPreview(transformedLines);

        return new ListItem(new CopyTransformedTextCommand(transformed, definition.Type, this._historyManager))
        {
            Title = preview,
            Subtitle = title + (extraSubject ?? ""),
            Details = new Details { Title = title, Body = BuildDetailPreview(transformed) }
        };
    }

    private static string ToPreview(params string[]? lines)
    {
        if (lines == null || lines.Length == 0)
            return "";

        const int maxLines = 2;
        const int maxCharsPerLine = 256;
        const string ellipsis = "...";

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

    protected override void Dispose(bool disposing)
    {
        this._clipboardMonitor?.ClipboardChanged -= this.ClipboardMonitorOnClipboardChanged;
        base.Dispose(disposing);
    }
}