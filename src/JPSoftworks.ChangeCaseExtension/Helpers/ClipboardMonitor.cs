// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace JPSoftworks.ChangeCaseExtension.Pages;

[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal sealed partial class ClipboardMonitor : IDisposable
{
    private const uint WM_CLIPBOARDUPDATE = 0x031D;
    private const uint WM_DESTROY = 0x0002;
    private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3); // Message-only window

    public event EventHandler? ClipboardChanged;
    private bool _disposed;
    private IntPtr _hwnd;
    private Thread? _messageLoopThread;
    private WindowProc? _windowProc;

    public void Dispose()
    {
        if (!this._disposed)
        {
            this.StopMonitoring();
            this._disposed = true;
        }
    }

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddClipboardFormatListener(IntPtr hwnd);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RemoveClipboardFormatListener(IntPtr hwnd);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr CreateWindowEx(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(IntPtr hWnd);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    private static partial IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll", EntryPoint = "RegisterClassW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    private static partial ushort RegisterClass(in WNDCLASS lpWndClass);

    [LibraryImport("user32.dll", EntryPoint = "GetMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(in MSG lpMsg);

    [LibraryImport("user32.dll")]
    private static partial IntPtr DispatchMessageW(in MSG lpMsg);

    [LibraryImport("user32.dll")]
    private static partial void PostQuitMessage(int nExitCode);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string? lpModuleName);

    public void StartMonitoring()
    {
        if (this._messageLoopThread != null)
            return;

        this._messageLoopThread = new Thread(() =>
        {
            // Set up STA for the message window
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            var className = $"ClipboardMonitor_{Guid.NewGuid():N}";
            this._windowProc = this.WndProc;

            var wndClass = new WNDCLASS
            {
                lpfnWndProc = this._windowProc, hInstance = GetModuleHandle(null), lpszClassName = className
            };

            var classAtom = RegisterClass(in wndClass);
            if (classAtom == 0)
                return;

            // Create message-only window
            this._hwnd = CreateWindowEx(
                0, className, "ClipboardMonitor",
                0, 0, 0, 0, 0,
                HWND_MESSAGE, IntPtr.Zero, wndClass.hInstance, IntPtr.Zero);

            if (this._hwnd == IntPtr.Zero)
                return;

            // Register for clipboard notifications
            AddClipboardFormatListener(this._hwnd);

            // Message loop
            while (GetMessage(out MSG msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(in msg);
                DispatchMessageW(in msg);
            }
        }) { IsBackground = true, Name = "ClipboardMonitor", };
        this._messageLoopThread.SetApartmentState(ApartmentState.STA);
        this._messageLoopThread.Start();
    }

    public void StopMonitoring()
    {
        if (this._hwnd != IntPtr.Zero)
        {
            RemoveClipboardFormatListener(this._hwnd);
            DestroyWindow(this._hwnd);
            this._hwnd = IntPtr.Zero;
        }

        if (this._messageLoopThread != null)
        {
            PostQuitMessage(0);
            this._messageLoopThread.Join(5000);
            this._messageLoopThread = null;
        }
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_CLIPBOARDUPDATE:
                Task.Run(() => this.ClipboardChanged?.Invoke(this, EventArgs.Empty));
                return IntPtr.Zero;

            case WM_DESTROY:
                PostQuitMessage(0);
                return IntPtr.Zero;
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private delegate IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);


    // Managed representation
    [NativeMarshalling(typeof(WndClassMarshaller))]
    private struct WNDCLASS
    {
        public uint style;
        public WindowProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
    }

    // Native representation for marshalling
    [StructLayout(LayoutKind.Sequential)]
    private struct WndClassNative
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public IntPtr lpszMenuName;
        public IntPtr lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public int pt_x;
        public int pt_y;
    }

    [CustomMarshaller(typeof(WNDCLASS), MarshalMode.ManagedToUnmanagedIn, typeof(WndClassMarshaller))]
    private static class WndClassMarshaller
    {
        public static WndClassNative ConvertToUnmanaged(WNDCLASS managed)
        {
            return new WndClassNative
            {
                style = managed.style,
                lpfnWndProc = managed.lpfnWndProc != null
                    ? Marshal.GetFunctionPointerForDelegate(managed.lpfnWndProc)
                    : IntPtr.Zero,
                cbClsExtra = managed.cbClsExtra,
                cbWndExtra = managed.cbWndExtra,
                hInstance = managed.hInstance,
                hIcon = managed.hIcon,
                hCursor = managed.hCursor,
                hbrBackground = managed.hbrBackground,
                lpszMenuName = managed.lpszMenuName != null
                    ? Marshal.StringToHGlobalUni(managed.lpszMenuName)
                    : IntPtr.Zero,
                lpszClassName = Marshal.StringToHGlobalUni(managed.lpszClassName)
            };
        }

        public static void Free(WndClassNative unmanaged)
        {
            if (unmanaged.lpszMenuName != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanaged.lpszMenuName);
            }

            if (unmanaged.lpszClassName != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanaged.lpszClassName);
            }
        }
    }
}