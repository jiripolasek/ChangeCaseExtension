// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class PasteCommand : InvokableCommand
{
    private readonly HistoryManager _historyManager;
    private readonly IDynamicListPage _parentList;
    private readonly TransformationType _transformationType;

    public string Text { get; set; }

    public PasteCommand(IDynamicListPage parentList, string text, TransformationType transformationType, HistoryManager historyManager)
    {
        ArgumentNullException.ThrowIfNull(historyManager);

        this._parentList = parentList;
        this._transformationType = transformationType;
        this._historyManager = historyManager;

        this.Text = text ?? "";
        this.Name = Strings.Command_Paste_Title!;
        this.Icon = Icons.Paste;
    }

    public override ICommandResult Invoke()
    {
        this._historyManager.RememberTransformation(this._transformationType);
        ClipboardHelper.SetText(this.Text);
        this._parentList.SearchText = "";

        // Get the current application window; at this moment I assume that it is the CmdPal host window.
        // I'll remember this and wait for the another window to be activated.
        var maybeCmdPalWindow = NativeMethods.GetForegroundWindow();

        _ = Task.Run(async () =>
        {
            try
            {
                // wait for the foreground window to change to something else, and ideally something that can handle text input IsViableWindowForPasting
                var timeout = DateTime.UtcNow.AddSeconds(5);
                var newWindowHandle = NativeMethods.GetForegroundWindow();
                while (DateTime.UtcNow < timeout)
                {
                    newWindowHandle = NativeMethods.GetForegroundWindow();
                    if (newWindowHandle != maybeCmdPalWindow && IsViableWindowForPasting(newWindowHandle))
                    {
                        // Found a viable window, break the loop
                        break;
                    }
                    await Task.Delay(100);
                }

                if (newWindowHandle != maybeCmdPalWindow && IsViableWindowForPasting(newWindowHandle))
                {
                    // wait a bit more to ensure that the new window is ready for input
                    // TODO: replace with send message
                    await Task.Delay(100);
                    ClipboardHelperEx.SendPasteKeyCombination();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        });

        return CommandResult.ShowToast(new ToastArgs
        {
            Message = Strings.CommandResult_Pasting,
            Result = CommandResult.Hide()
        });
    }

    private static bool IsViableWindowForPasting(nint windowHandle)
    {
        var desktopHandle = NativeMethods.GetDesktopWindow();
        var shellHandle = NativeMethods.GetShellWindow();

        // Check if the window is a viable target for pasting text.
        // This could be a more complex check based on the application type, but for now, we assume
        // that any window that can receive text input is viable.
        return windowHandle != nint.Zero
               && windowHandle != desktopHandle
               && windowHandle != shellHandle
               && NativeMethods.IsWindow(windowHandle)
               && NativeMethods.IsWindowVisible(windowHandle)
               && NativeMethods.IsWindowEnabled(windowHandle);
    }
}