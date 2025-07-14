// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class ChangeCaseExtensionPage : AsyncDynamicListPage
{
    private readonly ClipboardMonitor _clipboardMonitor;
    private readonly ClipboardPreviewListItem _clipboardPreviewItem;
    private readonly HistoryManager _historyManager = new();

    private bool _disposed;
    private readonly PinnedTransformationsManager _pinnedTransformationsManager = PinnedTransformationsManager.Instance;



    public ChangeCaseExtensionPage()
    {
        this.Icon = Icons.ChangeCaseIcon;
        this.Title = Strings.Page_ChangeCase_Title!;
        this.Name = Strings.Page_ChangeCase_Name!;
        this.ShowDetails = true;
        this.PlaceholderText = Strings.Page_ChangeCase_Placeholder!;
        this.Id = "com.jpsoftworks.cmdpal.changecase.changecase";

        this._clipboardMonitor = new ClipboardMonitor();
        this._clipboardMonitor.ClipboardChanged += this.ClipboardMonitorOnClipboardChanged;
        this._clipboardMonitor.StartMonitoring();

        this._historyManager.HistoryChanged += this.OnHistoryManagerOnHistoryChanged;

        this._clipboardPreviewItem = new ClipboardPreviewListItem();

        this.EmptyContent = new ListItem
        {
            Title = Strings.Page_ChangeCase_EmptyContent_Title!,
            Subtitle = Strings.Page_ChangeCase_EmptyContent_Subtitle!,
            Icon = Icons.ChangeCaseIcon
        };

        this._pinnedTransformationsManager.PinnedChanged += this.OnInstanceOnPinnedChanged;
    }

    private void OnInstanceOnPinnedChanged(object? o, TransformationType transformationType)
    {
        this.ForceUpdateSearch();
    }

    private void OnHistoryManagerOnHistoryChanged(object? sender, TransformationType transformationType)
    {
        this.ForceUpdateSearch();
    }

    private void ClipboardMonitorOnClipboardChanged(object? sender, EventArgs e)
    {
        this.ForceUpdateSearch();
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
        string textToTransform;
        string? clipboardText;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            clipboardText = ClipboardHelper.GetText().Trim();
            textToTransform = clipboardText;
        }
        else
        {
            clipboardText = null;
            textToTransform = searchText;
        }

        if (string.IsNullOrWhiteSpace(textToTransform))
        {
            return Task.FromResult<IListItem[]>([]);
        }

        var result = new List<IListItem>();

        // preview
        if (!string.IsNullOrWhiteSpace(clipboardText))
        {
            this._clipboardPreviewItem.Update(clipboardText);
            result.Add(this._clipboardPreviewItem);
        }

        cancellationToken.ThrowIfCancellationRequested();

        // quick action for a text delimited by non-space characters
        var detectAndExtractWords = StringCaseDetector.DetectAndExtractWordsMultiLine(textToTransform);
        if (detectAndExtractWords is { HasSpecialSeparators: true, HasInnerWhitespace: false })
        {
            var recomposedLines = detectAndExtractWords.Lines
                .Select(static line => string.Join(" ", line.Words ?? []))
                .ToArray();
            var newString = string.Join(Environment.NewLine, recomposedLines).Trim();
            result.Add(new ListItem(new UpdateSearchTextCommand(newString, this))
            {
                Title = Strings.Command_SplitSpecial_Title!,
                Subtitle = Strings.Command_SplitSpecial_Subtitle!,
                Icon = Icons.Replace,
                Details = new Details
                {
                    Title = Strings.Command_SplitSpecial_Detail_Title!,
                    Body = TransformationListItemBase.BuildDetailPreview(newString)
                }
            });
        }

        cancellationToken.ThrowIfCancellationRequested();

        var batchTransformResults = BatchTransformer.TransformAll(textToTransform);

        // recent transformation items
        var localRecentItems = this._historyManager.History
            .Select(recentTransformationType =>
                batchTransformResults.FirstOrDefault(t => t.Key.Type == recentTransformationType))
            .Select(pair => new RecentTransformationListItem(this, pair.Key, pair.Value, this._historyManager, this._pinnedTransformationsManager))
            .Take(3);
        result.AddRange(localRecentItems);

        // favorite transformation items
        var pinnedTransformation = this._pinnedTransformationsManager.Pinned.Except(this._historyManager.History)
            .Select(favoriteTransformationType =>
                batchTransformResults.FirstOrDefault(t => t.Key.Type == favoriteTransformationType))
            .Select(pair => new PinnedTransformationListItem(this, pair.Key, pair.Value, this._historyManager, this._pinnedTransformationsManager));
        result.AddRange(pinnedTransformation);

        cancellationToken.ThrowIfCancellationRequested();

        // all transformation items
        var tis = TransformationRegistry.GetTransformations()
            .Select(transformationDefinition => new TransformationListItem(
                this,
                transformationDefinition,
                batchTransformResults[transformationDefinition],
                this._historyManager,
                this._pinnedTransformationsManager));
        result.AddRange(tis);

#if MEASURE_PERF
        sw.Stop();
        result.Add(new ListItem(new NoOpCommand()) { Title = "Total update time: " + sw.Elapsed });
#endif

        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<IListItem[]>([.. result]);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !this._disposed)
        {
            this._disposed = true;
            this._historyManager.HistoryChanged -= this.OnHistoryManagerOnHistoryChanged;
            this._clipboardMonitor.ClipboardChanged -= this.ClipboardMonitorOnClipboardChanged;
            this._clipboardMonitor.Dispose();

            this._pinnedTransformationsManager.PinnedChanged -= this.OnInstanceOnPinnedChanged;
        }

        base.Dispose(disposing);
    }
}