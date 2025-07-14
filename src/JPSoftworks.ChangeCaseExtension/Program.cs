// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.CommandPalette.Extensions.Toolkit.Helpers;
using Shmuelie.WinRTServer.CsWinRT;

namespace JPSoftworks.ChangeCaseExtension;

public static class Program
{
    [MTAThread]
    public static async Task Main(string[] args)
    {
        Logger.Initialize("JPSoftworks", "ChangeCaseExtension");

        if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
        {
            ManualResetEvent appLifeMonitorTerminationEvent = new(false);
            
            TrySetAppLifeMonitor(appLifeMonitorTerminationEvent);

            TrySetShutdownPriority();

            Shmuelie.WinRTServer.ComServer server = new();

            ManualResetEvent extensionDisposedEvent = new(false);

            ChangeCaseExtension extensionInstance = new(extensionDisposedEvent);
            server.RegisterClass<ChangeCaseExtension, IExtension>(() => extensionInstance);
            server.Start();

            WaitHandle.WaitAny([extensionDisposedEvent, appLifeMonitorTerminationEvent]);
            server.UnsafeDispose();
        }
        else
        {
            await StartupHelper.HandleDirectLaunchAsync();
        }
    }



    private static void TrySetShutdownPriority()
    {
        // Set a lower priority, so system shutdown's other apps first (it goes from high dwLevel to low). Particularly we want
        // to allow host CmdPal to shut down before us. This has two effects:
        //    1. It allows CmdPal to release us, and we then shut down naturally.
        //    2. System won't shut us down before CmdPal, so if user cancels shutdown and CmdPal is still running, we are too.
        NativeMethods.SetProcessShutdownParameters(0x200, NativeMethods.SHUTDOWN_NORETRY);
    }



    private static AppLifeMonitor? TrySetAppLifeMonitor(ManualResetEvent appLifeMonitorTerminationEvent)
    {
        try
        {
            AppLifeMonitor? appLifeMonitor = new();
            appLifeMonitor.ExitRequested += (_, _) => { appLifeMonitorTerminationEvent.Set(); };
            appLifeMonitor.StartMonitoring();
            return appLifeMonitor;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            return null;
        }
    }
}