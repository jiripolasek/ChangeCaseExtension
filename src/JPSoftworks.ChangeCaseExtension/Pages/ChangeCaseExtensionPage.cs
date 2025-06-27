// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Diagnostics;
using JPSoftworks.ChangeCaseExtension.Commands;
using JPSoftworks.ChangeCaseExtension.Helpers;
using JPSoftworks.ChangeCaseExtension.Helpers.Transformers;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class ChangeCaseExtensionPage : AsyncDynamicListPage
{
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

        this._historyManager.HistoryChanged += OnHistoryManagerOnHistoryChanged;
    }

    private void OnHistoryManagerOnHistoryChanged(object? sender, TransformationType transformationType)
    {
        // force update search when history changes
        this.ForceUpdateSearch();
    }

    private void ClipboardMonitorOnClipboardChanged(object? sender, EventArgs e)
    {
        if (ClipboardHelper.GetText().Trim() != this._lastClipboardText)
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
#if MEASURE_PERF
        Stopwatch sw = Stopwatch.StartNew();
#endif

        string textToTransform = string.IsNullOrWhiteSpace(searchText) ? ClipboardHelper.GetText() : searchText;

        var result = new List<IListItem>();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            this._lastClipboardText = ClipboardHelper.GetText().Trim();

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
        if (detectAndExtractWords is { HasSpecialSeparators: true, HasInnerWhitespace: false })
        {
            var recomposedLines = detectAndExtractWords.Lines
                .Select(static line => string.Join(" ", line.Words))
                .ToArray();
            var newString = string.Join(Environment.NewLine, recomposedLines).Trim();
            result.Add(new ListItem(new UpdateSearchTextCommand(newString, this))
            {
                Title = "Split input using special characters",
                Subtitle = "Non-space word separator detected; decode the string into separate words",
                Icon = Icons.Replace,
                Details = new Details
                {
                    Title = "Replace with detected words", Body = BuildDetailPreview(newString)
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
            var definition = TransformationRegistry.GetTransformations()
                .FirstOrDefault(d => d.Type == recentTransformationType);
            if (definition != null)
            {
                var item = this.CreateTransformationItem(definition, linesToTransform, " • recently used");
                item.Icon = Icons.History;
                result.Add(item);
            }
        }

        // case transformations

        var batchTransformResults = BatchTransformer.TransformAll(textToTransform);
        foreach (var transformationDefinition in batchTransformResults.Keys)
        {
            result.Add(this.CreateTransformationItem(transformationDefinition,
                batchTransformResults[transformationDefinition], tag: transformationDefinition.Category.DisplayName));
        }

#if MEASURE_PERF
        sw.Stop();
        result.Add(new ListItem(new NoOpCommand()) { Title = "Total update time: " + sw.Elapsed });
#endif

        return Task.FromResult<IListItem[]>([.. result]);
    }

    // treat the text as raw text; format it as code in markdown and escape backticks
    private static string BuildDetailPreview(string? textToTransform) =>
        $"""
         ```
         {(textToTransform ?? "").Replace("`", "``")}
         ```
         """;

    private ListItem CreateTransformationItem(
        TransformationDefinition definition,
        string[] lines,
        string? extraSubject = null,
        string? tag = null)
    {
        var title = definition.Title ?? "";
        var transformed = string.Join(Environment.NewLine, lines);
        var preview = ToPreview(lines);

        var r = new ListItem(new CopyTransformedTextCommand(transformed, definition.Type, this._historyManager))
        {
            Title = preview,
            Subtitle = title + (extraSubject ?? ""),
            Details = new Details { Title = title, Body = BuildDetailPreview(transformed) },
            Tags = string.IsNullOrWhiteSpace(tag) ? [] : [new Tag(tag)]
        };

        if (definition.Category == TransformationCategory.Technical)
        {
            r.Icon = Icons.Keyboard;
        }
        else if (definition.Category == TransformationCategory.Text)
        {
            r.Icon = Icons.FontSansSerif;
        }
        else if (definition.Category == TransformationCategory.Separators)
        {
            r.Icon = Icons.Space;
        }

        return r;
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
        if (disposing)
        {
            this._historyManager.HistoryChanged -= this.OnHistoryManagerOnHistoryChanged;
            this._clipboardMonitor.ClipboardChanged -= this.ClipboardMonitorOnClipboardChanged;
        }

        base.Dispose(disposing);
    }
}