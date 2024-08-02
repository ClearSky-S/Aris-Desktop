using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
    
    [DllImport("Dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    
    // set layered window attribute
    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    
    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    const uint LWA_COLORKEY = 0x00000001;
    private IntPtr _hwnd;

    private int _displayIndex;
    // Start is called before the first frame update
    void Start()
    {
        _hwnd = GetActiveWindow();
        
        // 배경 투명하게
        MARGINS margins = new () {cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(_hwnd, ref margins);

        #if !UNITY_EDITOR
        SetWindowPos(_hwnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
        // 윈도우 통과해서 상호작용 가능
        SetWindowLong(_hwnd, GWL_EXSTYLE, new IntPtr(WS_EX_LAYERED));
        // 상호작용 가능한 색상 범위 설정
        SetLayeredWindowAttributes(_hwnd, 0, 0, LWA_COLORKEY);
        #endif
    }
    
    public void ShowMessageBox(string message)
    {
        var box = MessageBox(IntPtr.Zero, message, "알림", 0);
        Debug.Log(box);
    }
    
    public void MoveToMonitor(int monitorIndex)
    {
        // Screen.fullScreen = false;
        List<DisplayInfo> displayInfos = new();
        Screen.GetDisplayLayout(displayInfos);
        if(displayInfos.Count <= monitorIndex)
        {
            monitorIndex = 0;
        }
        _displayIndex = monitorIndex;
        DisplayInfo displayInfo = displayInfos[monitorIndex];
        Screen.MoveMainWindowTo(displayInfo, Vector2Int.zero);
        // Screen.fullScreen = true;
    }
    public void NextMonitor()
    {
        MoveToMonitor(_displayIndex + 1);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
