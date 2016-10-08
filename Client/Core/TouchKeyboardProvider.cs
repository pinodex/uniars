using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.IO;

public class TouchKeyboardProvider
{
    private readonly string virtualKeyboardPath;

    public TouchKeyboardProvider()
    {
        virtualKeyboardPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
            @"Microsoft Shared\ink\TabTip.exe");
    }

    public void ShowTouchKeyboard()
    {
        if (!IsSupported) return;

        try
        {
            Process.Start(virtualKeyboardPath);
        }
        catch { }
    }

    public void HideTouchKeyboard()
    {
        if (!IsSupported) return;

        var nullIntPtr = new IntPtr(0);
        const uint wmSyscommand = 0x0112;
        var scClose = new IntPtr(0xF060);

        var keyboardWnd = FindWindow("IPTip_Main_Window", null);
        if (keyboardWnd != nullIntPtr)
        {
            SendMessage(keyboardWnd, wmSyscommand, scClose, nullIntPtr);
        }
    }

    public bool IsSupported
    {
        get
        {
            return File.Exists(virtualKeyboardPath);
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string sClassName, string sAppName);

    [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);
}