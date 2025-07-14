// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local", Justification = "Allow use of Win32 naming conventions")]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Allow use of Win32 naming conventions")]
internal sealed partial class AppLifeMonitorNativeMethods
{
    internal const uint WS_POPUP = 0x80000000;
    internal const int GWL_WNDPROC = -4;
    internal const int GCL_WNDPROC = -24;

    internal const uint WM_DESTROY = 0x0002;
    internal const uint WM_CLOSE = 0x0010;
    internal const uint WM_QUERYENDSESSION = 0x0011;
    internal const uint WM_QUIT = 0x0012;
    internal const uint WM_ENDSESSION = 0x0016;



    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateWindowEx(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam);



    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(nint hWnd);



    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindow(nint hWnd);



    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static partial nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);



    [LibraryImport("user32.dll", EntryPoint = "RegisterClassW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial ushort RegisterClass(in WndClass lpWndClass);



    [LibraryImport("user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterClass([MarshalAs(UnmanagedType.LPWStr)] string lpClassName, nint hInstance);



    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);



    [LibraryImport("user32.dll", EntryPoint = "SetClassLongPtrW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetClassLongPtr(nint hWnd, int nIndex, nint dwNewLong);



    [LibraryImport("user32.dll", EntryPoint = "GetMessageW")]
    internal static partial int GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);



    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in MSG lpMsg);



    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    internal static partial nint DispatchMessage(in MSG lpMsg);



    [LibraryImport("user32.dll", EntryPoint = "PostQuitMessageW")]
    internal static partial void PostQuitMessage(int nExitCode);



    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string? lpModuleName);



    [LibraryImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true)]
    internal static partial nint GetProcAddress(nint hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);



    [LibraryImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessage(nint hWnd, uint msg, nint wParam, nint lParam);



    [LibraryImport("user32.dll", EntryPoint = "PostThreadMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostThreadMessage(uint idThread, uint msg, nint wParam, nint lParam);



    [LibraryImport("kernel32.dll", SetLastError = false)]
    internal static partial uint GetCurrentThreadId();



    internal delegate nint WindowProc(nint hWnd, uint msg, nint wParam, nint lParam);


    // Managed representation
    [NativeMarshalling(typeof(WndClassMarshaller))]
    internal struct WndClass
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public uint style;
        public WindowProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    // Native representation for marshalling
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct WndClassNative
    {
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public ushort* lpszMenuName;
        public ushort* lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public nint hwnd;
        public uint message;
        public nint wParam;
        public nint lParam;
        public uint time;
        public int pt_x;
        public int pt_y;
    }

    [CustomMarshaller(typeof(WndClass), MarshalMode.ManagedToUnmanagedIn, typeof(WndClassMarshaller))]
    internal static class WndClassMarshaller
    {
        public static unsafe WndClassNative ConvertToUnmanaged(WndClass managed)
        {
            return new()
            {
                style = managed.style,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(managed.lpfnWndProc),
                cbClsExtra = managed.cbClsExtra,
                cbWndExtra = managed.cbWndExtra,
                hInstance = managed.hInstance,
                hIcon = managed.hIcon,
                hCursor = managed.hCursor,
                hbrBackground = managed.hbrBackground,
                lpszMenuName = managed.lpszMenuName != null
                    ? Utf16StringMarshaller.ConvertToUnmanaged(managed.lpszMenuName)
                    : null,
                lpszClassName = Utf16StringMarshaller.ConvertToUnmanaged(managed.lpszClassName)
            };
        }



        public static unsafe void Free(WndClassNative unmanaged)
        {
            if (unmanaged.lpszMenuName != null)
            {
                Utf16StringMarshaller.Free(unmanaged.lpszMenuName);
            }

            if (unmanaged.lpszClassName != null)
            {
                Utf16StringMarshaller.Free(unmanaged.lpszClassName);
            }
        }
    }
}