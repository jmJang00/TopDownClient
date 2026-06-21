using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ApplicationUtil
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(
        IntPtr hWnd,
        string text,
        string caption,
        uint type);

    public static void ShowErrorAndQuit(
        string message,
        string title = "오류")
    {
        MessageBox(
            IntPtr.Zero,
            message,
            title,
            0);

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif
    }
}
