
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers;

using PInvoke = AppLifeMonitorNativeMethods;

/// <summary>
/// Monitors the application lifecycle events such as shutdown and end session.
/// </summary>
internal sealed class AppLifeMonitor : IDisposable
{
    public event EventHandler? ExitRequested;

    private static readonly Lazy<IntPtr> DefWindowProcAddress = new(() => PInvoke.GetProcAddress(PInvoke.GetModuleHandle("user32.dll"), "DefWindowProcW"));
    
    private readonly Lock _syncLock = new();
    private readonly ManualResetEvent _threadProcInitialized = new(false);
    
    private volatile bool _disposed;
    private nint _hwnd;
    private Thread? _messageLoopThread;
    private uint _nativeThreadId;
    private PInvoke.WindowProc? _windowProc;



    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        try
        {
            lock (this._syncLock)
            {
                if (this._disposed)
                {
                    return;
                }

                this._disposed = true;

                if (this._nativeThreadId != nint.Zero)
                {
                    PInvoke.PostThreadMessage(this._nativeThreadId, PInvoke.WM_QUIT, 0, 0);
                }

                // Wait up to 5 s for the loop to exit
                if (this._messageLoopThread?.IsAlive == true &&
                    !this._messageLoopThread.Join(TimeSpan.FromSeconds(5)))
                {
                    // Failed to stop the message loop
                }

                this._messageLoopThread = null;
                this._nativeThreadId = 0u;
                // don't clear the _windowProc here, it will be cleared in CleanupWindow
                // this._messageLoopThread.Join might have timed out and the window might still be valid
            }
        }
        finally
        {
            this._threadProcInitialized.Dispose();
        }
    }



    public void StartMonitoring()
    {
        ObjectDisposedException.ThrowIf(this._disposed, this);

        if (this._messageLoopThread != null)
        {
            return;
        }

        lock (this._syncLock)
        {
            if (this._messageLoopThread != null)
            {
                return;
            }

            // Run mini message loop to observe application lifecycle events.
            // This will allow us to exit gracefully when the system or updater asks us to.
            this._messageLoopThread = new(this.MessageLoopThread) { IsBackground = true, Name = "AppLifeMonitor Thread" };
            this._messageLoopThread.SetApartmentState(ApartmentState.STA);
            this._messageLoopThread.Start();

            // make sure the thread is initialized before we return
            // also make sure this it set even if the thread fails to initialize to unblock the caller
            this._threadProcInitialized.WaitOne();
        }
    }



    private void MessageLoopThread()
    {
        string className = $"AppLifeMonitor_{Guid.NewGuid():N}";

        try
        {
            this._nativeThreadId = PInvoke.GetCurrentThreadId();
            this.InitializeWindow(className);
            this._threadProcInitialized.Set();

            if (this._hwnd != nint.Zero)
            {
                while (true)
                {
                    var result = PInvoke.GetMessage(out PInvoke.MSG msg, this._hwnd, 0, 0);
                    if (result == nint.Zero)
                    {
                        break; // WM_QUIT received, exit the loop
                    }

                    if (result == -1)
                    {
                        break; // GetMessage failed
                    }

                    PInvoke.TranslateMessage(in msg);
                    PInvoke.DispatchMessage(in msg);
                }
            }
        }
        catch (Exception)
        {
            this._threadProcInitialized.Set();
        }

        this.SignalTermination();
        this.CleanupWindow(className);
    }



    private void InitializeWindow(string className)
    {
        this._windowProc = this.WndProc;
        var wndClass = new PInvoke.WndClass { lpfnWndProc = this._windowProc, hInstance = PInvoke.GetModuleHandle(null), lpszClassName = className };

        var classAtom = PInvoke.RegisterClass(in wndClass);
        if (classAtom == nint.Zero)
        {
            this._windowProc = null;
            return;
        }

        // don't make this message window, it wouldn't receive WM_QUERYENDSESSION and WM_ENDSESSION messages
        this._hwnd = PInvoke.CreateWindowEx(0, className, "AppLifeMonitor", PInvoke.WS_POPUP, 0, 0, 0, 0, 0, nint.Zero, wndClass.hInstance, nint.Zero);
    }



    private void CleanupWindow(string className)
    {
        if (this._hwnd == nint.Zero)
        {
            return;
        }

        var hwnd = this._hwnd;
        this._hwnd = nint.Zero;

        // prevent the window from calling our WndProc
        if (PInvoke.IsWindow(hwnd) && DefWindowProcAddress.Value != IntPtr.Zero)
        {
            PInvoke.SetWindowLongPtr(hwnd, PInvoke.GWL_WNDPROC, DefWindowProcAddress.Value);
            PInvoke.SetClassLongPtr(hwnd, PInvoke.GCL_WNDPROC, DefWindowProcAddress.Value);
        }

        // If the window is still valid, try to destroy it gracefully.
        // If that fails, post a close message to it. If that also fails, unregister the class.
        if (PInvoke.IsWindow(hwnd) && !PInvoke.DestroyWindow(hwnd))
        {
            PInvoke.PostMessage(hwnd, PInvoke.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
        else
        {
            var hInstance = PInvoke.GetModuleHandle(null);
            PInvoke.UnregisterClass(className, hInstance);
            this._windowProc = null;
        }
    }



    private nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case PInvoke.WM_CLOSE:
                if (this._hwnd != nint.Zero)
                {
                    PInvoke.DestroyWindow(this._hwnd);
                    this._hwnd = nint.Zero;
                }

                break;

            case PInvoke.WM_DESTROY:
                PInvoke.PostQuitMessage(0);
                this._hwnd = nint.Zero;
                break;

            case PInvoke.WM_QUERYENDSESSION:
                // TODO: implement callback to allow application to veto the shutdown
                break;

            case PInvoke.WM_ENDSESSION:
                if (wParam != nint.Zero)
                {
                    this.SignalTermination();
                }

                break;
        }

        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }



    private void SignalTermination()
    {
        if (this._messageLoopThread != null && !this._disposed)
        {
            this.ExitRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}